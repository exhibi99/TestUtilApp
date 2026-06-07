using OpenCvSharp;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    public interface IOpenCvAlgorithm
    {
        string Name    { get; }
        string Summary { get; }
        Mat Execute(Mat input);
        bool ShowConfigDialog(IWin32Window owner);

        /// <summary>
        /// Returns a new inline settings panel for this algorithm.
        /// The caller is responsible for disposing the returned panel.
        /// </summary>
        AlgorithmSettingsPanel GetSettingsPanel();
    }
}
