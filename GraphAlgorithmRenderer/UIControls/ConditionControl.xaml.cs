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
            var groupName = $"Mode{GetHashCode()}";
            CurSf.GroupName = groupName;
            AllSf.GroupName = groupName;
            AllSfArgs.GroupName = groupName;
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
            if (AllSf.IsChecked == true)
            {
                return ConditionMode.AllStackFrames;
            }

            if (AllSfArgs.IsChecked == true)
            {
                return ConditionMode.AllStackFramesArgsOnly;
            }

            return ConditionMode.CurrentStackFrame;
        }


        public string Description =>
            string.Join(" ", ConditionBox.Text, RegexBox.Text,
                Enum.GetName(typeof(ConditionMode), GetMode()));

        public void FromCondition(GraphAlgorithmRenderer.Config.Condition condition)
        {
            ConditionBox.Text = condition.Template;
            RegexBox.Text = condition.FunctionNameRegex;
            CurSf.IsChecked = true;
            switch (condition.Mode)
            {
                case ConditionMode.AllStackFrames:
                    AllSf.IsChecked = true;
                    break;
                case ConditionMode.AllStackFramesArgsOnly:
                    AllSfArgs.IsChecked = true;
                    break;
                case ConditionMode.CurrentStackFrame:
                    CurSf.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}