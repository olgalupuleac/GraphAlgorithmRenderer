using System.Diagnostics;
using ICSharpCode.AvalonEdit.Highlighting;

namespace GraphAlgorithmRenderer
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SettingsWindowControl.
    /// </summary>
    public partial class SettingsWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindowControl"/> class.
        /// </summary>
        public SettingsWindowControl()
        {
            this.InitializeComponent();
            Config.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
            Config.ShowLineNumbers = true;
            
            this.DataContext = this;
        }

        public string JsonConfig { get; set; }

        
    }
}