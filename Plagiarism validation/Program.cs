using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace app
{

    class Program
    {
        #region structures
        public static int n;
        public static List<List<edge>> adj;
        public static List<edge> edges;
        public static bool[] vis;
        public class edge
        {
            public int node1, node2;
            public float sim1, sim2;
            public int number_of_lines;
            public float mx_similarity ;
        }
        private class InverseComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                return y.CompareTo(x);
            }
        }

        #endregion ;

        #region 1. Read from excel file
        // Mariam
        public static List<edge> read_from_excel()
        {
            List<edge> edges = new List<edge>();
            // Your Code
            return edges;
        }
        public static List<List<edge>> Build_Graph(List<edge> edges)
        {
            List<List<edge>> adj = new List<List<edge>>();
            // Your Code 
            return adj;
        }
        #endregion;
        #region 2. Find avg for each group
        // Sohier
        public static List<KeyValuePair<List<int>, float>> calculate_avg_for_each_group()
        {
            List<KeyValuePair<List<int>, float>> groups_avg = new List<KeyValuePair<List<int>, float>>();
            // Your Code 

            return groups_avg;
        }
        #endregion
        #region 3. Apply MST using 2 different algorithms (Kruskal, Prim)
        // Kerollos
        public static List<edge> MST_Kruskal(List<edge> edges)
        {
            List<edge> final_edges = new List<edge>();
            // Your Code
            return final_edges;
        }
        // Abanoub
        public static void Prim_one_group(int start, List<edge> final_edges)
        {
            PriorityQueue<edge, float> pq = new PriorityQueue<edge, float>(new InverseComparer());
            vis[start] = true;
            foreach (edge e in adj[start])
                pq.Enqueue(e, e.mx_similarity);
            while (pq.TryDequeue(out edge mx_edge, out float priority))
            {
                if (!vis[mx_edge.node1])
                    start = mx_edge.node1;
                else if (!vis[mx_edge.node2])
                    start = mx_edge.node2;
                else continue;

                vis[start] = true;
                final_edges.Add(mx_edge);
                foreach (edge e in adj[start])
                {
                    if (!vis[e.node1] | !vis[e.node2])
                        pq.Enqueue(e, e.mx_similarity);
                }
            }
        }
        public static List<edge> MST_Prim()
        {
            List<edge> final_edges = new List<edge>();
            for (int id = 1; id <= n; id++)
                vis[id] = false;
            for (int node = 1; node <= n; node++)
            {
                if (!vis[node])
                    Prim_one_group(node, final_edges);
            }
            return final_edges;
        }
        #endregion;
        #region 4. Output groups' statistics & final edges
        // Ali 
        public static void ouput_groups_statistics_into_excel(List<KeyValuePair<List<int>, float>> groups_avg)
        {
            // Your Code
        }

        // Marina
        public static void output_edges_of_MST_into_excel(List<edge> final_edges)
        {
            // Your Code
        }
        #endregion;
        static void Main(string[] args)
        {
            List<edge> final_edges = new List<edge>();
        }
    }
}
