using OpenCvSharp;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    public interface IOpenCvAlgorithm
    {
        string Name    { get; }
        string Summary { get; }

        /// <summary>
        /// 0-based index of the pipeline step whose output this step uses as input.
        /// -1 means "use the immediately preceding step" (default sequential behaviour).
        /// </summary>
        int InputFromStep { get; set; }

        /// <summary>True for source nodes (e.g. Acquire) that produce output without consuming input.</summary>
        bool IsSourceNode { get; }

        Mat Execute(Mat input);
        bool ShowConfigDialog(IWin32Window owner);

        /// <summary>
        /// Returns a new inline settings panel for this algorithm.
        /// The caller is responsible for disposing the returned panel.
        /// </summary>
        AlgorithmSettingsPanel GetSettingsPanel();
    }
}
