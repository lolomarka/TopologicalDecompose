using System.Collections.Immutable;
using System.IO;
using System.Xml.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        var matrix = ReadMatrixFromFile("input.txt");
        Graph graph = new Graph(matrix);

        graph.PrintAllMatrixes();

        graph.PrintReachabilityMatrix();
        graph.PrintVertexOrders();
        graph.PrintTakt();
        graph.PrintContourExist();
        graph.PrintInputVertices();
        graph.PrintOutputVertices();
        graph.PrintHangingVertices();
        graph.PrintWaysNumberByLength(3);
        graph.PrintAllWaysCount();
        graph.PrintIncludeElements(8);
        graph.PrintExcludeElements(5);
    }



    private static int[][] ReadMatrixFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        var result = new int[lines.Length][];
        for (int i = 0; i < result.Length; i++)
        {
            var line = lines[i].ToCharArray();
            result[i] = new int[line.Length];
            for (int j = 0; j < line.Length; j++)
            {
                result[i][j] = -'0' + line[j];
            }
        }

        return result;
    }
}

internal class Graph
{
    private List<int[][]> _Matrixes;
    private Dictionary<int, int> _VertexOrders;
    private int[][] _ReachabilityMatrix;
    private List<int> _InputVertices;
    private List<int> _OutputVertices;
    private List<int> _HangingVertices;

    public Graph(int[][] matrix)
    {
        _Matrixes = new List<int[][]>();
        _Matrixes.Add(matrix);

        Exp();
        CalculateSigma();
    }

    public List<int[][]> Matrixes { get => _Matrixes; }
    public Dictionary<int, int> VertexOrders
    {
        get
        {
            if (_VertexOrders == null)
            {
                _VertexOrders = new Dictionary<int, int>();
                CalculateVertexOrders();
            }
            return _VertexOrders;
        }
    }


