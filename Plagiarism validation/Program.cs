using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace app
{

    class Program
    {
        #region structures
        public static int n;
        public static List<List<edge>> adj;
        public static List<edge> edges;
        public static bool[] vis;
        public static List<string> name;
        public class edge
        {
            public int node1, node2;
            public float sim1, sim2;
            public int number_of_lines;
            public float mx_similarity;
            public edge(int node1, int node2, float sim1, float sim2, int number_of_lines = 5)
            {
                this.node1 = node1;
                this.node2 = node2;
                this.sim1 = sim1;
                this.sim2 = sim2;
                this.mx_similarity = Math.Max(sim1, sim2);
                this.number_of_lines = number_of_lines;
            }
        }
        private class InverseComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                return y.CompareTo(x);
            }
        }
        class DSU
        {
            private int[] parent;
            private int[] size;
            public DSU(int sz)
            {
                parent = new int[sz];
                size = new int[sz];
                for (int i = 0; i < sz; i++)
                {
                    parent[i] = i;
                    size[i] = 0;
                }
            }
            public int Find(int x)
            {
                if (parent[x] != x)
                    parent[x] = Find(parent[x]);
                return parent[x];
            }
            public void Union(int x, int y)
            {
                x = Find(x);
                y = Find(y);
                if (x == y) return;
                if (size[y] > size[x]) (x, y) = (y, x);
                parent[y] = x;
                size[x] += size[y];
            }
        }
        #endregion ;

        #region 1. Read from excel file
        // Mariam
        public static List<edge> read_from_excel_and_build_edges()
        {
            // Your code


            
            
            // test code
            List<edge> edges = new List<edge>();

            // Data in the text format
            string[] lines = {
                "https://example.com/file1/(36%) https://example.com/file2/(62%) 98",
                "https://example.com/file2/(34%) https://example.com/file3/(96%) 130",
                "https://example.com/file3/(60%) https://example.com/file4/(76%) 136",
                "https://example.com/file4/(94%) https://example.com/file5/(78%) 172",
                "https://example.com/file5/(94%) https://example.com/file6/(69%) 163",
                "https://example.com/file6/(78%) https://example.com/file1/(99%) 177",
                "https://example.com/file7/(97%) https://example.com/file8/(85%) 182",
                "https://example.com/file8/(91%) https://example.com/file9/(44%) 135",
                "https://example.com/file9/(42%) https://example.com/file10/(22%) 64",
                "https://example.com/file10/(86%) https://example.com/file11/(89%) 175",
                "https://example.com/file11/(42%) https://example.com/file12/(42%) 84",
                "https://example.com/file12/(80%) https://example.com/file7/(49%) 129"
            };

            Dictionary<string, int> fileToNode = new Dictionary<string, int>();
            name = new List<string>();
            name.Add("Alsalamo 3likom our respective readers");
            int nextNode = 1;
            foreach (string line in lines)
            {
                string[] parts = line.Split(' ');
                string name1 = parts[0];
                string name2 = parts[1];
                int number_of_lines = int.Parse(parts[2]);

                string name1_ = name1.Substring(0, name1.Length - 5);
                string name2_ = name2.Substring(0, name2.Length - 5);
                if (!fileToNode.ContainsKey(name1_))
                {
                    fileToNode[name1_] = nextNode++;
                    name.Add(name1_);
                }
                if (!fileToNode.ContainsKey(name2_))
                {
                    fileToNode[name2_] = nextNode++;
                    name.Add(name1_);
                }

                float sim1 = float.Parse(name1.Substring(name1.Length - 4, 2));
                float sim2 = float.Parse(name2.Substring(name2.Length - 4, 2));

                edges.Add(new edge(fileToNode[name1_], fileToNode[name2_], sim1, sim2, number_of_lines));
            }
            n = nextNode - 1;
            return edges;
        }
        // Gadallah
        public static List<List<edge>> Build_Graph(List<edge> edges)
        {
           
            List<List<edge>> adj = new List<List<edge>>();
            // Your Code 
            vis = new bool[n + 1];
            for (int i = 0; i <= n; i++)
            {
                adj.Add(new List<edge>());
                vis[i] = false;
            }
            foreach(var edge in edges) {

                adj[edge.node1].Add(edge);
                adj[edge.node2].Add(edge);
            }
            return adj;
        }
        #endregion;
        #region 2. Find avg for each group

        // Sohier

        private static void Dfs(int node, ref float sum, ref int count, ref List<int> currentGroup)
        {
            vis[node] = true;
            currentGroup.Add(node);

            foreach (var neighbor in adj[node])
            {
                int curNeighbor = neighbor.node1;
                //float curSum = neighbor.sim1;
                if (curNeighbor == node)
                {
                    curNeighbor = neighbor.node2;
                    //curSum = neighbor.sim2;
                }
               
                count += 2;
                sum += neighbor.sim1 + neighbor.sim2;

                if (vis[curNeighbor] == false)
                {
                    Dfs(curNeighbor, ref sum, ref count, ref currentGroup);
                }
            }
        }

        public static List<KeyValuePair<List<int>, float>> calculate_avg_for_each_group()
        {
            List<KeyValuePair<List<int>, float>> groups_avg = new List<KeyValuePair<List<int>, float>>();
            for (int i = 1; i <= n; i++)
            {
                if (vis[i] == false)
                {
                    float sum = 0;
                    int count = 0;
                    List<int> currentGroup = new List<int>();
                    Dfs(i, ref sum, ref count, ref currentGroup);

                    float avg = sum / count;
                    groups_avg.Add(new KeyValuePair<List<int>, float>(currentGroup, avg));
                }
            }

            return groups_avg;
        }
        #endregion
        #region 3. Apply MST using 2 different algorithms (Kruskal, Prim)
        // Kerollos
        public static List<edge> MST_Kruskal(List<edge> edges)
        {
            List<edge> final_edges = new List<edge>();
           
            var sortedEdges = edges.OrderByDescending(x => x.mx_similarity);
            DSU dsu = new DSU(n + 1);
            foreach (var edge in sortedEdges)
            {
                int node1 = edge.node1, node2 = edge.node2;
                if (dsu.Find(node1) == dsu.Find(node2)) continue;
                dsu.Union(node1, node2);
                final_edges.Add(edge);
            }
            List<edge>[] group = new List<edge>[n + 1];
            foreach (var edge in final_edges)
            {
                int p = dsu.Find(edge.node1);
                if (group[p] == null)
                    group[p] = new List<edge>();
                group[dsu.Find(edge.node1)].Add(edge);
            }
            int j = 0;
            for (int i = 1; i <= n; i++)
            {
                int p = dsu.Find(i);
                if (group[p] == null) continue;
                foreach (var edge in group[p])
                {
                    final_edges[j++] = edge;
                }
                group[p] = null;
            }
            return final_edges;
        }
        // Abanoub
        public static void Prim_one_group(int start, List<edge> final_edges, List<List<edge>> adj)
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
        public static List<edge> MST_Prim(List<List<edge>> adj)
        {
            List<edge> final_edges = new List<edge>();
            for (int id = 1; id <= n; id++)
                vis[id] = false;
            for (int node = 1; node <= n; node++)
            {
                if (!vis[node])
                    Prim_one_group(node, final_edges, adj);
            }
            return final_edges;
        }
        #endregion;
        #region 4. Output groups' statistics & final edges
        // Ali 
        public static void ouput_groups_statistics_into_excel(List<KeyValuePair<List<int>, float>> groups_avg)
        {
            // Your code

            
            // Write this output in the excel file in StatFiles format
            foreach (var group in groups_avg)
            {
                foreach (var i in group.Key)
                {
                    Console.Write(name[i] + " ");
                }
                Console.WriteLine(group.Value);
            }
        }

        // Marina
        public static void output_edges_of_MST_into_excel(List<edge> final_edges)
        {
            // Your Code

            // Write this output in the excel file in mst_files format
            foreach (var edge in final_edges)
            {
                Console.Write(name[edge.node1] + " ");
                Console.Write(name[edge.node2] + " ");
                Console.WriteLine(edge.mx_similarity);
            }
        }
        #endregion;
        static void Main(string[] args)
        {
            edges = read_from_excel_and_build_edges();
            adj = Build_Graph(edges);

            List<KeyValuePair<List<int>, float>> groups_avg = calculate_avg_for_each_group();
            ouput_groups_statistics_into_excel(groups_avg);
            Console.WriteLine();
            Console.WriteLine();

            List<edge> prim_edges = MST_Prim(adj);
            output_edges_of_MST_into_excel(prim_edges);
            Console.WriteLine();
            Console.WriteLine();

            List<edge> kruskal_edges = MST_Kruskal(edges);
            output_edges_of_MST_into_excel(kruskal_edges);
        }
    }
}