using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Colora.Model;

namespace Colora.View
{
    public partial class SettingsWindow : Window
    {
        ScreenPicker screenPicker;

        HashSet<BindingExpressionBase> beSet;

        public SettingsWindow(ScreenPicker screenPicker)
        {
            this.screenPicker = screenPicker;
            InitializeComponent();
            beSet = new HashSet<BindingExpressionBase>();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            beSet.UnionWith(BindingOperations.GetSourceUpdatingBindings(this));
            foreach (var be in beSet)
            {
                be.UpdateSource();
            }
            this.DialogResult = true;
            this.Close();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            beSet.UnionWith(BindingOperations.GetSourceUpdatingBindings(this));
        }

        private void ChangeShortcutButton_Click(object sender, RoutedEventArgs e)
        {
            App.ConfigureShortcut(this, this.screenPicker);
        }
    }
}
