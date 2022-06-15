
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Collections;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using WindowsFormsApplication35;
namespace WindowsFormsApplication35
{
    class DirectedWeightedGraph
    {


        public readonly int MAX_VERTICES = 30;
        int n;
        int e;
        int[,] adj;
        Vertex[] vertexList;
        int kok = 0, hedef = 0;
        List<int> kisaYol = new List<int>();
        public static int[] cizgipoint = new int[10];

        Point[] here;
        Graphics gObjectt;
        private readonly int TEMPORARY = 1;
        private readonly int PERMANENT = 2;
        private readonly int NIL = -1;
        private readonly int INFINITY = 99999;

        public DirectedWeightedGraph()
        {
            adj = new int[MAX_VERTICES, MAX_VERTICES];
            vertexList = new Vertex[MAX_VERTICES];
        }


        private void Dijkstra(int s)
        {
            int v, c;

            for (v = 0; v < n; v++)
            {
                vertexList[v].status = TEMPORARY;
                vertexList[v].pathLength = INFINITY;
                vertexList[v].predecessor = NIL;
            }

            vertexList[s].pathLength = 0;

            while (true)
            {
                c = TempVertexMinPL();

                if (c == NIL)
                    return;

                vertexList[c].status = PERMANENT;

                for (v = 0; v < n; v++)
                {
                    if (IsAdjacent(c, v) && vertexList[v].status == TEMPORARY)
                    {
                        if (vertexList[c].pathLength + adj[c, v] < vertexList[v].pathLength)
                        {
                            vertexList[v].predecessor = c;
                            vertexList[v].pathLength = vertexList[c].pathLength + adj[c, v];
                        }

                    }

                }
            }
        }

        private int TempVertexMinPL()
        {
            int min = INFINITY;
            int x = NIL;
            for (int v = 0; v < n; v++)
            {
                if (vertexList[v].status == TEMPORARY && vertexList[v].pathLength < min)
                {
                    min = vertexList[v].pathLength;
                    x = v;
                }
            }
            return x;
        }

        public void FindPaths(String source)
        {
            int s = GetIndex(source);
            Brush black = new SolidBrush(Color.Black);
            Pen redPen = new Pen(black, 6);
            Dijkstra(s);

            Console.WriteLine("kaynak : " + source + "\n");

            for (int v = 0; v < n; v++)
            {
                Console.WriteLine("varış : " + vertexList[v].name);
                if (vertexList[v].pathLength == INFINITY)
                    Console.WriteLine("There is no path from " + source + " to vertex " + vertexList[v].name + "\n");
                else
                {
                    FindPath(s, v);
                    for (int i = count; i >= 1; i--)
                    {
                        gObjectt.DrawLine(redPen, here[cizgipoint[i]], here[cizgipoint[i-1]]);
                    }
                    break;
                }
            }

        }

        internal void InsertVertex()
        {
            throw new NotImplementedException();
        }
        public int count = 0;
        private void FindPath(int s, int v)
        {
            Brush black = new SolidBrush(Color.Black);
            Pen redPen = new Pen(black, 4);
            int i, u;
            int[] path = new int[100];
            int sd = 0;
            count = 0;

            while (v != s)
            {
                count++;
                path[count] = v;
                u = vertexList[v].predecessor;
                sd += adj[u, v];
                v = u;
            }
            count++;
            path[count] = s;
            
            MessageBox.Show("En kısa rota : ");
            for (i = count; i >= 1; i--)
            { 
                MessageBox.Show(path[i] + 1 + "->");
                cizgipoint[i] = path[i];
          //    gObjectt.DrawLine(redPen, here[path[i]], here[path[i - 1]]);
            }
          
            kisaYol.Add(sd);
            if (sd != 0)
            {
                MessageBox.Show("\n En kısa yol : " + sd + "\n");

            }
        }

        private int GetIndex(String s)
        {
            for (int i = 0; i < n; i++)
                if (s.Equals(vertexList[i].name))
                    return i;
            throw new System.InvalidOperationException("Invalid Vertex");
        }

        public void InsertVertex(String name)
        {
            vertexList[n++] = new Vertex(name);
        }


        private bool IsAdjacent(int u, int v)
        {
            return (adj[u, v] != 0);
        }

        /*Insert an edge (s1,s2) */
        public void InsertEdge(String s1, String s2, int wt)
        {
            int u = GetIndex(s1);
            int v = GetIndex(s2);
            if (u == v)
                throw new System.InvalidOperationException("Not a valid edge");

            if (adj[u, v] != 0)
                Console.Write("Edge already present");
            else
            {
                adj[u, v] = wt;
                e++;
            }
        }

        public int[,] matrix()
        {
            return adj;
        }
        public List<int> kisaYolR()
        {
            return kisaYol;
        }
        public Point[] getpoin
        {
            get
            {
                return here;
            }
            set
            {
                here = value;
            }
        }
        public Graphics getObject
        {
            get
            {
                return gObjectt;
            }
            set
            {
                gObjectt = value;
            }
        }

    }
}