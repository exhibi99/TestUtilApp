using TestUtilApp.Dice;
using Newtonsoft.Json;
using OpenCvSharp;
using TestUtilApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestUtilApp.Services
{
    /// <summary>
    /// Detection 결과를 Labeler 포맷의 JSON 파일로 생성하는 서비스
    /// </summary>
    public class LabelGeneratorService
    {
        private AppConfig _config;

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgressUpdate;

        public LabelGeneratorService(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Config 업데이트
        /// </summary>
        public void UpdateConfig(AppConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// 폴더 내 모든 이미지에 대해 Detection 수행 후 JSON 라벨 생성
        /// </summary>
        public void GenerateLabelsForFolder(string sourceFolderPath)
        {
            if (!Directory.Exists(sourceFolderPath))
            {
                throw new ArgumentException("The source folder does not exist.");
            }

            if (!DiceManager.IsDetectModelLoaded(_config.DiceModels?.DetectModel?.Path))
            {
                throw new InvalidOperationException("The detection model is not loaded.");
            }

            string[] imageFiles = GetImageFiles(sourceFolderPath);
            int processedImages = 0;
            int skippedExisting = 0;
            int createdLabels = 0;

            OnLogMessage?.Invoke($"Starting label generation for {imageFiles.Length} image(s)...");
            OnLogMessage?.Invoke($"Settings - MinConfidence: {_config.LabelGeneration.MinConfidence}, SkipExistingJson: {_config.LabelGeneration.SkipExistingJson}");

            foreach (string imagePath in imageFiles)
            {
                try
                {
                    // JSON 파일 경로 확인
                    string jsonPath = Path.ChangeExtension(imagePath, ".json");

                    // ★ 기존 JSON 파일이 있고 SkipExistingJson이 true이면 건너뛰기 ★
                    if (_config.LabelGeneration.SkipExistingJson && File.Exists(jsonPath))
                    {
                        OnLogMessage?.Invoke($"Skipped existing JSON: {Path.GetFileName(jsonPath)}");
                        skippedExisting++;
                        processedImages++;
                        OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                        continue;
                    }

                    using (Mat image = Cv2.ImRead(imagePath, ImreadModes.Color))
                    {
                        if (image.Empty())
                        {
                            OnLogMessage?.Invoke($"Image load failed: {Path.GetFileName(imagePath)}");
                            processedImages++;
                            OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                            continue;
                        }

                        // Detection 수행
                        List<ResultDiceDet> diceResults = DiceManager.DetectModel.Inference(image);

                        if (diceResults == null || diceResults.Count == 0)
                        {
                            OnLogMessage?.Invoke($"No detection results: {Path.GetFileName(imagePath)}");
                            processedImages++;
                            OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                            continue;
                        }

                        // JSON 라벨 생성
                        var labelData = ConvertToLabelFormat(diceResults[0], image.Size());

                        // 유효한 shape가 하나도 없으면 JSON 생성하지 않음
                        if (labelData.shapes.Count == 0)
                        {
                            OnLogMessage?.Invoke($"No valid detection results: {Path.GetFileName(imagePath)}");
                            processedImages++;
                            OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                            continue;
                        }

                        // JSON 파일 저장
                        SaveJsonLabel(jsonPath, labelData);
                        createdLabels++;

                        OnLogMessage?.Invoke($"Label generated: {Path.GetFileName(jsonPath)} ({labelData.shapes.Count} detection(s))");
                    }

                    processedImages++;
                    OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Error occurred ({Path.GetFileName(imagePath)}): {ex.Message}");
                    processedImages++;
                    OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                }
            }

            OnLogMessage?.Invoke($"Label generation completed: {processedImages} file(s) processed");
        }

        /// <summary>
        /// Detection 결과를 Labeler JSON 포맷으로 변환
        /// </summary>
        private LabelData ConvertToLabelFormat(ResultDiceDet detectionResult, OpenCvSharp.Size imageSize)
        {
            // ★ Config에서 MinConfidence 가져오기 ★
            float minConf = _config.LabelGeneration.MinConfidence;

            var labelData = new LabelData
            {
                version = "0.10.7",
                task_type = "det",
                imageHeight = imageSize.Height,
                imageWidth = imageSize.Width,
                imageDepth = 3,
                split = "train",
                shapes = new List<Shape>()
            };

            foreach (var rect in detectionResult.listRect)
            {
                // Rect 정보: left-top (x, y), right-bottom (x, y)
                float x1 = rect.rect.X;
                float y1 = rect.rect.Y;
                float x2 = rect.rect.X + rect.rect.Width;
                float y2 = rect.rect.Y + rect.rect.Height;

                // 이미지 범위를 벗어나는 경우 제외
                if (x1 < 0 || y1 < 0 || x2 > imageSize.Width || y2 > imageSize.Height)
                {
                    OnLogMessage?.Invoke($"Skipped out-of-bounds result: {rect.class_name} [{x1}, {y1}, {x2}, {y2}]");
                    continue;
                }

                // 유효하지 않은 rect 크기 제외
                if (rect.rect.Width <= 0 || rect.rect.Height <= 0)
                {
                    OnLogMessage?.Invoke($"Skipped invalid-size result: {rect.class_name}");
                    continue;
                }

                // ★ MinConfidence보다 낮은 경우 제외 ★
                if (rect.conf < minConf)
                {
                    OnLogMessage?.Invoke($"Skipped low-confidence result: {rect.class_name}, {rect.conf:F3} < {minConf:F3}");
                    continue;
                }

                var shape = new Shape
                {
                    label = rect.class_name,
                    points = new List<List<double>>
                    {
                        new List<double> { x1, y1 },  // 좌측 상단
                        new List<double> { x2, y2 }   // 우측 하단
                    },
                    group_id = null,
                    shape_type = "rectangle",
                    flags = new Dictionary<string, object>()
                };

                labelData.shapes.Add(shape);
            }

            return labelData;
        }

        /// <summary>
        /// JSON 라벨 파일 저장
        /// </summary>
        private void SaveJsonLabel(string jsonPath, LabelData labelData)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            };

            string jsonContent = JsonConvert.SerializeObject(labelData, settings);
            File.WriteAllText(jsonPath, jsonContent);
        }

        /// <summary>
        /// 이미지 파일 목록 가져오기
        /// </summary>
        private string[] GetImageFiles(string folderPath)
        {
            string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };
            return extensions.SelectMany(ext =>
                Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories)
            ).ToArray();
        }
    }

    #region JSON Label Data Classes

    public class LabelData
    {
        public string version { get; set; }
        public string task_type { get; set; }
        public List<Shape> shapes { get; set; }
        public string split { get; set; }
        public int imageHeight { get; set; }
        public int imageWidth { get; set; }
        public int imageDepth { get; set; }
    }

    public class Shape
    {
        public string label { get; set; }
        public List<List<double>> points { get; set; }
        public object group_id { get; set; }
        public string shape_type { get; set; }
        public Dictionary<string, object> flags { get; set; }
    }

    #endregion
}


