using System.Diagnostics;
using System.Globalization;


public class Benchmark {
    const int MAX_BITS = 31;
    public static void Run_1c(int size, int count, int max_bits=MAX_BITS) {
        Console.WriteLine($"Benchmarking opg 1c");

        var csv = "i,size,bits,MSH,MMP,PMP\n";
        var stream = Generate.CreateStream(size, 63);
        for(int bits = 1; bits <= max_bits; bits++) {
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };

            for(int i = 0; i < count; i++) {
                csv += $"{i},{size},{bits}";
                foreach (var hasher in benches) {
                    Console.Write($"bits = {bits}, i = {i}{new String(' ', 42)}\r");
                    ulong sum = 0;

                    var timer = Stopwatch.StartNew();
                    foreach (var (x, v) in stream) {
                        sum += hasher.Hash(x);
                    }
                    var STime =  (double)timer.ElapsedMilliseconds / 1000;
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

        var csv = "i,size,bits,MSH,MMP,PMP\n";
        var stream = Generate.CreateStream(size, 63);
        for(int bits = 1; bits <= max_bits; bits++) {
            var benches = new List<IHash> {
                new MultiplyShiftHash(bits),
                new MultiplyModPrime(bits),
                new PolynomialModPrime(bits),
            };

            for(int i = 0; i < count; i++) {
                csv += $"{i},{size},{bits}";
                foreach (var hasher in benches) {
                    Console.Write($"bits = {bits}, i = {i}{new String(' ', 42)}\r");
                    var table = new HashTable(hasher);

                    var timer = Stopwatch.StartNew();
                    var S = App.TableSquareSum(stream, table);
                    var STime =  (double)timer.ElapsedMilliseconds / 1000;
                    var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                    csv += $",{STimePretty}";
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
        var stream = Generate.CreateStream(size, 63);
        for(int bits = 1; bits <= max_bits; bits++) {
            for(int i = 0; i < count; i++) {
                Console.Write($"bits = {bits}, i = {i}{new String(' ', 42)}\r");
                var hasher = new PolynomialModPrime(bits);
                var sketch = new CountSketch(hasher);
                var table = new HashTable(hasher);

                var timer = Stopwatch.StartNew();
                var S = App.TableSquareSum(stream, table);
                var STime =  (double)timer.ElapsedMilliseconds / 1000;
                var STimePretty = STime.ToString(CultureInfo.InvariantCulture);
                
                timer = Stopwatch.StartNew();
                var X = App.SketchSquareSum(stream, sketch);
                var XTime =  (double)timer.ElapsedMilliseconds / 1000;
                var XTimePretty = XTime.ToString(CultureInfo.InvariantCulture);

                csv += $"{i},{size},{bits},{S},{X},{STimePretty},{XTimePretty}\n";
            }
        }
        using (var file = new StreamWriter("opg_7_8.csv")) {
            file.Write(csv);
        }
    }
}