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
        private Mat  _source;
        private bool _ownsMat;

        public string FilePath  { get; private set; }
        public string Name      => "Open Image";
        public string Summary   => FilePath != null ? Path.GetFileName(FilePath) : "(no image)";
        public int    InputFromStep { get; set; } = -1;
        public bool   IsSourceNode  => true;
        public bool   IsEnabled     { get; set; } = true;
        public bool   HasSource     => _source != null && !_source.Empty();

        /// <summary>
        /// Stores a reference to the Mat (not owned — caller disposes).
        /// </summary>
        public void SetSource(Mat mat, string filePath)
        {
            DisposeIfOwned();
            _source   = mat;
            FilePath  = filePath;
            _ownsMat  = false;
        }

        /// <summary>
        /// Loads the image from disk; this instance owns and disposes the Mat.
        /// </summary>
        public void LoadSource(string filePath, bool asGray)
        {
            DisposeIfOwned();
            var mode = asGray ? ImreadModes.Grayscale : ImreadModes.Color;
            _source  = Cv2.ImRead(filePath, mode);
            FilePath = filePath;
            _ownsMat = true;
        }

        private void DisposeIfOwned()
        {
            if (_ownsMat && _source != null)
            {
                _source.Dispose();
                _source = null;
            }
            _ownsMat = false;
        }

        ~AcquireAlgorithm() => DisposeIfOwned();

        public Mat Execute(Mat input)
            => _source != null && !_source.Empty() ? _source.Clone() : null;

        public bool ShowConfigDialog(IWin32Window owner) => false;
        public AlgorithmSettingsPanel GetSettingsPanel()  => null;
    }
}
