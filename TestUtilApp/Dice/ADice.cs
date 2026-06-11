using DICE_Library;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TestUtilApp.Dice
{
    abstract class ADice
    {
        public readonly string Name;
        public readonly double retryCount = 10;
        protected string ProjectPath;
        protected DICE_Model Model;

        public bool IsLoaded { get; private set; }

        public ADice(string name)
        {
            Name = name;
            ProjectPath = "";
            IsLoaded = false;
        }

        public bool LoadModel(string projectPath, string diceVersion = "v2")
        {
            ProjectPath = projectPath;
            try
            {
                bool isV1 = string.Equals(diceVersion, "v1", StringComparison.OrdinalIgnoreCase);

                if (isV1)
                {
                    // v1: getModel(ProjectPath) - 소문자, 1개 인자
                    var method = typeof(DICE_Interface).GetMethod("getModel",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                        null,
                        new[] { typeof(string) },
                        null);

                    if (method != null)
                    {
                        Model = (DICE_Model)method.Invoke(null, new object[] { ProjectPath });
                    }
                }
                else
                {
                    // v2: GetModel(ProjectPath, Device("cuda","cpu") - 대문자, 2개 인자
                    var method = typeof(DICE_Interface).GetMethod("GetModel",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                        null,
                        new[] { typeof(string), typeof(string) },
                        null);

                    if (method != null)
                    {
                        Model = (DICE_Model)method.Invoke(null, new object[] { ProjectPath, "cuda" });
                    }
                }

                return IsLoaded = (Model != null);
            }
            catch (Exception e)
            {
                try
                {
                    // v2: GetModel(ProjectPath, Device("cuda","cpu") - 대문자, 2개 인자
                    var method = typeof(DICE_Interface).GetMethod("GetModel",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                        null,
                        new[] { typeof(string), typeof(string) },
                        null);

                    if (method != null)
                    {
                        Model = (DICE_Model)method.Invoke(null, new object[] { ProjectPath, "cpu" });
                    }

                    return IsLoaded = (Model != null);
                }
                catch
                {
                    string msg = e.Message;
                    return IsLoaded = false;
                }
            }

        }

        public void UnloadModel()
        {
            IsLoaded = false;
            Model = null;  // Release the Model object reference so DLL can be unloaded
        }

        protected virtual string _Inference(Mat[] imgs) //다중 이미지 inference
        {
            if (IsLoaded == false)
            {
                return "error";
            }
            try
            {
                int w = int.MaxValue;
                int h = int.MaxValue;
                int c = int.MaxValue;
                List<byte[]> listData = new List<byte[]>();
                foreach (Mat img in imgs)
                {
                    if (img.IsDisposed)
                        continue;
                    w = (img.Width < w) ? img.Width : w;
                    h = (img.Height < h) ? img.Height : h;
                    c = (img.Channels() < c) ? img.Channels() : c;
                }

                foreach (Mat img in imgs)
                {
                    if (img.IsDisposed)
                        continue;
                    Size size = new Size(w, h);
                    Point point = new Point((img.Width - w) / 2, (img.Height - h) / 2);
                    Rect rect = new Rect(point, size);
                    Mat roiImg = img[rect].Clone();
                    int length = w * h * c;
                    byte[] data = new byte[length];
                    Marshal.Copy(roiImg.Data, data, 0, length);
                    listData.Add(data);
                }
                var vData = listData.ToArray();
                string res = Model.inference(vData, w, h, c);

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("ADice Error: " + e.Message);
                return "error";
            }
        }
    }
}
