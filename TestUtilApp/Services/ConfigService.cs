using System;
using System.IO;
using Newtonsoft.Json;
using TestUtilApp.Models;

namespace TestUtilApp.Services
{
    public class ConfigService
    {
        private const string CONFIG_FILE_NAME = "SettingConfig.json";
        private const string CONFIG_FOLDER_NAME = "Config";
        private readonly string _configFilePath;
        private readonly string _projectRootConfigPath;

        public ConfigService()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string configFolder = Path.Combine(appDirectory, CONFIG_FOLDER_NAME);
            Directory.CreateDirectory(configFolder);
            _configFilePath = Path.Combine(configFolder, CONFIG_FILE_NAME);

            // 프로젝트 루트 경로 찾기 (bin\Debug 또는 bin\Release에서 3단계 상위)
            _projectRootConfigPath = FindProjectRootConfigPath(appDirectory);
        }

        /// <summary>
        /// 프로젝트 루트 폴더의 Config\SettingConfig.json 경로 찾기
        /// </summary>
        private string FindProjectRootConfigPath(string appDirectory)
        {
            try
            {
                DirectoryInfo current = new DirectoryInfo(appDirectory);

                // bin\Debug 또는 bin\Release 구조에서 프로젝트 루트 찾기
                // 예: C:\Projects\MyApp\bin\Debug -> C:\Projects\MyApp
                while (current != null)
                {
                    // bin 폴더 찾기
                    if (current.Name.Equals("bin", StringComparison.OrdinalIgnoreCase))
                    {
                        // bin의 부모 = 프로젝트 루트
                        if (current.Parent != null)
                        {
                            string projectRootPath = Path.Combine(current.Parent.FullName, CONFIG_FOLDER_NAME, CONFIG_FILE_NAME);
                            return projectRootPath;
                        }
                    }
                    current = current.Parent;
                }

                // bin 폴더를 못 찾으면 null 반환 (실행 폴더만 사용)
                return null;
            }
            catch
            {
                return null;
            }
        }

        public AppConfig LoadConfig()
        {
            try
            {
                // 1. 프로젝트 루트와 실행 폴더의 SettingConfig.json 동기화
                bool needsCopy = false;

                if (!string.IsNullOrEmpty(_projectRootConfigPath) && File.Exists(_projectRootConfigPath))
                {
                    if (!File.Exists(_configFilePath))
                    {
                        // 실행 폴더에 SettingConfig.json이 없으면 복사
                        needsCopy = true;
                    }
                    else
                    {
                        // 두 파일 모두 존재하는 경우, 수정 시간 비교
                        DateTime projectRootModified = File.GetLastWriteTime(_projectRootConfigPath);
                        DateTime execFolderModified = File.GetLastWriteTime(_configFilePath);

                        // 프로젝트 루트 파일이 더 최신이면 복사
                        if (projectRootModified > execFolderModified)
                        {
                            needsCopy = true;
                        }
                    }

                    if (needsCopy)
                    {
                        // 프로젝트 루트의 SettingConfig.json을 실행 폴더로 복사
                        File.Copy(_projectRootConfigPath, _configFilePath, true);
                    }
                }
                else if (!File.Exists(_configFilePath))
                {
                    // 프로젝트 루트에도 없고 실행 폴더에도 없으면 기본 설정 생성
                    var defaultConfig = CreateDefaultConfig();
                    SaveConfig(defaultConfig);
                    return defaultConfig;
                }

                string jsonContent = File.ReadAllText(_configFilePath);
                AppConfig config = null;

                try
                {
                    config = JsonConvert.DeserializeObject<AppConfig>(jsonContent);
                }
                catch (JsonException jsonEx)
                {
                    // JSON 파싱 오류 시 백업 후 기본 설정 생성
                    string backupPath = _configFilePath + $".backup_{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(_configFilePath, backupPath);

                    var defaultConfig = CreateDefaultConfig();
                    SaveConfig(defaultConfig);

                    throw new Exception(
                        $"The settings file format is invalid, so default settings were created.\n" +
                        $"The existing file was backed up: {backupPath}\n\n" +
                        $"Error details: {jsonEx.Message}", jsonEx);
                }

                if (config == null || config.DetectionClasses == null || config.DetectionClasses.Count == 0)
                {
                    return CreateDefaultConfig();
                }

                // ★ LabelGeneration 설정이 없으면 기본값 설정 ★
                if (config.LabelGeneration == null)
                {
                    config.LabelGeneration = new LabelGenerationConfig
                    {
                        MinConfidence = 0.5f,
                        SkipExistingJson = true
                    };
                }

                EnsureClassificationDefaults(config);

                // ★ FileFilter 설정 체크 ★
                if (config.FileFilter == null)
                {
                    config.FileFilter = CreateDefaultFileFilterConfig();
                }
                else
                {
                    EnsureFileFilterDefaults(config.FileFilter);
                }

                if (string.IsNullOrWhiteSpace(config.DiceVersion))
                {
                    config.DiceVersion = "v2";
                }

                EnsureInspectionConditionDefaults(config);

                return config;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while loading the settings file: {ex.Message}", ex);
            }
        }

