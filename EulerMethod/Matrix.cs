using Newtonsoft.Json;

[JsonObject]
internal class Matrix
{
    [JsonProperty]
    private Vector[] matrix;

    [JsonIgnore]
    public int Rows { get; }

    [JsonIgnore]
    public int Columns { get; }

    public Matrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        matrix = new Vector[rows];
        for (int i = 0; i < rows; i++)
        {
            matrix[i] = new Vector(columns);
        }
    }

    [JsonConstructor]
    private Matrix(Vector[] matrix)
    {
        this.matrix = matrix;
        Rows = matrix.Length;
        Columns = matrix[0].Size;
    }

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
        get => matrix[row];
        set => matrix[row] = value;
    }
}
