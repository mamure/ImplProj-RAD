using System.Diagnostics;
using System.Globalization;


public class Benchmark {
    const int MAX_ITERS = 31;
    private static void Run(int n, string path, Func<int, IHash, (long?, ulong)> func) {
        var csv = "l,n,msh,mmp,pmp\n";
        var store = new List<ulong>();
        for (int i = 1; i <= MAX_ITERS; i++) {
            Console.Write($"l = {i}\r");
            var benches = new List<IHash> {
                new MultiplyShiftHash(i),
                new MultiplyModPrime(i),
                new PolynomialModPrime(i),
            };

            csv += $"{i},{n}";
            foreach (var hasher in benches) {
                ulong temp = 0;
                long? elapsed = null;
                try {
                    (elapsed, temp) = func(i, hasher);
                } catch (Exception) { }
                
                if (elapsed == null) {
                    csv = csv.Substring(0, csv.LastIndexOf("\n"));
                    i = MAX_ITERS;
                    break;
                }
                var timestr = ((double)elapsed / 1000.0f).ToString(CultureInfo.InvariantCulture);
                store.Add(temp);
                csv += $",{timestr}";
            }
            csv += "\n";
        }
        using (var file = new StreamWriter(path)) {
            file.Write(csv);
        }
        using (var file = new StreamWriter("temp_to_avoid_optim_in_bench.bin")) {
            file.Write(string.Join(" ", store));
        }
    }

    public static void Run_1c(int n) {
        Console.WriteLine($"Benchmarking sum (opg 1c)");
        Run(n, "opg_1c.csv", (i, hasher) => {
            var stream = Generate.CreateStream(n, i);
            ulong sum = 0;

            var timer = Stopwatch.StartNew();
            foreach (var (x, v) in stream) {
                sum += hasher.Hash(x);
            }
            return (timer.ElapsedMilliseconds, (ulong)sum);
        });
    }
    public static void Run_3(int n) {
        Console.WriteLine($"Benchmarking squared sum (opg 3)");
        Run(n, "opg_3.csv", (i, hasher) => {
            if ((1 << i) >= n) {
                return (null, 0);
            }
            var stream = Generate.CreateStream(n, i);
            var table = new HashTable(hasher);

            var timer = Stopwatch.StartNew();
            var sum = App.SquareSum(stream, table);
            return (timer.ElapsedMilliseconds, (ulong)sum);
        });
    }
}