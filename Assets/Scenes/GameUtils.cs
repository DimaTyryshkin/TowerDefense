using System.Globalization;
using UnityEngine;

namespace IncreGame
{
    public static class GameUtils
    {
        public static int SortingOrderFromY(float y) => -Mathf.RoundToInt(y * 100);
        public static int SortingOrderFromMinY(SpriteRenderer s) => SortingOrderFromY(s.bounds.min.y);

        public static string AddCoinIcon(string originText) => "<sprite name=\"coin\"> " + originText;
        public static string AddCoinIcon(string originText, Color iconColor)
        {
            return $"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGBA(iconColor)}><sprite name=\"coin\" tint=1></color> {originText}";
        }

        /// <summary>
        /// int to 1,000,000
        /// </summary>
        public static string IntToSting(int number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// flaot to 1,000,000
        /// </summary>
        public static string FlaotToSting(int number)
        {
            return number.ToString("N2", CultureInfo.InvariantCulture);
        }
    }
}
