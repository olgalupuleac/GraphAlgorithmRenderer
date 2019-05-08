using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Condition = System.Windows.Condition;

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
            CurSf.GroupName = $"Mode{GetHashCode()}";
            AllSf.GroupName = $"Mode{GetHashCode()}";
            CurSf.IsChecked = true;
        }

        public GraphAlgorithmRenderer.Config.Condition Condition
        {
            get
            {
                ConditionMode mode = AllSf.IsChecked == true
                    ? ConditionMode.AllStackFrames
                    : ConditionMode.CurrentStackFrame;
                return new GraphAlgorithmRenderer.Config.Condition(ConditionBox.Text, RegexBox.Text, mode);
            }
        }

        public void FromCondition(GraphAlgorithmRenderer.Config.Condition condition)
        {
            ConditionBox.Text = condition.Template;
            RegexBox.Text = condition.FunctionNameRegex;
            CurSf.IsChecked = true;
            if (condition.Mode == ConditionMode.AllStackFrames)
            {
                AllSf.IsChecked = true;
            }
            else
            {
                CurSf.IsChecked = true;
            }
        }
    }
}