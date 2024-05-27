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
        Benchmark.Run_1c(1000, 10, 10);
        Benchmark.Run_3(1000, 10, 10);
        Benchmark.Run_7_8(1000, 10, 10);
    }
}