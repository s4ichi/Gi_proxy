using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gi_Proxy
{
    /// <summary>
    /// X座標とY座標の構造体
    /// </summary>
    public struct Pair
    {
        //X座標
        public double X;
        //Y座標
        public double Y;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Pair(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        //和算した場合に2点の座標を実際に足す
        public static Pair operator +(Pair a, Pair b)
        {
            return new Pair()
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }
    }
}
