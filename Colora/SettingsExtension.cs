using System;
using System.Windows.Data;

namespace Colora
{
    public class SettingsExtension : Binding
    {
        public SettingsExtension()
        {
            init();
        }

        public SettingsExtension(string path) : base(path)
        {
            init();
        }

        private void init()
        {
            this.Source = Properties.Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}
