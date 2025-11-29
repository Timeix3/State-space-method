
internal class Matrix
{
    private List<Vector> _matrix;

    public int Rows => _matrix.Count;

    public int Columns => _matrix[0].Size;

    public Matrix(int rows, int columns)
    {
        _matrix = new List<Vector>(rows);
        for (int i = 0; i < rows; i++)
        {
            _matrix.Add(new Vector(columns));
        }
    }

    public Matrix(List<Vector> vectors) => _matrix = vectors;

    public static Vector operator *(Matrix matrix, Vector vector)
    {
        if (matrix.Columns != vector.Size)
            throw new ArgumentException($"Количество столбцов матрицы ({matrix.Columns}) должно совпадать с размером вектора ({vector.Size})");
        Vector result = new Vector(matrix.Rows);
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i] += matrix[i][j] * vector[j];
            }
        }
        return result;
    }

    public void Add(Vector row) => _matrix.Add(row);

    public void Print()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Console.Write($"{this[i][j],6:F3} ");
            }
            Console.WriteLine();
        }
    }

    public static Matrix Transpose(Matrix M)
    {
        Matrix T = new Matrix(M.Columns, M.Rows);
        for (int i = 0; i < M.Rows; i++)
            for (int j = 0; j < M.Columns; j++)
                T[j][i] = M[i][j];
        return T;
    }

    public Vector GetColumn(int columnIndex)
    {
        Vector column = new Vector(Rows);
        for (int i = 0; i < Rows; i++)
        {
            column[i] = this[i][columnIndex];
        }
        return column;
    }

    public Vector this[int row]
    {
        get => _matrix[row];
        set => _matrix[row] = value;
    }
}
