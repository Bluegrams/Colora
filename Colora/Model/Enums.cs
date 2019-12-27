using System.ComponentModel;

namespace Colora.Model
{
    public enum ColorSamplingMode
    {
        [Description("")]
        Single = 0,
        [Description("3x3")]
        Average3x3 = 1,
        [Description("5x5")]
        Average5x5 = 2
    }
}
