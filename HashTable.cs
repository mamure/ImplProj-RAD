using System;
using System.Collections.Generic;

public class HashTabel {
    private LinkedList<(ulong, T)>[] buckets;
    private readonly int size;
    private IHash hashFunction;

    public HashTabel(int l, Func<ulong, int> hashFunction) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l))
        }
        this.size = 1 << l;
        this.hashFunction = hashFunction;
        this.buckets = new LinkedList<(ulong, T)>[1 << l];
        for (int i = 0; i < buckets.Length; i++) {
            buckets[i] = new LinkedList<(ulong, T)>();
        }
    }

    public T get(ulong x) {
        int index = (int)hashFunction.Hash(x);
        foreach ((ulong key, T value) in buckets[index]) {
            if (key == x) {
                return value;
            }
        }
        return default(T);
    }

    public long set(ulong x, T v) {
        int index = (int)hashFunction.Hash(x);
        for (var node = buckets[index].First; node != null; node = node.Next) {
            if (node.Value.Item1 == x) {
                node.Value = (x, v);
                return;
            }
        }
        buckets[index].AddLast((x, v));
    }

    public long increment(ulong x, long d) {
        int index = (int)hashFunction.Hash(x);
        for (var node = buckets[index].First; node != null; node = node.Next) {
            if (node.Value.Item1 == x) {
                node.Value = (x, node.Value.Item2 + d);
                return;
            }
        }
        buckets[index].AddLast((x, d));
    }
}