    private bool IsZeroMatrix(int[][] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[i].Length; j++)
            {
                if (matrix[i][j] != 0) { return false; }
            }
        }
        return true;
    }

    private int[] SumArrays(int[] arr1, int[] arr2)
    {
        int[] arr3 = new int[arr1.Length];
        for (int i = 0; i < arr1.Length; i++)
        {
            arr3[i] = arr1[i] + arr2[i];
        }
        return arr3;
    }

    private void CalculateReachabilityMatrix()
    {
        _ReachabilityMatrix = new int[Matrixes[0].Length][];
        for (int i = 0; i < ReachabilityMatrix.Length; i++)
        {
            ReachabilityMatrix[i] = new int[Matrixes[0][0].Length];
            foreach (var matrix in Matrixes)
            {
                ReachabilityMatrix[i] = SumArrays(ReachabilityMatrix[i], matrix[i]);
            }
        }
    }

    public void Exp()
    {
        int k = 0;
        int[][] startMatrix = Matrixes[k];

        while (!IsZeroMatrix(Matrixes[k]))
        {
            int[][] newMatrix = new int[startMatrix.Length][];
            for (int i = 0; i < startMatrix.Length; i++)
            {
                newMatrix[i] = new int[startMatrix[0].Length];
                for (int j = 0; j < startMatrix[i].Length; j++)
                {
                    if (startMatrix[i][j] != 0)
                    {
                        newMatrix[i] = SumArrays(newMatrix[i], Matrixes[k][j]);
                    }
                }
            }
            Matrixes.Add(newMatrix);
            k++;
        }
    }

    private void CalculateSigma()
    {
        foreach (var matrix in Matrixes)
        {
            for (int i = 0; i < matrix.Length - 1; i++)
            {
                for (int j = 0; j < matrix[0].Length - 1; j++)
                {
                    matrix[i][matrix[i].Length - 1] += matrix[i][j];
                    matrix[matrix.Length - 1][j] += matrix[i][j];
                }
            }
        }
    }

    private void CalculateVertexOrders()
    {
        int[] arr = Matrixes[0][Matrixes[0].Length - 1];

        for (int j = 0; j < arr.Length - 1; j++)
        {
            if (arr[j] == 0)
            {
                VertexOrders.Add(j + 1, 0);
            }
        }
        for (int i = 0; i < Matrixes.Count - 1; i++)
        {
            int[] arr1 = Matrixes[i][Matrixes[0].Length - 1];
            int[] arr2 = Matrixes[i + 1][Matrixes[0].Length - 1];
            for (int j = 0; j < arr1.Length - 1; j++)
            {
                if (arr1[j] > 0 && arr2[j] == 0)
                {
                    VertexOrders.Add(j + 1, i + 1);
                }
            }
        }
    }

    private bool CheckContur()
    {
        foreach (var matrix in Matrixes)
        {
            for (int i = 0; i < matrix.Length - 1; i++)
            {
                for (int j = 0; j < matrix[0].Length - 1; j++)
                {
                    if (matrix[i][j] != 0 && i == j)
                        return true;
                }
            }
        }
        return false;
    }

    public List<int> InputVerteces
    {
        get
        {
            if (_InputVertices == null)
            {
                _InputVertices = new List<int>();
                for (int i = 0; i < Matrixes[0].Length - 1; i++)
                {
                    if (Matrixes[0][i][Matrixes[0].Length - 1] == 0)
                    {
                        _InputVertices.Add(i + 1);
                    }
                }
            }
            return _InputVertices;
        }
    }

    public List<int> HangingVertices
    {
        get
        {
            if (_HangingVertices == null)
            {
                _HangingVertices = new List<int>();
                for (int i = 0; i < Matrixes[0].Length - 1; i++)
                {
                    if (Matrixes[0][i][Matrixes[0].Length - 1] == Matrixes[0][Matrixes[0].Length - 1][i]
                        && Matrixes[0][Matrixes[0].Length - 1][i] == 0)
                    {
                        _HangingVertices.Add(i + 1);
                    }
                }
            }
            return _HangingVertices;
        }
    }

    public int[][] ReachabilityMatrix
    {
        get
        {
            if (_ReachabilityMatrix == null)
            {
                CalculateReachabilityMatrix();
            }

            return _ReachabilityMatrix;
        }
    }

    public List<int> OutputVertices
    {
        get
        {
            if (_OutputVertices == null)
            {
                _OutputVertices = new List<int>();
                for (int i = 0; i < Matrixes[0].Length - 1; i++)
                {
                    if (Matrixes[0][i][Matrixes[0].Length - 1] == 0)
                    {
                        _OutputVertices.Add(i + 1);
                    }
                }
            }
            return _OutputVertices;
        }
    }

    public void PrintWaysNumberByLength(int l)
    {
        Console.WriteLine($"Число путей длинной {l}");
        for (int i = 0; i < Matrixes[0].Length - 1; i++)
        {
            for (int j = 0; j < Matrixes[0][0].Length - 1; j++)
            {
                if (Matrixes[l - 1][i][j] != 0)
                {
                    Console.WriteLine($"{i + 1} -> {j + 1} = {Matrixes[l - 1][i][j]}");
                }
            }
        }
    }

    private void PrintMatrix(int[][] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            Console.WriteLine(string.Join(" ", matrix[i]));
        }
    }

    public void PrintAllWaysCount()
    {
        Console.WriteLine("Количество всевозможных путей");
        for (int i = 0; i < ReachabilityMatrix.Length - 1; i++)
        {
            for (int j = 0; j < ReachabilityMatrix[0].Length - 1; j++)
                if (ReachabilityMatrix[i][j] != 0)
                {
                    Console.WriteLine($"{i + 1} -> {j + 1} = {ReachabilityMatrix[i][j]}");
                }
        }
    }

    public Dictionary<int, int> GetIncludeElements(int x)
    {
        var result = new Dictionary<int, int>();

        for (int i = 0; i < ReachabilityMatrix.Length - 1; i++)
        {
            if (ReachabilityMatrix[i][x - 1] != 0)
            {
                result.Add(i + 1, ReachabilityMatrix[i][x - 1]);
            }
        }
        return result;
    }

    public Dictionary<int, int> GetExcludeElements(int x)
    {
        var result = new Dictionary<int, int>();
        for (int i = 0; i < ReachabilityMatrix.Length - 1; i++)
        {
            if (ReachabilityMatrix[x - 1][i] != 0)
            {
                result.Add(i + 1, ReachabilityMatrix[x - 1][i]);
            }
        }
        return result;
    }

    public void PrintAllMatrixes()
    {
        int i = 1;

        foreach (var matrix in Matrixes)
        {
            Console.WriteLine($"A({i++}) = ");
            PrintMatrix(matrix);
            Console.WriteLine();
        }
    }

    public void PrintReachabilityMatrix()
    {
        Console.WriteLine("Матрица достижимости");
        PrintMatrix(ReachabilityMatrix);
    }

    public void PrintVertexOrders()
    {
        Console.WriteLine("Порядки элементов");
        foreach (var item in VertexOrders)
        {
            Console.WriteLine($"{item.Key} : {item.Value}");
        }
    }

    public void PrintTakt()
    {
        Console.WriteLine("Тактность системы");
        Console.WriteLine(VertexOrders.Values.Max());
    }

    public void PrintContourExist()
    {
        Console.WriteLine("Наличие контура");
        Console.WriteLine(CheckContur() ? "Да" : "Нет");
    }

    public void PrintInputVertices()
    {
        Console.WriteLine("Входные элементы");
        Console.WriteLine(string.Join(", ", InputVerteces));
    }

    public void PrintOutputVertices()
    {
        Console.WriteLine("Выходные элементы");
        Console.WriteLine(string.Join(", ", OutputVertices));
    }

    public void PrintHangingVertices()
    {
        Console.WriteLine("Висящие элементы");
        Console.WriteLine(string.Join(", ", HangingVertices));
    }

    public void PrintIncludeElements(int x)
    {
        Console.WriteLine($"Элементы, участвующие в формирование X{x} (элемент = сколько раз)");
        var elements = GetIncludeElements(x);
        foreach (var item in elements)
        {
            Console.WriteLine($"{item.Key} = {item.Value}");
        }
    }

    public void PrintExcludeElements(int x2)
    {
        Console.WriteLine($"Элементы, в формировании которых участвует X{x2} (элемент = сколько раз)");
        var elements = GetExcludeElements(x2);

        foreach (var item in elements)
        {
            Console.WriteLine($"{item.Key} = {item.Value}");
        }
    }
}

