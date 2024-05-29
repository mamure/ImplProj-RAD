using System.Diagnostics;
using System.Globalization;


public class Benchmark {
    public static void Run_1c(int size, List<int> counts, int max_bits) {
        if (counts.Count < max_bits) {
            throw new ArgumentException(nameof(counts));
        }

        Console.WriteLine($"Benchmarking opg 1c");
        var csv = "bits,i,size,MSH,MMP,PMP\n";
        for(int bits = 1; bits <= max_bits; bits++) {
            var count = counts[bits - 1];
            var gen = new Generator(size, bits);
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };

            for(int i = 0; i < count; i++) {
                Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
                csv += $"{bits},{i},{size}";
                foreach (var hasher in benches) {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var stream = gen.CreateStream();
                    ulong sum = 0;

                    var timer = Stopwatch.StartNew();
                    foreach (var (x, v) in stream) {
                        sum += hasher.Hash(x);
                    }
                    var STime = timer.Elapsed.TotalMilliseconds;
                    var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                    csv += $",{STimePretty}";
                }
                csv += "\n";
            }
        }
        using (var file = new StreamWriter("opg_1c.csv")) {
            file.Write(csv);
        }
    }
    public static void Run_3(int size, List<int> counts, int max_bits) {
        if (counts.Count < max_bits) {
            throw new ArgumentException(nameof(counts));
        }

        Console.WriteLine($"Benchmarking opg 3");
        var csv = "bits,i,size,MSH,S_MSH,MMP,S_MMP,PMP,S_PMP\n";
        for(int bits = 1; bits <= max_bits; bits++) {
            var count = counts[bits - 1];
            var gen = new Generator(size, bits);
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };
        
            for(int i = 0; i < count; i++) {
                Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
                csv += $"{bits},{i},{size}";
                foreach (var hasher in benches) {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    var stream = gen.CreateStream();
                    var table = new HashTable(hasher);

                    var timer = Stopwatch.StartNew();
                    var S = App.TableSquareSum(stream, table);
                    var STime = timer.Elapsed.TotalMilliseconds;
                    var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                    csv += $",{STimePretty},{S}";
                }
                csv += "\n";
            }
        }
        using (var file = new StreamWriter("opg_3.csv")) {
            file.Write(csv);
        }
    }

    public static void Run_7_8(int size, List<int> counts, int max_bits) {
        if (counts.Count < max_bits) {
            throw new ArgumentException(nameof(counts));
        }

        Console.WriteLine($"Benchmarking opg 7/8");
        var csv = "bits,i,size,S,X,Stime,Xtime\n";
        for(int bits = 1; bits <= max_bits; bits++) {
            var count = counts[bits - 1];
            var gen = new Generator(size, bits);
            for(int i = 0; i < count; i++) {
                Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                var stream_a = gen.CreateStream();
                var stream_b = gen.CreateStream();
                var hasher = new PolynomialModPrime(bits);
                var sketch = new CountSketch(hasher);
                var table = new HashTable(hasher);
                var timer = Stopwatch.StartNew();

                timer.Restart();
                var S = App.TableSquareSum(stream_a, table);
                var STime =  timer.Elapsed.TotalMilliseconds;
                var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                
                timer.Restart();
                var X = App.SketchSquareSum(stream_b, sketch);
                var XTime = timer.Elapsed.TotalMilliseconds;
                var XTimePretty = XTime.ToString(CultureInfo.InvariantCulture);

                csv += $"{bits},{i},{size},{S},{X},{STimePretty},{XTimePretty}\n";
            }
        }
        using (var file = new StreamWriter("opg_7_8.csv")) {
            file.Write(csv);
        }
    }
}
