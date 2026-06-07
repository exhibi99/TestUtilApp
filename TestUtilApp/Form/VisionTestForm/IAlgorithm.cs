using OpenCvSharp;

namespace TestUtilApp.UI
{
    public interface IAlgorithm
    {
        string Name    { get; }
        string Summary { get; }
        Mat Apply(Mat input);
    }
}
