
internal class EulerSolver
{
    public Matrix Ydata;
    public Matrix Xdata;
    public double[] time;

    public Vector Solve(SystemData eulerData, double tEnd = 10.0, double h = 0.01)
    {
        int steps = (int)(tEnd / h);
        time = new double[steps];
        Ydata = new Matrix(steps, eulerData.C.Rows);
        Xdata = new Matrix(steps, eulerData.VariablesX.Count);
        double t = 0.0;
        Vector X = eulerData.X;
        for (int i = 0; i < steps; i++)
        {
            Vector Y = eulerData.C * X + eulerData.D * eulerData.V;
            Vector dXdt = eulerData.A * X + eulerData.B * eulerData.V;
            X += h * dXdt;

            time[i] = t;
            Ydata[i] = Y;
            Xdata[i] = X;
            t += h;
        }
        return Ydata[steps - 1];
    }
}

