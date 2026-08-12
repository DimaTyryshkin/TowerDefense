using System.Globalization;

namespace Game.CoreGame.Gui
{
    static class GuiFormat
    {
        public static string FormatToInt(float value) => string.Format(CultureInfo.InvariantCulture, "{0:N0}", value);
    }
}