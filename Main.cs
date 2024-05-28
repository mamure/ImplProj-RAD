public class App {
    public static ulong TableSquareSum(IEnumerable<(ulong, int)> stream, HashTable table) {
        table.Process(stream);
        return table.SquareSum();
    }

    public static ulong SketchSquareSum(IEnumerable<(ulong, int)> stream, CountSketch sketch) {
        sketch.Process(stream);
        return sketch.SquareSum();
    }

    public static void Main() {
        Benchmark.Run_1c((int)1e7, 100);
        Benchmark.Run_3((int)1e7, 100);
        Benchmark.Run_7_8((int)1e7, 100);
    }
}