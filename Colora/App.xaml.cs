using System;
using System.Windows;

namespace Colora
{
    public partial class App : Application
    {
        public const string UPDATE_URL = "https://colora.sourceforge.io/update.xml";

#if PORTABLE
        public const string UPDATE_MODE = "portable";
#else
        public const string UPDATE_MODE = "install";
#endif
    }
}
