using System;
using System.Windows.Forms;

namespace TestUtilApp.UI
{
    /// <summary>
    /// Abstract base class for inline algorithm settings panels.
    /// Each concrete algorithm provides its own panel via IOpenCvAlgorithm.GetSettingsPanel().
    /// </summary>
    public abstract class AlgorithmSettingsPanel : UserControl
    {
        public event EventHandler Applied;

        protected void RaiseApplied()
        {
            Applied?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Loads current algorithm parameters into this panel's controls.</summary>
        public abstract void LoadFrom(IOpenCvAlgorithm algorithm);

        /// <summary>Writes this panel's control values back to the algorithm.</summary>
        public abstract void ApplyTo(IOpenCvAlgorithm algorithm);
    }
}
