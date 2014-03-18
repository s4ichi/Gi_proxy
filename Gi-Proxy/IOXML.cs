using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;

namespace Gi_Proxy
{
    /// <summary>
    /// グラフ保存をするためのXMLコンバータ
    /// </summary>
    public class IOXML
    {
        public NodeNetwork SampleNodeNet = new NodeNetwork();
        
        Type[] nl = new Type[] { typeof(NodeInfo) };
        Type[] il = new Type[] { typeof(TableItem) };
         
        public ArrayList ItemList = new ArrayList();
        public ArrayList NodeList = new ArrayList();

        /// <summary>
        /// 書き込む
        /// </summary>
        /// <param name="Nnet"></param>
        /// <param name="Path">ローカルパス</param>
        public void writeData(NodeNetwork Nnet, string Path)
        {
            lock (Nnet)
            {
                foreach (Node n in Nnet.Nodes.Values)
                {
                    NodeInfo nf = new NodeInfo();
                    nf.setNodeInfo(n);
                    nf.setToNode(n.ToEdge);
                    NodeList.Add(nf);
                }
            }

            foreach (TableItem t in Nnet.TableItems)
            {
                ItemList.Add(t);
            }

            //こっからXML化
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayList), nl);
            FileStream fs = new FileStream(@Path + @"\Graph.xml", FileMode.Create);
            serializer.Serialize(fs, this.NodeList);
            fs.Close();

            serializer = new XmlSerializer(typeof(ArrayList), il);
            fs = new FileStream(@Path + @"\Table.xml", FileMode.Create);
            serializer.Serialize(fs, this.ItemList);
            fs.Close();
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="Path">ローカルパス</param>
        /// <returns></returns>
        public NodeNetwork readData(string Path)
        {
            //XmlSerializerオブジェクトを作成
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayList), nl);
            //読み込むファイルを開く
            FileStream fs = new FileStream(Path + @"\Graph.xml", System.IO.FileMode.Open);
            //XMLファイルから読み込み、逆シリアル化する
            var obj = (ArrayList)serializer.Deserialize(fs);
            //ファイルを閉じる
            fs.Close();

            //XmlSerializerオブジェクトを作成
            serializer = new XmlSerializer(typeof(ArrayList), il);
            //読み込むファイルを開く
            fs = new FileStream(Path + @"\Table.xml", System.IO.FileMode.Open);
            //XMLファイルから読み込み、逆シリアル化する
            var obj2 = (ArrayList)serializer.Deserialize(fs);
            //ファイルを閉じる
            fs.Close();


            /*読み込んだらコンバート*/
            foreach (TableItem t in obj2)
            {
                SampleNodeNet.TableItems.Add(t);
            }

            Random R = new Random();
            int cnt = R.Next();
            foreach (NodeInfo n in obj)
            {
                Node nn = new Node(n.NodeName, 1.0d, n.X, n.Y,++cnt);
                nn.LastAccessTime = n.LastAccessTime;
                SampleNodeNet.Nodes.Add(n.NodeName, nn);
            }

            foreach (NodeInfo n in obj)
            {
                foreach (string e in n.ToNode)
                {
                    SampleNodeNet.Nodes[n.NodeName].ToEdge.Add(new Edge(SampleNodeNet.Nodes[e]));
                    SampleNodeNet.Nodes[n.NodeName].LinkNode.Add(SampleNodeNet.Nodes[e]);
                    SampleNodeNet.Nodes[e].LinkNode.Add(SampleNodeNet.Nodes[n.NodeName]);
                }
            }
            return SampleNodeNet;
        }
    }

    /// <summary>
    /// XML化するときに保存する項目
    /// </summary>
    public class NodeInfo
    {
        public string NodeName;
        public string LastAccessTime;
        public ArrayList ToNode = new ArrayList();
        public double X, Y;

        public void setNodeInfo(Node n)
        {
            NodeName = n.NodeName;
            LastAccessTime = n.LastAccessTime;
            X = n.Coordinate.X;
            Y = n.Coordinate.Y;
        }

        public void setToNode(ICollection<Edge> node)
        {
            foreach (Edge e in node)
            {
                ToNode.Add(e.ToNode.NodeName);
            }
        }
    }
}
