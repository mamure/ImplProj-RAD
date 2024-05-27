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

    public static void Run_7(int n) {
        Console.WriteLine($"Evaluating Count-Sketch of datastream (opg 7)");
        Console.WriteLine($"Evaluating Estimator (opg 7)");
        Console.WriteLine($"Evaluating MSE over 100 eksperiments (opg 7)");
        Console.WriteLine($"Evaluating Median over groups (opg 7)");
        double[] test = {}; // Placeholder
        double[] Mi = new double[9];

        for (int i = 1; i < 9; i++) {
            double[] Gi = test.Skip((i - 1) * 11).Take(11).ToArray();
            Array.Sort(Gi);

            int len = Gi.Length;
            if (len % 2 == 0) {
                Mi[i] = (Gi[len / 2 - 1] + Gi[len / 2]) / 2.0;
            } else {
                Mi[i] = Gi[len / 2];
            }
        }
        Array.Sort(Mi);
        double[] x = [1,2,3,4,5,6,7,8,9];
        double[] y = Mi;
        for (int i = 1; i < 9; i++) {
            Console.WriteLine($"{x[i]}, {y[i]}");
        }
    }

    public static void Run_8(int n) {
        Console.WriteLine($"Benchmarking value of m (opg 8)");
        Console.WriteLine($"Benchmarking value of m in Count-Sketch(opg 8)");
    }
}