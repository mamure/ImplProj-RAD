public class App {
    public static UInt128 TableSquareSum(IEnumerable<(ulong, int)> stream, HashTable table) {
        table.Process(stream);
        return table.SquareSum();
    }

    public static UInt128 SketchSquareSum(IEnumerable<(ulong, int)> stream, CountSketch sketch) {
        sketch.Process(stream);
        return sketch.SquareSum();
    }

    public static void Main() {
        var max_count = 100;
        var min_count = 5;
        var max_bits = 23;
        var cutoff = 15;
        var counts = new List<int>();
        for (int i = 0; i < max_bits; i++) {
            // Generate the amount of times each expirement is ran.
            // The first few are max_count, after cutoff they fall linearly.
            // This increaes variance for the last few, but decreases time to bench.
            if (i < cutoff) {
                counts.Add(max_count);
            } else {
                counts.Add(Math.Max(
                    max_count - (i - cutoff) * max_count / (max_bits - cutoff), 
                    min_count
                ));
            }
        }
        
        // NOTE: The size must be larger than 2^max_bits
        Benchmark.Run_1c((int)1e7, counts, max_bits);
        Benchmark.Run_3((int)1e7, counts, max_bits);
        Benchmark.Run_7_8((int)1e7, counts, max_bits);
    }
}