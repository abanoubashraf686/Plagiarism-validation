using System;
using System.Collections.Generic;

namespace app
{

    class Program
    {
        #region structures
        public static int n;
        public class node
        {
            public int name;
            public float similarity;
        }
        List<node> []adj;
        #endregion ;

        #region 1. Read from excel file
        // Mariam
        public static List<Tuple<node, node, int>> read_from_excel()
        {
            List<Tuple<node, node, int /*number of lines*/>> edge = new List<Tuple<node, node, int>>();
            // Your Code
            return edge;
        }
        public static List<Tuple<node, node, int>> Build()
        {
            List<Tuple<node, node, int /*number of lines*/>> edge = new List<Tuple<node, node, int>>();
            // Your Code
            return edge;
        }
        #endregion;
        #region 2. Find avg for each group
        // Sohier
        public static List<KeyValuePair<List<node>, float>> calculate_avg_for_each_group(List<Tuple<node, node, int>> edges)
        {
            List<KeyValuePair<List<node>, float>> groups_avg = new List<KeyValuePair<List<node>, float>>();
            // Your Code 

            return groups_avg;
        }
        #endregion
        #region 3. Apply MST using 2 different algorithms (Kruskal, Prim)
        // Kerollos
        public static List<Tuple<node, node, int>> MST_Kruskal(List<Tuple<node, node, int>> edges)
        {
            List<Tuple<node, node, int>> final_edges = new List<Tuple<node, node, int>>();
            // Your Code 

            return final_edges;
        }
        // Abanoub
        public static List<Tuple<node, node, int>> MST_Prim(List<Tuple<node, node, int>> edges)
        {
            List<Tuple<node, node, int>> final_edges = new List<Tuple<node, node, int>>();
            // Your Code 
            return final_edges;
        }
        #endregion;
        #region 4. Output groups' statistics & final edges
        // Ali 
        public static void ouput_groups_statistics_into_excel(List<KeyValuePair<List<node>, float>> groups_avg)
        {
            // Your Code
        }

        // Marina
        public static void output_edges_of_MST_into_excel(List<Tuple<node, node, int>> final_edges)
        {
            // Your Code
        }
        #endregion;
        static void Main(string[] args)
        {

        }
    }
}
