using OpenCvSharp;
using TestUtilApp.Dice;
using TestUtilApp.Models;
using TestUtilApp.Process;
using TestUtilApp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using static TestUtilApp.Process.DefineAlgorithm;

namespace TestUtilApp.Services
{
    public class ImageProcessorService
    {
        private readonly AppConfig _config;

        public event Action<string> OnLogMessage;
        public event Action<int, int> OnProgressUpdate;

        public ImageProcessorService(AppConfig config)
        {
            _config = config;
        }

        public class ImageInfo
        {
            public string Name = "";
            public Mat SourceImage = new Mat();

            public bool IsDetectRects = false;

            public ResultDiceDet detInfo;
        }

        private class ClassificationResultInfo
        {
            public string ClassName { get; set; }
            public float Confidence { get; set; }
        }

        /// <summary>
        /// 이미지 크롭 처리 (모든 detect 클래스 처리)
        /// </summary>
        public void ProcessCropImages(string sourceFolderPath, string cropFolderPath)
        {
            if (!Directory.Exists(cropFolderPath))
            {
                Directory.CreateDirectory(cropFolderPath);
            }

            string[] imageFiles = GetImageFiles(sourceFolderPath);
            int processedImages = 0;

            foreach (string imagePath in imageFiles)
            {
                try
                {
                    using (Mat originalImage = Cv2.ImRead(imagePath, ImreadModes.Color))
                    {
                        if (originalImage.Empty())
                        {
                            OnLogMessage?.Invoke($"Image load failed: {Path.GetFileName(imagePath)}");
                            continue;
                        }

                        ImageInfo imgInfo = new ImageInfo
                        {
                            Name = Path.GetFileName(imagePath),
                            SourceImage = originalImage.Clone()
                        };

                        // 객체 검출 수행
                        RESULT detectResult = ObjectDetPos(ref imgInfo);

                        if (detectResult == RESULT.OK && imgInfo.IsDetectRects)
                        {
                            // 각 검출된 클래스별로 크롭 수행
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(imagePath);
                            string fileExt = Path.GetExtension(imagePath);
                            
                            foreach (var rectInfo in imgInfo.detInfo.listRect)
                            {
                                if (rectInfo.rect.Width > 0 && rectInfo.rect.Height > 0)
                                {
                                    CropAndSave(originalImage, BasicAlgoRect.IntRect(rectInfo.rect),
                                        cropFolderPath, rectInfo.class_name, fileNameWithoutExt, fileExt, imagePath, sourceFolderPath);
                                }
                            }
                        }
                        else
                        {
                            OnLogMessage?.Invoke($"Detection failed or no object detected: {Path.GetFileName(imagePath)}");
                        }
                    }

                    processedImages++;
                    OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Crop error: {Path.GetFileName(imagePath)} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 수동 크롭 처리 (고정 영역 사용)
        /// </summary>
        public void ProcessManualCropImages(string sourceFolderPath, string cropFolderPath, Rect fixedCropArea)
        {
            if (!Directory.Exists(cropFolderPath))
            {
                Directory.CreateDirectory(cropFolderPath);
            }

            // 수동 크롭용 폴더 생성
            string manualFolderPath = Path.Combine(cropFolderPath, "MANUAL");
            if (!Directory.Exists(manualFolderPath))
            {
                Directory.CreateDirectory(manualFolderPath);
            }

            string[] imageFiles = GetImageFiles(sourceFolderPath);
            int processedImages = 0;

            foreach (string imagePath in imageFiles)
            {
                try
                {
                    using (Mat originalImage = Cv2.ImRead(imagePath, ImreadModes.Color))
                    {
                        if (originalImage.Empty())
                        {
                            OnLogMessage?.Invoke($"Image load failed: {Path.GetFileName(imagePath)}");
                            continue;
                        }

                        // 크롭 영역 조정
                        Rect adjustedRect = AdjustCropArea(fixedCropArea, originalImage.Size());

                        if (adjustedRect.Width <= 0 || adjustedRect.Height <= 0)
                        {
                            OnLogMessage?.Invoke($"Crop area is outside the image bounds: {Path.GetFileName(imagePath)}");
                            continue;
                        }

                        // 이미지 크롭
                        using (Mat croppedImage = new Mat(originalImage, adjustedRect))
                        {
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(imagePath);
                            string fileExt = Path.GetExtension(imagePath);
                            string newFileName = $"MANUAL_{fileNameWithoutExt}{fileExt}";

                            // 상대 경로 유지
                            string relativePath = GetRelativePath(sourceFolderPath, Path.GetDirectoryName(imagePath));
                            string outputDir = Path.Combine(manualFolderPath, relativePath);

                            if (!Directory.Exists(outputDir))
                            {
                                Directory.CreateDirectory(outputDir);
                            }

                            string outputPath = Path.Combine(outputDir, newFileName);
                            Cv2.ImWrite(outputPath, croppedImage);

                            OnLogMessage?.Invoke($"Manual crop completed: MANUAL/{relativePath}/{newFileName}");
                        }
                    }

                    processedImages++;
                    OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Manual crop error: {Path.GetFileName(imagePath)} - {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 크롭 및 저장
        /// </summary>
        private void CropAndSave(Mat originalImage, Rect cropRect, string cropFolderPath,
            string className, string fileNameWithoutExt, string fileExt, string originalPath, string sourceFolderPath)
        {
            try
            {
                // 크롭 영역 조정
                Rect adjustedRect = AdjustCropArea(cropRect, originalImage.Size());

                // 클래스별 폴더 생성
                string classFolderPath = Path.Combine(cropFolderPath, className);
                if (!Directory.Exists(classFolderPath))
                {
                    Directory.CreateDirectory(classFolderPath);
                }

                // 이미지 크롭
                using (Mat croppedImage = new Mat(originalImage, adjustedRect))
                {
                    // 새 파일명 생성: {ClassName}_{원본파일명}.{확장자}
                    string newFileName = $"{className}_{fileNameWithoutExt}{fileExt}";

                    // 상대 경로 유지 (하위 폴더 구조 보존)
                    string relativePath = GetRelativePath(sourceFolderPath, Path.GetDirectoryName(originalPath));
                    string outputDir = Path.Combine(classFolderPath, relativePath);

                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    string outputPath = Path.Combine(outputDir, newFileName);
                    Cv2.ImWrite(outputPath, croppedImage);

                    OnLogMessage?.Invoke($"Crop completed: {className}/{relativePath}/{newFileName}");
                }
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Crop save error ({className}): {ex.Message}");
            }
        }

        /// <summary>
        /// 객체 검출 (DetectModel 모델 사용)
        /// </summary>
        private static RESULT ObjectDetPos(ref ImageInfo imageInfo)
        {
            try
            {
                List<ResultDiceDet> diceResults = DiceManager.DetectModel.Inference(imageInfo.SourceImage);

                if (diceResults == null || diceResults.Count == 0)
                {
                    Console.WriteLine("No detection results");
                    return RESULT.ERR;
                }

                if (diceResults.Count != 1)
                {
                    Console.WriteLine($"CountErr : dice({diceResults.Count}) != img(1)");
                    return RESULT.ERR;
                }

                var result = diceResults[0];
                bool hasDetection = false;

                for (int i = result.listRect.Count - 1; i >= 0; i--)
                {
                    if (result.listRect[i].conf < 0.5)
                    {
                        result.listRect.RemoveAt(i);
                    }
                    else
                    {
                        hasDetection = true;
                    }
                }

                imageInfo.IsDetectRects = hasDetection;
                imageInfo.detInfo = result;

                return hasDetection ? RESULT.OK : RESULT.ERR;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObjectDetPos error: {ex.Message}");
                return RESULT.ERR;
            }
        }

        /// <summary>
        /// 이미지 분류 처리
        /// </summary>
        public Dictionary<string, List<string>> ProcessClassification(
            string sourceFolderPath,
            string selectedModels)
        {
            var classificationResults = new Dictionary<string, List<string>>();

            string parentFolder = Directory.GetParent(sourceFolderPath).FullName;
            string folderName = Path.GetFileName(sourceFolderPath);
            string classifiedFolder = Path.Combine(parentFolder, folderName + "_classified");

            if (!Directory.Exists(classifiedFolder))
            {
                Directory.CreateDirectory(classifiedFolder);
            }

            string[] imageFiles = GetImageFiles(sourceFolderPath);
            int processedImages = 0;

            //_ClsInference(selectedModels, imageFiles);


            foreach (string imagePath in imageFiles)
            {
                try
                {
                    using (Mat image = Cv2.ImRead(imagePath, ImreadModes.Color))
                    {
                        if (image.Empty())
                        {
                            OnLogMessage?.Invoke($"Image load failed: {Path.GetFileName(imagePath)}");
                            continue;
                        }

                        // 선택된 모델로 분류 수행
                        {
                            ClassificationResultInfo classification = ClassifyWithModel(image, imagePath, selectedModels);
                            string classificationName = classification.ClassName;

                            // 결과 키: {ModelName}_{ClassificationResult}
                            string resultKey = $"{selectedModels}_{classificationName}";

                            // 모델별 분류 폴더 생성
                            string modelClassFolder = Path.Combine(classifiedFolder, selectedModels, classificationName);
                            if (!Directory.Exists(modelClassFolder))
                            {
                                Directory.CreateDirectory(modelClassFolder);
                            }

                            // 분류 결과를 이미지에 표시
                            Mat annotatedImage = AddClassificationLabel(image, classificationName, selectedModels, classification.Confidence);

                            // 상대 경로 유지
                            string relativePath = GetRelativePath(sourceFolderPath, Path.GetDirectoryName(imagePath));
                            string outputDir = Path.Combine(modelClassFolder, relativePath);

                            if (!Directory.Exists(outputDir))
                            {
                                Directory.CreateDirectory(outputDir);
                            }

                            string outputPath = Path.Combine(outputDir, Path.GetFileName(imagePath));
                            Cv2.ImWrite(outputPath, annotatedImage);

                            // 분류 결과 저장
                            if (!classificationResults.ContainsKey(resultKey))
                            {
                                classificationResults[resultKey] = new List<string>();
                            }
                            classificationResults[resultKey].Add(outputPath);

                            string confidenceText = classification.Confidence >= 0f
                                ? $" ({classification.Confidence:F3})"
                                : string.Empty;
                            OnLogMessage?.Invoke($"Classification completed: {Path.GetFileName(imagePath)} -> {selectedModels}/{classificationName}{confidenceText}");

                            annotatedImage.Dispose();
                        }
                    }

                    processedImages++;
                    OnProgressUpdate?.Invoke(processedImages, imageFiles.Length);
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Classification error: {Path.GetFileName(imagePath)} - {ex.Message}");
                }
            }

            return classificationResults;
        }

        /// <summary>
        /// 특정 모델로 이미지 분류
        /// </summary>
        private ClassificationResultInfo ClassifyWithModel(Mat image, string imagePath, string modelName)
        {
            try
            {
                Mat[] arr = { image };

                List<ResultDiceCls> clsResult = _ClsInference(modelName, arr);

                if (clsResult == null || clsResult.Count == 0)
                {
                    throw new InvalidOperationException("No classification result was returned.");
                }

                ResultDiceCls result = clsResult[0];
                string className = string.IsNullOrWhiteSpace(result.class_name)
                    ? $"CLASS_{result.pred}"
                    : result.class_name;
                float confidence = GetClassificationConfidence(result);
                float minConfidence = GetClassificationMinConfidence();

                if (confidence < minConfidence)
                {
                    OnLogMessage?.Invoke(
                        $"Low-confidence classification: {Path.GetFileName(imagePath)} -> {className} ({confidence:F3} < {minConfidence:F3})");

                    return new ClassificationResultInfo
                    {
                        ClassName = "LOW_CONFIDENCE",
                        Confidence = confidence
                    };
                }

                return new ClassificationResultInfo
                {
                    ClassName = className,
                    Confidence = confidence
                };
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Classification model execution error ({modelName}): {ex.Message}");
                return new ClassificationResultInfo
                {
                    ClassName = "ERROR",
                    Confidence = 0f
                };
            }
        }

        private float GetClassificationConfidence(ResultDiceCls result)
        {
            if (result == null || result.conf == null || result.conf.Count == 0)
            {
                return 1f;
            }

            if (result.pred >= 0 && result.pred < result.conf.Count)
            {
                return NormalizeConfidence(result.conf[result.pred]);
            }

            return NormalizeConfidence(result.conf.Max());
        }

        private float GetClassificationMinConfidence()
        {
            if (_config == null || _config.Classification == null)
            {
                return 0.5f;
            }

            return NormalizeConfidence(_config.Classification.MinConfidence);
        }

        private float NormalizeConfidence(float confidence)
        {
            if (float.IsNaN(confidence) || float.IsInfinity(confidence))
            {
                return 0f;
            }

            if (confidence < 0f)
            {
                return 0f;
            }

            if (confidence > 1f)
            {
                return 1f;
            }

            return confidence;
        }

        /// <summary>
        /// 이미지에 분류 결과 라벨 추가
        /// </summary>
        private Mat AddClassificationLabel(Mat image, string classification, string modelName, float confidence)
        {
            Mat result = image.Clone();

            string confidenceText = confidence >= 0f ? $" ({confidence:F2})" : string.Empty;
            string labelText = $"{modelName}: {classification}{confidenceText}";
            int fontFace = (int)HersheyFonts.HersheySimplex;
            double fontScale = 1.2;
            int thickness = 2;

            // 색상 설정
            Scalar textColor;
            if (classification.Equals("OK", StringComparison.OrdinalIgnoreCase))
            {
                textColor = new Scalar(0, 255, 0); // 초록색
            }
            else if (classification.Equals("NG", StringComparison.OrdinalIgnoreCase))
            {
                textColor = new Scalar(0, 0, 255); // 빨간색
            }
            else if (classification.Equals("LOW_CONFIDENCE", StringComparison.OrdinalIgnoreCase))
            {
                textColor = new Scalar(0, 215, 255); // 노란색
            }
            else
            {
                textColor = new Scalar(255, 0, 0); // 파란색
            }

            // 텍스트 크기 계산
            Size textSize = Cv2.GetTextSize(labelText, (HersheyFonts)fontFace, fontScale, thickness, out int baseline);

            // 배경 사각형
            int padding = 10;
            Point topLeft = new Point(padding, padding);
            Point bottomRight = new Point(padding + textSize.Width + padding,
                padding + textSize.Height + baseline + padding);

            Cv2.Rectangle(result, topLeft, bottomRight, new Scalar(255, 255, 255), -1);
            Cv2.Rectangle(result, topLeft, bottomRight, textColor, 2);

            // 텍스트 그리기
            Point textOrigin = new Point(padding + padding / 2, padding + textSize.Height + padding / 2);
            Cv2.PutText(result, labelText, textOrigin, (HersheyFonts)fontFace,
                fontScale, textColor, thickness, LineTypes.AntiAlias);

            return result;
        }

        /// <summary>
        /// 크롭 영역 조정
        /// </summary>
        private Rect AdjustCropArea(Rect cropArea, OpenCvSharp.Size imageSize)
        {
            int x = Math.Max(0, Math.Min(cropArea.X, imageSize.Width - 1));
            int y = Math.Max(0, Math.Min(cropArea.Y, imageSize.Height - 1));
            int width = Math.Min(cropArea.Width, imageSize.Width - x);
            int height = Math.Min(cropArea.Height, imageSize.Height - y);

            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// 상대 경로 가져오기
        /// </summary>
        private string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
                return string.Empty;

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
                return toPath;

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }

        private string[] GetImageFiles(string folderPath)
        {
            string[] extensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };
            return extensions.SelectMany(ext =>
                Directory.GetFiles(folderPath, ext, SearchOption.AllDirectories)
            ).ToArray();
        }

        public static List<ResultDiceCls> _ClsInference(string model, Mat[] imgs)
        {
            switch (model)
            {
                case nameof(DiceManager.ClassifyModel_A):
                    return DiceManager.ClassifyModel_A.Inference(imgs.ToArray());

                case nameof(DiceManager.ClassifyModel_B):
                    return DiceManager.ClassifyModel_B.Inference(imgs.ToArray());
                default:
                    return new List<ResultDiceCls>();
            }
        }

        public void ClsTest(string testsetDir, string model)
        {
            if (!DiceManager.IsLoad(model)) return;

            List<string> subDirs = new List<string>();
            if (!Utils.CheckValidDatasetDirectory(testsetDir, ref subDirs))
                return;

            foreach (var dir in subDirs)
            {
                string[] files = Directory.GetFiles(dir);
                int totalCount = files.Count();

                if (totalCount <= 0) continue;

                string labeledClass = dir.Split(Path.DirectorySeparatorChar).Last();

                var images = new List<Mat>();
                var filePaths = new List<string>();
                Utils.LoadFilePathsAndImages(dir, ref filePaths, ref images);
                if (filePaths.Count() != images.Count())
                {
                    Console.WriteLine($@"{labeledClass}_CountErr : filePath({filePaths.Count()}) != img({images.Count()})");
                    continue;
                }
                List<ResultDiceCls> diceResults = _ClsInference(model, images.ToArray());
                if (diceResults.Count() != images.Count())
                {
                    Console.WriteLine($@"{labeledClass}_CountErr : dice({diceResults.Count()}) != img({images.Count()})");
                    continue;
                }

                int resultIndex = 0;
                foreach (var result in diceResults)
                {
                    var srcImg = images[resultIndex].Clone();
                    var resImg = BasicAlgoDisplay.MakeClsResultImage(ref srcImg, result, model, labeledClass);

                    string filePath = filePaths[resultIndex];
                    string resultFilePath = filePath.Replace(testsetDir, testsetDir + "_result");
                    Utils.DeleteDirectory(Path.GetDirectoryName(resultFilePath));
                    Utils.CreateDirectory(Path.GetDirectoryName(resultFilePath));
                    Utils.SaveImageFile(resultFilePath, resImg);

                    resultIndex++;
                }
                Console.WriteLine($@"number of files: {totalCount}");
            }
            Utils.WriteTail(model);
        }
    }
}
