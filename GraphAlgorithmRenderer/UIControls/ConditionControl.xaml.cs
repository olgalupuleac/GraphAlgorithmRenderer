using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphAlgorithmRenderer.Config;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for ConditionControl.xaml
    /// </summary>
    public partial class ConditionControl : UserControl
    {
        public ConditionControl()
        {
            InitializeComponent();
        }

        public void FromCondition(GraphAlgorithmRenderer.Config.Condition condition)
        {
            ConditionBox.Text = condition.Template;
            RegexBox.Text = condition.FunctionNameRegex;

            if (condition.Mode == ConditionMode.AllStackFrames)
            {
                AllSf.IsChecked = true;
            }
            else
            {
                CurSf.IsChecked = true;
            }
        }

        public void Reset()
        {
            ConditionBox.Text = "true";
            CurSf.IsChecked = true;
            AllSf.IsChecked = false;
            RegexBox.Text = ".*";
        }
    }
}