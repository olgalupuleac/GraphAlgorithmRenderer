using System.Diagnostics;

namespace GraphAlgorithmRenderer
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for GraphAlgorithmRendererControl.
    /// </summary>
    public partial class GraphAlgorithmRendererControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphAlgorithmRendererControl"/> class.
        /// </summary>
        public GraphAlgorithmRendererControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string JsonConfig { get; set; }
    }
}