
internal class SystemSolver
{
    private List<Branch> _branches;
    private Matrix _systemMatrix;
    private string[] _variables;
    private string[] _variablesY;

    public SystemSolver(List<Branch> branches, Matrix systemMatrix, string[] variables, string[] variablesY)
    {
        _branches = branches;
        _systemMatrix = systemMatrix;
        _variables = variables;
        _variablesY = variablesY;
    }

    public SystemData Solve()
    {
        SystemData data = new();
        RearrangeMatrix(data);
        GaussianMethod(data);
        ExtractAB(data);
        ExtractCD(data);
        CreateV(data);
        CreateX(data);
        return data;
    }

    private void CreateX(SystemData data)
    {
        data.X = new Vector(data.VariablesX.Count);
        for (int i = 0; i < data.VariablesX.Count; i++)
        {
            Console.Write($"Введите значение {data.VariablesX[i]} в момент времени t = 0: ");
            data.X[i] = double.Parse(Console.ReadLine());
        }
    }

    private void CreateV(SystemData data)
    {
        data.V = new Vector(data.VariablesInput.Count);
        for(int i = 0; i < data.VariablesInput.Count; i++)
            data.V[i] = _branches.FirstOrDefault(b => b.Name == "J" || b.Name == "E").Value;
    }

    private void RearrangeMatrix(SystemData data)
    {
        List<string> variablesOther = new();

        foreach (var variable in _variables)
        {
            string[] parts = variable.Split('_');
            string variableType = parts[0];
            Branch branch = _branches.FirstOrDefault(b => b.Name == parts[1]);
            if (branch.Type == "C" && variableType == "U")
                data.VariablesX.Add(variable);
            else if (branch.Type == "L" && variableType == "I")
                data.VariablesX.Add(variable);
            else if ((variableType == "I" && branch.Name == "J") || (variableType == "U" && branch.Name == "E"))
                data.VariablesInput.Add(variable);
            else variablesOther.Add(variable);
        }
        string[] sortedVariables = [.. data.VariablesX, .. data.VariablesInput, .. variablesOther];
        int[] columnSwap = [.. sortedVariables.Select(index => Array.IndexOf(_variables, index))];
        Matrix newMatrix = new Matrix(_systemMatrix.Rows, _systemMatrix.Columns);
        for (int i = 0; i < _systemMatrix.Rows; i++)
            for (int j = 0; j < _systemMatrix.Columns; j++)
                newMatrix[i][j] = _systemMatrix[i][columnSwap[j]];

        _systemMatrix = newMatrix;
        _variables = sortedVariables;
    }

    private void GaussianMethod(SystemData data)
    {
        int mainRow = 0;
        for (int column = data.VariablesX.Count + data.VariablesInput.Count; column < _systemMatrix.Columns 
            && mainRow < _systemMatrix.Rows; column++)
        {
            int nonZeroRow = -1;
            for (int i = mainRow; i < _systemMatrix.Rows; i++)
            {
                if (Math.Abs(_systemMatrix[i][column]) > 1e-10)
                {
                    nonZeroRow = i;
                    break;
                }
            }
            if (nonZeroRow != mainRow)
                (_systemMatrix[mainRow], _systemMatrix[nonZeroRow]) = (_systemMatrix[nonZeroRow], _systemMatrix[mainRow]);
            double mainValue = _systemMatrix[mainRow][column];
            for (int i = 0; i < _systemMatrix.Columns; i++)
                _systemMatrix[mainRow][i] /= mainValue;
            for (int i = 0; i < _systemMatrix.Rows; i++)
                if (i != mainRow)
                {
                    double factor = _systemMatrix[i][column];
                    for (int j = 0; j < _systemMatrix.Columns; j++)
                        _systemMatrix[i][j] -= factor * _systemMatrix[mainRow][j];
                }
            mainRow++;
        }
    }

    private void ExtractAB(SystemData data)
    {
        data.A = new Matrix(data.VariablesX.Count, data.VariablesX.Count);
        data.B = new Matrix(data.VariablesX.Count, data.VariablesInput.Count);

        for (int i = 0; i < data.VariablesX.Count; i++)
        {
            string[] parts = data.VariablesX[i].Split('_');
            string variableType = parts[0];
            string variableName = parts[1];
            Branch branch = _branches.First(b => b.Name == variableName);
            if (branch.Type == "C" && variableType == "U")
            {
                int row = FindMatrixRowFor($"I_{variableName}");
                for (int j = 0; j < data.VariablesX.Count; j++)
                    data.A[i][j] = -_systemMatrix[row][j] / branch.Value;

                for (int j = 0; j < data.VariablesInput.Count; j++)
                    data.B[i][j] = -_systemMatrix[row][data.VariablesX.Count + j] / branch.Value;
            }
            else if (branch.Type == "L" && variableType == "I")
            {
                int row = FindMatrixRowFor($"U_{variableName}");
                for (int j = 0; j < data.VariablesX.Count; j++)
                    data.A[i][j] = -_systemMatrix[row][j] / branch.Value;

                for (int j = 0; j < data.VariablesInput.Count; j++)
                    data.B[i][j] = -_systemMatrix[row][data.VariablesX.Count + j] / branch.Value;
            }
        }
    }

    private void ExtractCD(SystemData data)
    {
        data.C = new Matrix(_variablesY.Length, data.VariablesX.Count);
        data.D = new Matrix(_variablesY.Length, data.VariablesInput.Count);

        for (int i = 0; i < _variablesY.Length; i++)
        {
            int stateIndex = data.VariablesX.IndexOf(_variablesY[i]);
            if (stateIndex != -1)
            {
                data.C[i][stateIndex] = 1.0;
                continue;
            }
            int row = FindMatrixRowFor(_variablesY[i]);
            if (row == -1) continue;

            for (int j = 0; j < data.VariablesX.Count; j++)
                data.C[i][j] = -_systemMatrix[row][j];

            for (int j = 0; j < data.VariablesInput.Count; j++)
                data.D[i][j] = -_systemMatrix[row][data.VariablesX.Count + j];
        }
    }

    private int FindMatrixRowFor(string variable)
    {
        int variableIndex = Array.IndexOf(_variables, variable);
        if (variableIndex == -1) return -1;
        for (int i = 0; i < _systemMatrix.Rows; i++)
            if (Math.Abs(Math.Abs(_systemMatrix[i][variableIndex]) - 1.0) < 1e-10)
                return i;
        return -1;
    }
}

