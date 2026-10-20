using UnityEngine;

namespace IncreKindom
{
    public static class GameMath
    {
        public static int SortingOrderFromY(float y) => -Mathf.RoundToInt((y) * 100);

    }
}
