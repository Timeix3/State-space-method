using ScottPlot;

internal class Drawer
{
    private string _filePath;
    public Drawer(string filePath) => _filePath = filePath;
    public void DrawToFile(Matrix data, double[] time, string[] variables)
    {
        var plt = new Plot();
        for (int j = 0; j < data.Columns; j++)
        {
            var curve = plt.Add.ScatterLine(time, data.GetColumn(j).ToArray());
            curve.LegendText = variables[j];
        }
        plt.XLabel("Время");
        plt.YLabel("Значение");
        plt.ShowLegend();
        plt.ShowGrid();
        plt.Save(_filePath, 800, 600);
    }
}
