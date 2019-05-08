using System;
using System.Windows.Controls;
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
            CurSf.GroupName = $"Mode{GetHashCode()}";
            AllSf.GroupName = $"Mode{GetHashCode()}";
            CurSf.IsChecked = true;
        }

        public GraphAlgorithmRenderer.Config.Condition Condition
        {
            get
            {
                var mode = GetMode();
                return new GraphAlgorithmRenderer.Config.Condition(ConditionBox.Text, RegexBox.Text, mode);
            }
        }

        public ConditionMode GetMode()
        {
            return  AllSf.IsChecked == true
                ? ConditionMode.AllStackFrames
                : ConditionMode.CurrentStackFrame;
        }


        public string Description =>
            string.Join(" ", ConditionBox.Text, RegexBox.Text,
                Enum.GetName(typeof(ConditionMode), GetMode()));

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