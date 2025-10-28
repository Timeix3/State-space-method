
internal class EulerSolver
{
    public Matrix Ydata;
    public double[] time;

    public Vector Solve(EulerData eulerData, double tEnd = 10.0, double h = 0.01)
    {
        int steps = (int)(tEnd / h);
        time = new double[steps];
        Ydata = new Matrix(steps, eulerData.C.Rows);
        double t = 0.0;
        for (int i = 0; i < steps; i++)
        {
            Vector Y = eulerData.C * eulerData.X + eulerData.D * eulerData.V;
            Vector dXdt = eulerData.A * eulerData.X + eulerData.B * eulerData.V;
            eulerData.X += h * dXdt;

            time[i] = t;
            Ydata[i] = Y;
            t += h;
        }
        return Ydata[steps - 1];
    }
}

