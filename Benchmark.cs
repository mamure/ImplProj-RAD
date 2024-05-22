using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;


public class Benchmark {
    public static List<(string, float, int)> Run(int n, int l) {
        var benches = new List<(string, IHash<UInt64>)> {
            ("Multiply Mod Prime", new MultiplyModPrime<UInt64>(l)),
            ("Multiply Shift Hash", new MultiplyShiftHash<UInt64>(l)),
        };

        var store = new List<(string, float, int)>();
        foreach (var (name, hasher) in benches) {
            var stream = Generate.CreateStream(n, l);
            var table = new HashTable<UInt64, int>(hasher);

            var timer = Stopwatch.StartNew();
            foreach (var (x, v) in stream) {
                table.Increment(x, v);
            }
            var elapsed = timer.ElapsedMilliseconds;
            
            store.Add((name, (float)elapsed / 1000.0f, table[0]));
        }
        return store;
    }
    public static void RunSingle(int n, int l) {
        foreach(var (name, time, _) in Run(n, l)) {
            Console.WriteLine($"{name}: {time}s");
        }
    }
    public static void RunAll(int n) {
        for (int i = 1; i <= 24; i++) {
            try {
                Console.Write($"{i}");
                foreach(var (_, time, _) in Run(n, i)) {
                    var timestr = time.ToString(CultureInfo.InvariantCulture);
                    Console.Write($",{timestr}");
                }
                Console.WriteLine();
            } catch (System.Exception) {
                break;
            }
        }

    }
}