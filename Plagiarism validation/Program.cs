using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using OfficeOpenXml;

using static app.Program;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;

namespace app
{

    class Program
    {
        #region structures
        public static int n=0;
        public const  int MAXN = 1000000;
        public static List<List<Edge>> adj;
        public static List<Edge> Edges;
        public static bool[] vis;
        public static int []group_num;
        public static string PATH = null;
        public class Edge
        {
            public int node1, node2;
            public float sim1, sim2;
            public int number_of_lines;
            public float mx_similarity;
            public Edge(int node1, int node2, float sim1, float sim2, int number_of_lines = 5)
            {
                this.node1 = node1;
                this.node2 = node2;
                this.sim1 = sim1;
                this.sim2 = sim2;
                this.mx_similarity = Math.Max(sim1, sim2);
                this.number_of_lines = number_of_lines;
            }
        }
        public class EdgeComparer : IComparer<Edge>
        {
            public int Compare(Edge x, Edge y)
            {
                int result = y.mx_similarity.CompareTo(x.mx_similarity);
                if (result == 0)
                    return y.number_of_lines.CompareTo(x.number_of_lines);
                return result;
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
        public static List<Edge> read_from_excel_and_build_Edges(ref int n , ref string PATH ,string filePath)
        {
            string path1, path2;
            int lineMatch;
            int id1, id2;
            int sim1, sim2;
            bool inB;
            bool start = false;
            int pathEnd = -1;
            List<Edge> Edges = new List<Edge>();
            // Set up EPPlus license context (EPPlus 5+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var workbook = package.Workbook;
                if (workbook != null)
                {
                    var worksheet = workbook.Worksheets.First();
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        path1 = worksheet.Cells[row, 1].Value.ToString();
                        path2 = worksheet.Cells[row, 2].Value.ToString();
                        lineMatch = int.Parse(worksheet.Cells[row, 3].Value.ToString());
                        id1 = 0;
                        id2 = 0;
                        inB = false;
                        sim1 = 0;
                        sim2 = 0;
                        for (int j = 0; j < path1.Length; j++)
                        {
                            char c = path1[j];
                            if (c == '(')
                            {
                                inB = true;
                                continue;
                            }
                            if (c == '%') break;
                            if (c >= '0' && c <= '9')
                            {
                                if (inB)
                                {
                                    if (!start)
                                    {
                                        sim1 = 0;
                                        start = true;
                                    }
                                    sim1 = sim1 * 10 + (c - '0');
                                }
                                else
                                {
                                    if (!start)
                                    {
                                        id1 = 0;
                                        start = true;
                                        pathEnd = j - 1;
                                    }
                                    id1 = id1 * 10 + (c - '0');
                                }
                            }
                            else start = false;
                        }
                        if (PATH == null)
                        {
                            if (pathEnd == -1)
                                PATH = "";
                            else
                                PATH = path1.Substring(0, pathEnd + 1);

                        }
                        inB = false;
                        foreach (var c in path2)
                        {
                            if (c == '(')
                            {
                                inB = true;
                                continue;
                            }
                            if (c == '%') break;
                            if (c >= '0' && c <= '9')
                            {
                                if (inB)
                                {
                                    if (!start)
                                    {
                                        sim2 = 0;
                                        start = true;
                                    }
                                    sim2 = sim2 * 10 + (c - '0');
                                }
                                else
                                {
                                    if (!start)
                                    {
                                        id2 = 0;
                                        start = true;
                                    }
                                    id2 = id2 * 10 + (c - '0');
                                }
                            }
                            else start = false;
                        }
                        n = Math.Max(n, id1);
                        n = Math.Max(n, id2);
                        Edges.Add(new Edge(id1, id2, sim1, sim2, lineMatch));
                    }
                }
            }
            return Edges;
        }
        public static List<List<Edge>> Build_Graph(List<Edge> Edges)
        {

            List<List<Edge>> adj = new List<List<Edge>>();
            // Your Code 
            vis = new bool[n + 1];
            group_num = new int[n + 1];
//            Console.WriteLine("n  = "+n);
            for (int i = 0; i <= n; i++)
            {
                adj.Add(new List<Edge>());
                vis[i] = false;
            }
            foreach (Edge e in Edges)
            {

                adj[e.node1].Add(e);
                adj[e.node2].Add(e);
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
                if (curNeighbor == node)
                {
                    curNeighbor = neighbor.node2;
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
                if (adj[i].Count == 0)
                    continue;
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

            foreach (KeyValuePair<List<int>, float> keyValuePair in groups_avg)
                keyValuePair.Key.Sort();
            groups_avg.Sort((x, y) => y.Value.CompareTo(x.Value));
            int group = 0;
            foreach (KeyValuePair<List<int>, float> keyValuePair in groups_avg)
            {
                foreach (int node in keyValuePair.Key)
                    group_num[node] = group;
                group++;
            }
            return groups_avg;
        }
        #endregion
        #region 3. Apply MST using 2 different algorithms (Kruskal, Prim)
        // Kerollos
        public static List<Edge> MST_Kruskal(List<Edge> Edges)
        {
            List<Edge> final_Edges = new List<Edge>();

            var sortedEdges = Edges.OrderByDescending(x => x.mx_similarity)
                       .ThenByDescending(x => x.number_of_lines);
            DSU dsu = new DSU(n + 1);
            foreach (var Edge in sortedEdges)
            {
                int node1 = Edge.node1, node2 = Edge.node2;
                if (dsu.Find(node1) == dsu.Find(node2)) continue;
                dsu.Union(node1, node2);
                final_Edges.Add(Edge);
            }
            List<Edge>[] group = new List<Edge>[n + 1];
            foreach (var Edge in final_Edges)
            {
                int p = dsu.Find(Edge.node1);
                if (group[p] == null)
                    group[p] = new List<Edge>();
                group[dsu.Find(Edge.node1)].Add(Edge);
            }
            int j = 0;
            for (int i = 1; i <= n; i++)
            {
                int p = dsu.Find(i);
                if (group[p] == null) continue;
                foreach (var Edge in group[p])
                {
                    final_Edges[j++] = Edge;
                }
                group[p] = null;
            }
            final_Edges.Sort((a, b) => {
                int res = group_num[a.node1].CompareTo(group_num[b.node1]);
                if(res==0)
                    return b.number_of_lines.CompareTo(a.number_of_lines);
                return res;
            });
            return final_Edges;
        }
        // Abanoub
        public static void Prim_one_group(int start, List<Edge> final_Edges, List<List<Edge>> adj)
        {
            PriorityQueue<bool, Edge> pq = new PriorityQueue<bool,Edge>(new EdgeComparer());
            vis[start] = true;
            foreach (Edge e in adj[start])
                pq.Enqueue(true, e);
            List<Edge>final_Edges_of_groups = new List<Edge>();
            while (pq.TryDequeue(out bool m, out Edge mx_Edge))
            {
                if (!vis[mx_Edge.node1])
                    start = mx_Edge.node1;
                else if (!vis[mx_Edge.node2])
                    start = mx_Edge.node2;
                else continue;
                vis[start] = true;
                final_Edges_of_groups.Add(mx_Edge);
                foreach (Edge e in adj[start])
                {
                    if (!vis[e.node1] | !vis[e.node2])
                        pq.Enqueue(true, e);
                }
            }
            final_Edges_of_groups.Sort((a, b) => b.number_of_lines.CompareTo(a.number_of_lines));
            foreach (Edge e in final_Edges_of_groups)
                final_Edges.Add(e);
        }
        public static List<Edge> MST_Prim(List<List<Edge>> adj, List<KeyValuePair<List<int>, float>> sorted_groups_by_avg)
        {
            List<Edge> final_Edges = new List<Edge>();
            for (int id = 1; id <= n; id++)
                vis[id] = false;
            foreach (KeyValuePair<List<int>, float> keyValuePair in sorted_groups_by_avg)
                Prim_one_group(keyValuePair.Key[0],final_Edges,adj);
            return final_Edges;
        }
        #endregion;
        #region 4. Output groups' statistics & final Edges
        // Ali 
        public static string print_edges(Edge e, string PATH)
        {
            string s = "";
            s+=PATH + e.node1 + "/(" + e.sim1 + "%)";
            s+="     ";
            s += PATH + e.node2 + "/(" + e.sim2 + "%) ";
            s += "     ";
            s += e.number_of_lines;
            return s;
        }
        // Marina
        public static void output_Edges_of_MST_into_excel(List<Edge> final_Edges)
        {
            Console.WriteLine("File 1     File 2     Line Matches");
            // Write this output in the excel file in mst_files format
            int line = 0;
            foreach (Edge e in final_Edges)
            {
                Console.Write(line++ + "    ");
                Console.Write(PATH+e.node1+"/("+e.sim1+"%)");
                Console.Write("     ");
                Console.Write(PATH + e.node2 + "/(" + e.sim2 + "%) ");
                Console.Write("     ");
                Console.WriteLine(e.number_of_lines);
            }
        }
        public static void OutputEdgesOfMSTIntoExcel(List<Edge> finalEdges,
            string filePath = @"E:\projects\algo\Project\Application\Plagiarism validation\Test Cases\Complete\Easy\Output\mst_file.xlsx")
        {
            // Set up EPPlus license context (EPPlus 5+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("MST");

                // Add header row
                worksheet.Cells["A1"].Value = "File 1";
                worksheet.Cells["B1"].Value = "File 2";
                worksheet.Cells["C1"].Value = "Line Matches";

                // Populate the worksheet with data
                int rowIndex = 2; // Start from the second row
                foreach (Edge edge in finalEdges)
                {

                    string file1 = $@"{PATH}{edge.node1}/({edge.sim1}%)" , file2 = $@"{PATH}{edge.node2}/({edge.sim2}%)";
                    worksheet.Cells[rowIndex, 1].Value = file1;
                    worksheet.Cells[rowIndex, 1].Style.Font.UnderLine = true;
                    worksheet.Cells[rowIndex, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[rowIndex, 2].Value = file2;
                    worksheet.Cells[rowIndex, 2].Style.Font.UnderLine = true;
                    worksheet.Cells[rowIndex, 2].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                    worksheet.Cells[rowIndex, 3].Value = edge.number_of_lines;
                    rowIndex++;
                }

                // AutoFit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the Excel package to a file
                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }
        }
        public static void OutputGroupsStatisticsIntoExcel(List<KeyValuePair<List<int>, float>> groupsAvg, string filePath= @"E:\projects\algo\Project\Application\Plagiarism validation\Test Cases\Complete\Easy\Output\Stat_file.xlsx")
        {
            // Set up EPPlus license context (EPPlus 5+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Create a new Excel package
            using (var package = new ExcelPackage())
            {
                // Add a new worksheet
                var worksheet = package.Workbook.Worksheets.Add("Components");

                // Add header row
                worksheet.Cells["A1"].Value = "Component Index";
                worksheet.Cells["B1"].Value = "Vertices";
                worksheet.Cells["C1"].Value = "Average Similarity";
                worksheet.Cells["D1"].Value = "Component Count";

                // Populate the worksheet with data
                int rowIndex = 2; // Start from the second row
                foreach (var group in groupsAvg)
                {
                    worksheet.Cells[rowIndex, 1].Value = rowIndex - 1; // Component Index
                    worksheet.Cells[rowIndex, 2].Value = string.Join(", ", group.Key); // Vertices
                    worksheet.Cells[rowIndex, 3].Value = Math.Round(group.Value,1); // Average Similarity
                    worksheet.Cells[rowIndex, 4].Value = group.Key.Count; // Component Count
                    rowIndex++;
                }

                // AutoFit columns for all cells
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save the Excel package to a file
                FileInfo excelFile = new FileInfo(filePath);
                package.SaveAs(excelFile);
            }
        }

        #endregion;
        #region TESTS
        public static bool CompareExcelFiles(string filePath1, string filePath2)
        {
            // Set up EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Load the first Excel file
            using (var package1 = new ExcelPackage(new FileInfo(filePath1)))
            using (var package2 = new ExcelPackage(new FileInfo(filePath2)))
            {
                // Get the first worksheet in each file
                var worksheet1 = package1.Workbook.Worksheets[0];
                var worksheet2 = package2.Workbook.Worksheets[0];

                // Check if dimensions are the same
                if (worksheet1.Dimension.Address != worksheet2.Dimension.Address)
                {
                    Console.WriteLine("Worksheets have different dimensions.");
                    return false;
                }

                // Compare each cell in the worksheet
                for (int row = 1; row <= worksheet1.Dimension.End.Row; row++)
                {
                    for (int col = 1; col <= worksheet1.Dimension.End.Column; col++)
                    {
                        var cell1 = worksheet1.Cells[row, col].Text;
                        var cell2 = worksheet2.Cells[row, col].Text;
                        if (cell1 != cell2)
                        {
                            Console.WriteLine($"Mismatch at row {row}, column {col}: '{cell1}' vs '{cell2}'");
                            return false;
                        }
                    }
                }
            }
            Console.WriteLine("TEST STAT PASSED Successfully");
            // If no differences are found
            return true;
        }

        public static void TEST_Algorithm(int id, List<Edge> Edges, List<Edge> output_edges, string PATH, string PATH_output)
        {
            Edges.Sort((a, b) => a.number_of_lines.CompareTo(b.number_of_lines));
            output_edges.Sort((a, b) => a.number_of_lines.CompareTo(b.number_of_lines));
            Console.WriteLine("TEST ID: " + id);
            double sum1 = 0, sum2 = 0;

            for (int i = 0; i < output_edges.Count; i++)
            {
                sum1 += Edges[i].mx_similarity;
                sum2 += output_edges[i].mx_similarity;
                if (Edges[i].number_of_lines != output_edges[i].number_of_lines)
                {
                    string s1 = print_edges(Edges[i], PATH);
                    string s2 = print_edges(output_edges[i], PATH_output);
                    Console.WriteLine("Line " + i);
                    Console.WriteLine("expected: " + s2);
                    Console.WriteLine("Output: " + s1);
                    return;
                }
            }
            if(sum1==sum2)
                Console.WriteLine("TEST PASSED Successfully");
            else
                Console.WriteLine("TEST Faild");
        }
        #endregion;

        static void Main(string[] args)
        {
            int level;
            do
            {
                Console.WriteLine("Enter test level (1/Easy 2/Medium 3/Hard):");
                level = int.Parse(Console.ReadLine());
                if (level < 1 || level > 3)
                {
                    Console.WriteLine("Invalid input:");
                    break;
                }
                Console.WriteLine("Enter test number 1/2:");
                int test = int.Parse(Console.ReadLine());
                if (test < 1 || test > 2)
                {
                    Console.WriteLine("Invalid input:");
                }
                else
                {
                    Console.WriteLine("Choose Algorithm (1/Prim 2/Kruskal 3/both):");
                    int algo = int.Parse(Console.ReadLine());
                    string chosen_path_in = @"E:\projects\algo\Project\Application\Plagiarism validation\Test Cases\Complete\";
                    string chosen_path_out = @"E:\projects\algo\Project\Application\Plagiarism validation\Output\";

                    if (level == 1)
                    {
                        chosen_path_in += "Easy\\";
                        chosen_path_out += "Easy\\";
                    }
                    else if (level == 2)
                    {
                        chosen_path_in += "Medium\\";
                        chosen_path_out += "Medium\\";

                    }
                    else
                    {
                        chosen_path_in += "Hard\\";
                        chosen_path_out += "Hard\\";

                    }
                    string chosen_path_expected_edges, chosen_path_expected_stat;
                    string chosen_path_output_edges, chosen_path_output_stat;
                    chosen_path_expected_edges = chosen_path_expected_stat = chosen_path_in;
                    chosen_path_output_edges = chosen_path_output_stat = chosen_path_out;
                    chosen_path_in += test+"-Input.xlsx";
                    chosen_path_expected_edges += test + "-mst_file.xlsx";
                    chosen_path_expected_stat += test + "-StatFile.xlsx";
                    chosen_path_output_edges += test + "-StatFile";
                    chosen_path_output_stat += test + "-StatFile.xlsx";
                    var watch = Stopwatch.StartNew();
                    Edges = read_from_excel_and_build_Edges(ref n, ref PATH, chosen_path_in);
                    adj = Build_Graph(Edges);
                    List<KeyValuePair<List<int>, float>> groups_avg = calculate_avg_for_each_group();
                    List<Edge> prim_Edges=null, kruskal_Edges=null;
                    if (algo == 1)
                    {
                        prim_Edges = MST_Prim(adj, groups_avg);
                        OutputEdgesOfMSTIntoExcel(prim_Edges, chosen_path_output_edges + "_prim.xlsx");
                    }
                    else if (algo == 2)
                    {
                        kruskal_Edges = MST_Kruskal(Edges);
                        OutputEdgesOfMSTIntoExcel(kruskal_Edges, chosen_path_output_edges + "_kruskal.xlsx");
                    }
                    else
                    {
                        prim_Edges = MST_Prim(adj, groups_avg);
                        kruskal_Edges = MST_Kruskal(Edges);
                        OutputEdgesOfMSTIntoExcel(kruskal_Edges, chosen_path_output_edges + "_kruskal.xlsx");
                        OutputEdgesOfMSTIntoExcel(prim_Edges, chosen_path_output_edges + "_prim.xlsx");
                    }
                    OutputGroupsStatisticsIntoExcel(groups_avg, chosen_path_output_stat);
                    watch.Stop();
                    long elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine($"Execution time: {elapsedMs} ms");
                    string format_path_output = "";
                    List<Edge> output_edges = read_from_excel_and_build_Edges(ref n, ref format_path_output, chosen_path_expected_edges);

                    // compare here
                    if(algo!=2)
                        TEST_Algorithm(1, prim_Edges, output_edges, PATH, format_path_output);
                    if (algo != 1)
                        TEST_Algorithm(2, kruskal_Edges, output_edges, PATH, format_path_output);
/*                    CompareExcelFiles(chosen_path_output_stat, chosen_path_expected_stat);
*/                }
            }while (level>0);
        }
    }
}