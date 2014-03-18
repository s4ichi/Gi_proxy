using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gi_Proxy
{
    //Dictionary(key:Nodename,contents:<Node>)を継承したNodePartyクラス
    //インスタンスは1つのグラフの情報を持つ
    public class NodeParty : Dictionary<string, Node>
    {
        /// <summary>
        /// 初期ノード(localhostを追加)
        /// </summary>
        public void InitNode(string Name, double Mass, double X, double Y)
        {
            Random R = new Random();
            this[Name] = new Node(Name, Mass, X, Y, R.Next());
        }

        /// <summary>
        /// ノードを追加するクラス
        /// </summary>
        /// <param name="Ri">ノードの内容が書き込まれているクラス</param>
        public bool AddNode(RequestInfo Ri)
        {
            try
            {
                string ByNodeName = "localhost";
                string ToNodeName = Ri.HostName;
                //もし送信先が指定されているんだったらそっちに置き換える
                foreach (string s in Ri.Data)
                {
                    if (s.IndexOf("Referer") > -1)
                    {
                        ByNodeName = s.Split(' ')[1].Replace("http://", "").Split('/')[0];
                    }
                }

                if (!this.ContainsKey(ByNodeName))
                {
                    ByNodeName = "localhost";
                }

                //もし既に登録されているんだったらリンクするのみ
                bool AddFlg = false;
                lock (this)
                {
                    if (!this.ContainsKey(ToNodeName))
                    {
                        //座標を乱数で調整
                        //座標が重複したら演算出来ないので被らないまでループ
                        Random Rand = new Random();
                        double X,Y;
                        while (true)
                        {
                            X = this[ByNodeName].Coordinate.X + ((Rand.NextDouble() - Rand.NextDouble()) * 100);
                            Y = this[ByNodeName].Coordinate.Y + ((Rand.NextDouble() - Rand.NextDouble()) * 100);
                            if (AssertNodePos(X, Y)) break;
                        }
                        this[ToNodeName] = new Node(ToNodeName, /*NewNodeMass*/1.0d, X, Y, Rand.Next());
                        AddFlg = true;
                    }
                    if (!this[ByNodeName].LinkNode.Contains(this[ToNodeName]) && !this[ToNodeName].LinkNode.Contains(this[ByNodeName]) && ByNodeName != ToNodeName)
                    {
                        LinkNode(ByNodeName, ToNodeName);
                    }
                }
                return AddFlg;
            }
            catch (Exception E)
            {
                Console.WriteLine("追加出来ませんでした。\n" + E);
                return false;
            }
        }

        /// <summary>
        /// ノードの座標が他のノードと重なっていないか検証
        /// </summary>
        private bool AssertNodePos(double X,double Y)
        {
            lock (this)
            {
                foreach (Node n in this.Values)
                {
                    if (n.Coordinate.X == X && n.Coordinate.Y == Y)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// ノードの連結
        /// </summary>
        public void LinkNode(string Dear,string To)
        {
            //無向
            this[Dear].ToEdge.Add(new Edge(this[To]));
            this[Dear].LinkNode.Add(this[To]);
            //有向
            this[To].LinkNode.Add(this[Dear]);
        }

        /// <summary>
        /// ノードの位置を更新
        /// </summary>
        public void MoveAll()
        {
            //1ms毎にD回グラフィックの処理行う
            lock (this)
            {
                const int D = 60;
                for (int i = 0; i < D; i++)
                {
                    const double DeltaT = 0.1d;
                    foreach (Node n in this.Values)
                    {
                        if (n.LockFlg) continue;
                        Pair F = new Pair();
                        foreach (Node nn in n.LinkNode)
                        {
                            F += n.GetSpringForce(nn);
                        }
                        foreach (Node nn in this.Values)
                        {
                            if (n != nn)
                            {
                                F += n.GetREplusiveForce(nn);
                            }
                        }
                        F += n.GetFrictionalForce();
                        n.MoveNode(DeltaT, F);
                    }
                }
            }
        }
        
        /// <summary>
        /// 置き換えの座標が収束するまで処理を繰り返す
        /// デバッグ用
        /// </summary>
        public void MoveAll_2()
        {
            const double DeltaT = 0.1d;
            const double EndParam = 0.001d;
            Pair MeasurementValue;
            lock (this)
            {
                do
                {
                    MeasurementValue = new Pair(0.0d, 0.0d);
                    foreach (Node n in this.Values)
                    {
                        if (n.LockFlg) continue;
                        Pair F = new Pair();
                        foreach (Node nn in n.LinkNode)
                        {
                            F += n.GetSpringForce(nn);
                        }
                        foreach (Node nn in this.Values)
                        {
                            if (n != nn)
                            {
                                F += n.GetREplusiveForce(nn);
                            }
                        }
                        F += n.GetFrictionalForce();
                        n.MoveNode(DeltaT, F);
                        MeasurementValue += F;
                    }

                } while ((System.Math.Abs(MeasurementValue.X) + System.Math.Abs(MeasurementValue.Y)) * 10000 >= EndParam);
            }
        }
    }
}
