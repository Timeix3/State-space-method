
using ScottPlot;

internal class Drawer
{
    private string filePath;
    public Drawer(string filePath) => this.filePath = filePath;
    public void DrawToFile(Matrix Ydata, double[] time)
    {
        var plt = new Plot();
        for (int j = 0; j < Ydata.Columns; j++)
        {
            var Yj = plt.Add.ScatterLine(time, Ydata.GetColumn(j).ToArray());
            Yj.LegendText = $"Y{j + 1}(t)";
        }
        plt.Title("Компоненты вектора Y(t)");
        plt.XLabel("Время, с");
        plt.YLabel("Значение Y");
        plt.ShowLegend();
        plt.ShowGrid();
        plt.Save(filePath, 800, 600);
    }
}
