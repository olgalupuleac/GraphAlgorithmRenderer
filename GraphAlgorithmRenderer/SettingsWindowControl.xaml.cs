using System.Diagnostics;

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
            this.DataContext = this;
        }

        public string JsonConfig { get; set; }

        public bool IsReady { get; set; }


        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(JsonConfig);
            IsReady = true;
        }

        private void Config_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsReady = false;
        }
    }
}