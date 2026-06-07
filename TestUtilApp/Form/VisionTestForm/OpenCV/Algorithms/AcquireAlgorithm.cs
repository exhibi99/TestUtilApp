using System.IO;
using System.Windows.Forms;
using OpenCvSharp;

namespace TestUtilApp.UI
{
    /// <summary>
    /// Source node that represents the "Open Image" step.
    /// Holds a reference to the loaded Mat; Execute() returns a clone.
    /// </summary>
    public class AcquireAlgorithm : IOpenCvAlgorithm
    {
        private Mat _source;

        public string FilePath  { get; private set; }
        public string Name      => "Open Image";
        public string Summary   => FilePath != null ? Path.GetFileName(FilePath) : "(no image)";
        public int    InputFromStep { get; set; } = -1;
        public bool   IsSourceNode  => true;

        /// <summary>
        /// Stores a reference to the Mat (not owned — caller disposes).
        /// </summary>
        public void SetSource(Mat mat, string filePath)
        {
            _source   = mat;
            FilePath  = filePath;
        }

        public Mat Execute(Mat input)
            => _source != null && !_source.Empty() ? _source.Clone() : null;

        public bool ShowConfigDialog(IWin32Window owner) => false;
        public AlgorithmSettingsPanel GetSettingsPanel()  => null;
    }
}