        public void SaveConfig(AppConfig config)
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include,
                    DefaultValueHandling = DefaultValueHandling.Include
                };

                string jsonContent = JsonConvert.SerializeObject(config, settings);

                // 1. 실행 폴더에 저장 (bin\Debug 또는 bin\Release)
                File.WriteAllText(_configFilePath, jsonContent);

                // 2. 프로젝트 루트에도 저장 (찾은 경우만)
                if (!string.IsNullOrEmpty(_projectRootConfigPath))
                {
                    try
                    {
                        File.WriteAllText(_projectRootConfigPath, jsonContent);
                    }
                    catch (Exception ex)
                    {
                        // 프로젝트 루트 저장 실패는 무시 (실행 폴더는 이미 저장됨)
                        System.Diagnostics.Debug.WriteLine($"Failed to save project root SettingConfig.json: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while saving the settings file: {ex.Message}", ex);
            }
        }

        private AppConfig CreateDefaultConfig()
        {
            return new AppConfig
            {
                DetectionClasses = new System.Collections.Generic.List<string>
                {
                    "BELT_TOP",
                    "BELT_BOTTOM"
                },
                DefaultPadding = 10,
                DefaultCropArea = new CropArea
                {
                    X = 100,
                    Y = 100,
                    Width = 200,
                    Height = 200
                },
                DiceModels = new DiceModelsConfig
                {
                    DetectModel = new DiceModelSetting
                    {
                        Use = true,
                        Path = "C:\\DICE_PACKAGE\\models\\belt_detection.dice"
                    },
                    ClassifyModel_A = new DiceModelSetting
                    {
                        Use = true,
                        Path = "C:\\DICE_PACKAGE\\models\\belt_cls_top.dice"
                    },
                    ClassifyModel_B = new DiceModelSetting
                    {
                        Use = true,
                        Path = "C:\\DICE_PACKAGE\\models\\belt_cls_bottom.dice"
                    }
                },
                LabelGeneration = new LabelGenerationConfig
                {
                    MinConfidence = 0.5f,
                    SkipExistingJson = true
                },
                Classification = new ClassificationConfig
                {
                    MinConfidence = 0.5f
                },
                FileFilter = CreateDefaultFileFilterConfig(),
                LastCropSourceFolder = "",
                LastClassifySourceFolder = "",
                LastLabelGenSourceFolder = "",
                InspectionSpecJsonPath = @"D:\tmp3\GMAPInspSpec.json",
                InspectionConditions = CreateDefaultInspectionConditions()
            };
        }

        private void EnsureInspectionConditionDefaults(AppConfig config)
        {
            if (config.InspectionConditions == null || config.InspectionConditions.Count == 0)
            {
                config.InspectionConditions = CreateDefaultInspectionConditions();
                return;
            }

            foreach (var condition in config.InspectionConditions)
            {
                if (condition.DetectClassKeywords == null)
                {
                    condition.DetectClassKeywords = new System.Collections.Generic.List<string>();
                }

                if (string.IsNullOrWhiteSpace(condition.ClassifyModel))
                {
                    condition.ClassifyModel = "A";
                }

                condition.ClassifyModel = condition.ClassifyModel.Trim().Equals("B", StringComparison.OrdinalIgnoreCase)
                    ? "B"
                    : "A";
            }
        }

        private System.Collections.Generic.List<InspectionConditionRule> CreateDefaultInspectionConditions()
        {
            return new System.Collections.Generic.List<InspectionConditionRule>
            {
                new InspectionConditionRule
                {
                    Use = true,
                    FileNameKeyword = "Tub",
                    DetectClassKeywords = new System.Collections.Generic.List<string> { "Tub", "Drain" },
                    ClassifyModel = "A"
                },
                new InspectionConditionRule
                {
                    Use = true,
                    FileNameKeyword = "Bubble",
                    DetectClassKeywords = new System.Collections.Generic.List<string> { "Bubble", "Air" },
                    ClassifyModel = "B"
                }
            };
        }

        private FileFilterConfig CreateDefaultFileFilterConfig()
        {
            var fileFilter = new FileFilterConfig
            {
                OutputPostfix = "_filtered",
                LastOutputFolder = "",
                UseCustomOutputFolder = false,
                ActiveProfileIndex = 0,
                ConditionProfiles = new System.Collections.Generic.List<FileFilterProfile>
                {
                    new FileFilterProfile
                    {
                        Name = "Preset 1",
                        IncludeFolderKeywords = new System.Collections.Generic.List<string> { "LED", "Blade" },
                        IncludeFileKeywords = new System.Collections.Generic.List<string>(),
                        ExcludeFileKeywords = new System.Collections.Generic.List<string> { "_temp", "backup" },
                        ExcludeExtensions = new System.Collections.Generic.List<string> { "tmp", "bak" },
                        UseFolderIncludeFilter = true,
                        UseFileIncludeFilter = false,
                        UseFileExcludeFilter = true,
                        UseExtensionExcludeFilter = true
                    }
                }
            };

            EnsureFileFilterDefaults(fileFilter);
            return fileFilter;
        }

        private void EnsureClassificationDefaults(AppConfig config)
        {
            if (config.Classification == null)
            {
                config.Classification = new ClassificationConfig
                {
                    MinConfidence = 0.5f
                };
            }

            if (config.Classification.MinConfidence < 0f || config.Classification.MinConfidence > 1f)
            {
                config.Classification.MinConfidence = 0.5f;
            }
        }

        private void EnsureFileFilterDefaults(FileFilterConfig fileFilter)
        {
            if (string.IsNullOrEmpty(fileFilter.OutputPostfix))
            {
                fileFilter.OutputPostfix = "_filtered";
            }
            if (fileFilter.LastOutputFolder == null)
            {
                fileFilter.LastOutputFolder = "";
            }

            if (fileFilter.ConditionProfiles == null)
            {
                fileFilter.ConditionProfiles = new System.Collections.Generic.List<FileFilterProfile>();
            }

            if (fileFilter.ConditionProfiles.Count == 0)
            {
                fileFilter.ConditionProfiles.Add(new FileFilterProfile
                {
                    Name = "Preset 1",
                    IncludeFolderKeywords = new System.Collections.Generic.List<string> { "LED", "Blade" },
                    IncludeFileKeywords = new System.Collections.Generic.List<string>(),
                    ExcludeFileKeywords = new System.Collections.Generic.List<string> { "_temp", "backup" },
                    ExcludeExtensions = new System.Collections.Generic.List<string> { "tmp", "bak" },
                    UseFolderIncludeFilter = true,
                    UseFileIncludeFilter = false,
                    UseFileExcludeFilter = true,
                    UseExtensionExcludeFilter = true
                });
            }

            while (fileFilter.ConditionProfiles.Count < 3)
            {
                int presetNumber = fileFilter.ConditionProfiles.Count + 1;
                fileFilter.ConditionProfiles.Add(new FileFilterProfile
                {
                    Name = $"Preset {presetNumber}",
                    IncludeFolderKeywords = new System.Collections.Generic.List<string>(),
                    IncludeFileKeywords = new System.Collections.Generic.List<string>(),
                    ExcludeFileKeywords = new System.Collections.Generic.List<string>(),
                    ExcludeExtensions = new System.Collections.Generic.List<string>(),
                    UseFolderIncludeFilter = true,
                    UseFileIncludeFilter = false,
                    UseFileExcludeFilter = true,
                    UseExtensionExcludeFilter = true
                });
            }

            for (int i = 0; i < fileFilter.ConditionProfiles.Count; i++)
            {
                var profile = fileFilter.ConditionProfiles[i];

                if (string.IsNullOrEmpty(profile.Name))
                {
                    profile.Name = $"Preset {i + 1}";
                }
                if (profile.IncludeFolderKeywords == null)
                {
                    profile.IncludeFolderKeywords = new System.Collections.Generic.List<string>();
                }
                if (profile.ExcludeFileKeywords == null)
                {
                    profile.ExcludeFileKeywords = new System.Collections.Generic.List<string>();
                }
                if (profile.IncludeFileKeywords == null)
                {
                    profile.IncludeFileKeywords = new System.Collections.Generic.List<string>();
                }
                if (profile.ExcludeExtensions == null)
                {
                    profile.ExcludeExtensions = new System.Collections.Generic.List<string>();
                }

            }

            if (fileFilter.ActiveProfileIndex < 0 || fileFilter.ActiveProfileIndex >= fileFilter.ConditionProfiles.Count)
            {
                fileFilter.ActiveProfileIndex = 0;
            }
        }

        public string GetConfigFilePath()
        {
            return _configFilePath;
        }

        public string GetProjectRootConfigPath()
        {
            return _projectRootConfigPath;
        }

        public bool HasProjectRootConfig()
        {
            return !string.IsNullOrEmpty(_projectRootConfigPath);
        }
    }
}
