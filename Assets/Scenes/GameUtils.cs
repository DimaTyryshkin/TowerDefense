using UnityEngine;

namespace IncreKindom
{
    public static class GameUtils
    {
        public static int SortingOrderFromY(float y) => -Mathf.RoundToInt(y * 100);

        public static string AddCoinIcon(string originText) => "<sprite name=\"coin\"> " + originText;
        public static string AddCoinIcon(string originText, Color iconColor)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(iconColor)}><sprite name=\"coin\" tint=1></color> {originText}";
        }
    }
}
