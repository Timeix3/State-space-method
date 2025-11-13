
internal class SystemData
{
    public Matrix A { get; set; }
    public Matrix B { get; set; }
    public Matrix C { get; set; }
    public Matrix D { get; set; }
    public Vector X { get; set; }
    public Vector V { get; set; }
    public List<string> VariablesX { get; set; } = new();
    public List<string> VariablesInput { get; set; } = new();

    public void Print()
    {
        Console.WriteLine("A:"); A.Print();
        Console.WriteLine("B:"); B.Print();
        Console.WriteLine("C:"); C.Print();
        Console.WriteLine("D:"); D.Print();
        Console.WriteLine("X:"); X.Print();
        Console.WriteLine("V:"); V.Print();
    }
}

