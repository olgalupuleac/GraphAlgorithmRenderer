﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GraphAlgorithmRenderer.Config;
using static System.String;

namespace GraphAlgorithmRenderer.UIControls
{
    /// <summary>
    /// Interaction logic for EdgeFamilyWindow.xaml
    /// </summary>
    public partial class EdgeFamilyWindow : Window
    { 
        private readonly Dictionary<string, NodeFamilyWindow> _availableNodes;
        private EdgeEndControl _targetWindow;
        private EdgeEndControl _sourceWindow;
       

        public EdgeFamilyWindow(Dictionary<string, NodeFamilyWindow> availableNodes)
        {
            _availableNodes = availableNodes;
            
            InitializeComponent();
       
            PropertiesControl.WindowGenerator = () => new EdgeConditionalPropertyWindow();
            PropertiesControl.Description = w => ((EdgeConditionalPropertyWindow) w).ConditionControl.Description;
            PropertiesControl.UpdateDescription = (w, i) =>
            {
                ((EdgeConditionalPropertyWindow) w).ok.Click += (o, sender) =>
                        i.Content = ((EdgeConditionalPropertyWindow) w).ConditionControl.Description;
               
            };
            foreach (var node in _availableNodes.Keys)
            {

                var targetRadioButton = new RadioButton { Content = node, GroupName = $"TargetNodes{GetHashCode()}" };
                targetRadioButton.Checked += (sender, args) =>
                    _targetWindow = new EdgeEndControl(_availableNodes[node].Ranges.Where(id => !IsNullOrEmpty(id.Name)).Select(id => id.Name).ToList(), node);
                TargetPanel.Children.Add(targetRadioButton);
                var sourceRadioButton = new RadioButton { Content = node, GroupName = $"SourceNodes{GetHashCode()}" };
                sourceRadioButton.Checked += (sender, args) =>
                    _sourceWindow = new EdgeEndControl(_availableNodes[node].Ranges.Where(id => !IsNullOrEmpty(id.Name)).Select(id => id.Name).ToList(), node);
                SourcePanel.Children.Add(sourceRadioButton);
            }
        }
      

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;    
            Hide(); 
        }

        private void TargetButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEdgeEnd(_targetWindow, "Target");
        }

        private void ShowEdgeEnd(EdgeEndControl window, string type)
        {
            if (window == null)
            {
                MessageBox.Show($"{type} node family is not defined", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            window.Show();
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            ShowEdgeEnd(_sourceWindow, "Source");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            foreach (var range in IdentifierPartRangeControl.Ranges)
            {
                if (IsNullOrEmpty(range.Name) ||
                    !IsNullOrEmpty(range.BeginTemplate) && !IsNullOrEmpty(range.EndTemplate)) continue;
                MessageBox.Show($"Identifier range {range.Name} is not finished", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_sourceWindow == null)
            {
                MessageBox.Show("Source node is not specified", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_targetWindow == null)
            {
                MessageBox.Show("Target node is not specified", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Hide();
        }

      

        public EdgeFamily EdgeFamily
        {
            get
            {
                var conditionalProperties = PropertiesControl.Windows.Cast<EdgeConditionalPropertyWindow>()
                    .Select(w => w.ConditionalProperty).ToList();
                conditionalProperties.Reverse();
                //TODO check for null
                return new EdgeFamily(IdentifierPartRangeControl.Ranges.ToList(), _sourceWindow.EdgeEnd, _targetWindow.EdgeEnd,
                    directed.IsChecked == true)
                {
                    ValidationTemplate = validationTemplateBox.Text,
                    ConditionalProperties = conditionalProperties
                };
            }
        }

        private void SetNodeFamilies(string target, string source)
        {
            Debug.WriteLine($"target {target}");
            foreach (var child in TargetPanel.Children)
            {
                if (child is RadioButton rb)
                {
                    Debug.WriteLine($"{rb.Content}");
                    if ((string) rb.Content != target)
                    {
                        continue;
                    }
                    rb.IsChecked = true;
                }
            }

            foreach (var child in SourcePanel.Children)
            {
                if (child is RadioButton rb && (string)rb.Content == source)
                {
                    rb.IsChecked = true;
                }
            }
        }

        public void FromEdgeFamily(EdgeFamily edgeFamily, List<string> nodeNames)
        {
            IdentifierPartRangeControl.FromRanges(edgeFamily.Ranges);
            directed.IsChecked = edgeFamily.IsDirected;
            validationTemplateBox.Text = edgeFamily.ValidationTemplate;
            _targetWindow = new EdgeEndControl(edgeFamily.Target);
            _sourceWindow = new EdgeEndControl(edgeFamily.Source);
            SetNodeFamilies(edgeFamily.Target.NodeFamilyName, edgeFamily.Source.NodeFamilyName);
            var conditionalProperties = ((IEnumerable<ConditionalProperty<IEdgeProperty>>)edgeFamily.ConditionalProperties)
                .Reverse().ToList();
            var windows = new List<Window>();
            foreach (var conditionalProperty in conditionalProperties)
            {
               var window = new EdgeConditionalPropertyWindow();
               window.FromConditionalProperty(conditionalProperty);
               windows.Add(window);
            }
            PropertiesControl.SetNewWindows(windows);
        }
    }
}