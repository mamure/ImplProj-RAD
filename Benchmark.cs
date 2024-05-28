using System.Diagnostics;
using System.Globalization;


public class Benchmark {
    const int MAX_BITS = 24;
    public static void Run_1c(int size, int count, int max_bits=MAX_BITS) {
        Console.WriteLine($"Benchmarking opg 1c");

        var csv = "i,size,bits,MSH,MMP,PMP\n";
        for(int bits = 1; bits < max_bits; bits++) {
            var stream = Generate.CreateStream(size, bits);
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };

            for(int i = 0; i < count; i++) {
                csv += $"{i},{size},{bits}";
                foreach (var hasher in benches) {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
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
    public static void Run_3(int size, int count, int max_bits=MAX_BITS) {
        Console.WriteLine($"Benchmarking opg 3");

        var csv = "i,size,bits,MSH,S_MSH,MMP,S_MMP,PMP,S_PMP\n";
        for(int bits = 1; bits < MAX_BITS; bits++) {
            var stream = Generate.CreateStream(size, bits);
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };
        
            for(int i = 0; i < count; i++) {
                csv += $"{i},{size},{bits}";
                foreach (var hasher in benches) {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
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

    public static void Run_7_8(int size, int count, int max_bits=MAX_BITS) {
        Console.WriteLine($"Benchmarking opg 7/8");

        var csv = "i,size,bits,S,X,Stime,Xtime\n";
        for(int bits = 1; bits < max_bits; bits++) {
            var stream = Generate.CreateStream(size, bits);
            for(int i = 0; i < count; i++) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Console.Write($"bits = {bits}, i = {i}{new string(' ', 42)}\r");
                var hasher = new PolynomialModPrime(bits);
                var sketch = new CountSketch(hasher);
                var table = new HashTable(hasher);
                var timer = Stopwatch.StartNew();

                timer.Restart();
                var S = App.TableSquareSum(stream, table);
                var STime =  timer.Elapsed.TotalMilliseconds;
                var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                
                timer.Restart();
                var X = App.SketchSquareSum(stream, sketch);
                var XTime = timer.Elapsed.TotalMilliseconds;
                var XTimePretty = XTime.ToString(CultureInfo.InvariantCulture);

                csv += $"{i},{size},{bits},{S},{X},{STimePretty},{XTimePretty}\n";
            }
        }
        using (var file = new StreamWriter("opg_7_8.csv")) {
            file.Write(csv);
        }
    }
}
