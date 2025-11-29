
internal class DependencyFinder
{
    public string[] dependentVariables = [];

    public void FindDependentVariables(Matrix matrix, List<string> variables)
    {
        int mainRow = 0;
        while (mainRow < matrix.Rows)
        {
            for (int i = mainRow; i < matrix.Rows; i++)
                if (Math.Abs(matrix[i][mainRow]) > 1e-10)
                    (matrix[mainRow], matrix[i]) = (matrix[i], matrix[mainRow]);
            for (int j = mainRow; j < matrix.Columns; j++)
                if (Math.Abs(matrix[mainRow][j]) > 1e-10)
                    SwapColumns(matrix, variables, mainRow, j);
            double value = matrix[mainRow][mainRow];
            if (Math.Abs(value) > 1e-10)
            {
                matrix[mainRow].Normalize(value);
                for (int i = 0; i < matrix.Rows; i++)
                    if (i != mainRow)
                    {
                        double factor = matrix[i][mainRow];
                        for (int j = 0; j < matrix.Columns; j++)
                            matrix[i][j] -= factor * matrix[mainRow][j];
                    }
            }
            mainRow++;
        }
        var dependentVars = new List<string>();
        for (int i = 0; i < matrix.Rows; i++)
        {
            if (i < matrix.Columns && Math.Abs(matrix[i][i] - 1.0) < 1e-10) dependentVars.Add(variables[i]);
            else break;
        }
        dependentVariables = [.. dependentVars];
    }

    static void SwapColumns(Matrix matrix, List<string> variables, int col1, int col2)
    {
        for (int i = 0; i < matrix.Rows; i++)
            (matrix[i][col1], matrix[i][col2]) = (matrix[i][col2], matrix[i][col1]);
        (variables[col1], variables[col2]) = (variables[col2], variables[col1]);
    }
}

