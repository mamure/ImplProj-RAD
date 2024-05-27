public class App {
    public static int Main(String[] args) {
        var benches = new List<string>(); 
        var bench_n = (int)1e6; 
        foreach (var item in args) {
            var lower = item.ToLower().Trim();
            if (lower.StartsWith("-b=")) {
                var sub = lower.Substring(3);
                if (sub.StartsWith("\"") && sub.EndsWith("\"")) {
                    sub.Substring(1, -1);
                }
                foreach (var part in sub.Split(",")) {
                    benches.Add(part.Trim());
                }
            } else if (lower.StartsWith("-n=")) {
                var sub = lower.Substring(3).Trim();
                var result = 0;
                if (int.TryParse(sub, out result)) {
                    bench_n = result;
                } else {
                    Console.WriteLine("Invalid argument for -n. Please give valid integer.");
                    return -1;
                }
            }
        }
        foreach (var bench in benches) {
            if (bench == "1c") {
                Benchmark.Run_1c(bench_n);
            } else if (bench == "3") {
                Benchmark.Run_3(bench_n);
            } else {
                Console.WriteLine($"Invalid benchmark {bench}");
                return -1;
            }
        }

        var stream = Generate.CreateStream(1000, 10);
        var hasher = new PolynomialModPrime(12);
        var sketch = new CountSketch(hasher);
        sketch.Process(stream);
        Console.WriteLine(sketch.CalculateEstimate());
        var table = new HashTable(hasher);
        Console.WriteLine(SquareSum(stream, table));



        return 0;
    }

    public static long SquareSum(IEnumerable<(ulong, int)> stream, HashTable table) {
        var temp = new List<ulong>();
        foreach (var (x, v) in stream) {
            table.Increment(x, v);
            temp.Add(x);
        }

        long S = 0;
        foreach (var bucket in table.buckets) {
            foreach(var (_, s) in bucket) {
                S += s * s;
            }
        }
        return S;
    }
}