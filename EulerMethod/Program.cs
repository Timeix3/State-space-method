using Newtonsoft.Json;

class Program
{
    private static void Main()
    {
        EulerSolver solver = new EulerSolver();
        Vector y = new(1);
        Console.WriteLine("H - Ввести вручную \nR - Считать из файла\nL - Схема аналогового устройства");
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.H)
        {
            Console.Write("Введите число переменных состояния n: ");
            int n = int.Parse(Console.ReadLine());

            Console.Write("Введите число источников m: ");
            int m = int.Parse(Console.ReadLine());

            Console.Write("Введите количество искомых величин k: ");
            int k = int.Parse(Console.ReadLine());

            var data = new EulerData
            {
                A = ReadMatrix("A", n, n),
                B = ReadMatrix("B", n, m),
                C = ReadMatrix("C", k, n),
                D = ReadMatrix("D", k, m),
                X = ReadVector("X0", n),
                V = ReadVector("V", m)
            };
            string json = JsonConvert.SerializeObject(data,Formatting.Indented);
            File.WriteAllText("euler_data2.json", json);
            y = solver.Solve(data);
        }
        else if (key.Key == ConsoleKey.R)
        {
            string jsonFromFile = File.ReadAllText("../../../euler_data.json");
            var eulerData = JsonConvert.DeserializeObject<EulerData>(jsonFromFile);
            y = solver.Solve(eulerData);
        }
        else if (key.Key == ConsoleKey.L)
        {
            Console.Write("Введите значения C, L, J, R1, R2: ");
            string input = Console.ReadLine();
            string[] values = input.Split(' ');
            double Cap = double.Parse(values[0]);
            double L = double.Parse(values[1]);
            double J = double.Parse(values[2]);
            double R1 = double.Parse(values[3]);
            double R2 = double.Parse(values[4]);
            Matrix A = new(2, 2);
            A[0][0] = -1 / (Cap * R2); A[0][1] = -1 / Cap;
            A[1][0] = 1 / L; A[1][1] = -R1 / L;
            Matrix B = new(2, 1);
            B[0][0] = 1 / Cap; B[1][0] = 0.0;
            Matrix C = new(2, 2);
            C[0][0] = 1 / R2; C[0][1] = 0.0;
            C[1][0] = -1 / R2; C[1][1] = -1;
            Matrix D = new(2, 1);
            D[0][0] = 0.0; D[1][0] = 1.0;
            Vector X = new(2);
            X[0] = 0.0; X[1] = 0.0;
            Vector V = new(1);
            V[0] = J;
            var data = new EulerData
            {
                A = A,
                B = B,
                C = C,
                D = D,
                X = X,
                V = V
            };
            y = solver.Solve(data);
        }
        Console.WriteLine("Y=");
        for(int i = 0; i < y.Size; i++)
            Console.WriteLine(y[i]);
        new Drawer("../../../output.png").DrawToFile(solver.Ydata, solver.time);
    }

    static Matrix ReadMatrix(string name, int rows, int cols)
    {
        Matrix M = new Matrix(rows, cols);
        Console.WriteLine($"\nВведите матрицу {name} ({rows}x{cols})");
        for (int i = 0; i < rows; i++)
        {
            string[] parts = (Console.ReadLine()).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < cols; j++)
                M[i][j] = double.Parse(parts[j]);
        }
        return M;
    }

    static Vector ReadVector(string name, int n)
    {
        Console.WriteLine($"\nВведите вектор {name} ({n} элементов):");
        string[] parts = (Console.ReadLine()).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Vector v = new Vector(n);
        for (int i = 0; i < n; i++)
            v[i] = double.Parse(parts[i]);
        return v;
    }
}
