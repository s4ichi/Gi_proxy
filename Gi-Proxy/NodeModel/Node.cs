using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.PowerPacks;
using System.Drawing;

namespace Gi_Proxy
{
    /// <summary>
    /// 各ノードの情報を表すクラス
    /// </summary>
    public class Node
    {
        //ノードの名前
        public string NodeName { get; private set; }
        //ノードの質量
        public double NodeMass { get; private set; }

        //有向グラフ(グラフィック用)
        public ICollection<Edge> ToEdge { get; private set; }

        //無向グラフ(処理用)
        public ICollection<Node> LinkNode { get; private set; }

        //ノードの座標
        public Pair Coordinate;

        //ノードの円Object
        public OvalShape NodeShape = new OvalShape();

        //ノードの円Objectの色
        public SolidBrush NodeColor { get; set; }

        //ノードの速度
        public Pair Speed { get; set; }

        //ノードの指すドメインへの最終アクセス日時
        public string LastAccessTime;

        //ノードをロックしているかどうか
        public bool LockFlg = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Name">ノード名</param>
        /// <param name="Mass">ノード質量</param>
        /// <param name="X">ノードの初期X座標</param>
        /// <param name="Y">ノードの初期Y座標</param>
        public Node(string Name, double Mass, double X, double Y, int Seed)
        {
            this.NodeName = Name;
            this.NodeMass = Mass;
            this.Coordinate = new Pair(X, Y);
            this.NodeShape.Name = Name;
            this.ToEdge = new List<Edge>();
            this.LinkNode = new List<Node>();
            this.Speed = new Pair(0.0d, 0.0d);
            this.LastAccessTime = DateTime.Now.ToString();
            //乱数
            Random Rand = new Random(Seed);
            int R = Rand.Next(0, 255), G = Rand.Next(0, 255), B = Rand.Next(0, 255);
            this.NodeColor = new SolidBrush(Color.FromArgb(R, G, B));
        }

        //ばね定数
        private const double K = 0.1d;
        //ばねの元の長さ
        public double L = 100.0d;
        //斥力
        public double G = 1000.0d;
        //摩擦力
        private const double M = 0.8d;

        /// <summary>
        /// ばねによってかかる力の大きさを求める(フックの法則)
        /// </summary>
        /// <param name="n">繋がっているノード</param>
        /// <returns>そのノードのx方向とy方向にかかる力の大きさ</returns>
        public Pair GetSpringForce(Node n)
        {
            double dx = this.Coordinate.X - n.Coordinate.X;
            double dy = this.Coordinate.Y - n.Coordinate.Y;
            double d2 = dx * dx + dy * dy;

            if (d2 < double.Epsilon)
            {
                Random Rand = new Random();
                return new Pair()
                {
                    X = Rand.NextDouble() - 0.99d,
                    Y = Rand.NextDouble() - 0.99d
                };
            }

            double d = Math.Sqrt(d2);
            double cos = dx / d;
            double sin = dy / d;
            double dl = d - L;

            return new Pair()
            {
                X = -K * dl * cos,
                Y = -K * dl * sin
            };
        }
        
        /// <summary>
        /// ノード間の斥力(クーロン力)
        /// </summary>
        /// <param name="n">対象のノード</param>
        /// <returns>そのノードのx方向とy方向にかかる力の大きさ</returns>
        public Pair GetREplusiveForce(Node n)
        {
            double dx = this.Coordinate.X - n.Coordinate.X;
            double dy = this.Coordinate.Y - n.Coordinate.Y;
            double d2 = dx * dx + dy * dy;

            if (d2 < double.Epsilon)
            {
                Random Rand = new Random();
                return new Pair()
                {
                    X = Rand.NextDouble() - 0.99d,
                    Y = Rand.NextDouble() - 0.99d
                };
            }

            double d = Math.Sqrt(d2);
            double cos = dx / d;
            double sin = dy / d;

            return new Pair()
            {
                X = G / d2 * cos,
                Y = G / d2 * sin
            };
        }

        /// <summary>
        /// ノードにかかる摩擦力
        /// </summary>
        /// <returns>そのノードのx方向とy方向にかかる力の大きさ</returns>
        public Pair GetFrictionalForce()
        {
            return new Pair()
            {
                X = -M * this.NodeMass * this.Speed.X,
                Y = -M * this.NodeMass * this.Speed.Y
            };
        }

        /// <summary>
        /// 力が加わった場合のノードの座標と速度を求める
        /// </summary>
        /// <param name="DeltaT">微小時間,時間の変化量</param>
        /// <param name="f">ノードに伝わった力の合計</param>
        public void MoveNode(double DeltaT, Pair f)
        {
            this.Speed = new Pair()
            {
                X = this.Speed.X + DeltaT * f.X / this.NodeMass,
                Y = this.Speed.Y + DeltaT * f.Y / this.NodeMass
            };

            this.Coordinate = new Pair()
            {
                X = this.Coordinate.X + DeltaT * this.Speed.X,
                Y = this.Coordinate.Y + DeltaT * this.Speed.Y
            };
        }
    }
}
