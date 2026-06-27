

using UnityEngine;

namespace Game.CoreGame
{
    public struct Side
    {
        /// <summary>
        /// 0-верх 1-влево 2-вниз 3-вправо
        /// </summary>
        public int side;

        public Side(int side)
        {
            this.side = side;
        }



        public static Side FromDir(Vector2 dir)
        {
            return FromSignedAngle(Vector2.SignedAngle(Vector2.up, dir));
        }

        /// <summary> Вернет 0-верх 1-влево 2-вниз 3-вправо </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public static Side FromSignedAngle(float signedAngle)
        {
            if (signedAngle < 0)
                signedAngle += 360;

            return signedAngle switch
            {
                < 360 * 1 / 8f or >= 360 * 7 / 8f => new Side(0),
                < 360 * 3 / 8f and >= 360 * 1 / 8f => new Side(1),
                < 360 * 5 / 8f and >= 360 * 3 / 8f => new Side(2),
                < 360 * 7 / 8f and >= 360 * 5 / 8f => new Side(3),
                _ => throw new System.NotImplementedException()
            };
        }
    }
}
