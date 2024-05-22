using System;
using System.Numerics;
using System.Collections.Generic;


public interface IHash<T> where T : INumber<T> {
    public int BitLen();
    public UInt64 Hash(T x);
}


public class HashTable<K, V>
    where K : INumber<K>
    where V : INumber<V>
 {
    private LinkedList<(K, V)>[] buckets;
    private IHash<K> hasher;

    public HashTable(IHash<K> hasher) {
        this.hasher = hasher;
        this.buckets = new LinkedList<(K, V)>[(UInt64)1 << hasher.BitLen()];
        for (int i = 0; i < buckets.Length; i++) {
            buckets[i] = new LinkedList<(K, V)>();
        }
    }

    public V? this[K x] {
        get { return Get(x); }
        set { Set(x, value!); }
    }

    public V? Get(K x) {
        UInt64 index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (_, value) = node.Value;
            return value;
        }
        return default(V?);
    }

    public void Set(K x, V v) {
        UInt64 index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (key, _) = node.Value;
            node.Value = (key, v);
        } else {
            buckets[index].AddLast((x, v));
        }
    }

    public V? Pop(K x) {
        UInt64 index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (_, value) = node.Value;
            buckets[index].Remove(node);
            return value;
        }
        return default(V?);
    }

    public void Increment(K x, V d) {
        UInt64 index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (key, value) = node.Value;
            node.Value = (key, value + d);
        } else {
            buckets[index].AddLast((x, d));
        }
    }


    private LinkedListNode<(K, V)>? Find(K x, UInt64 index) {
        var current = buckets[index].First;
        while (current != null) {
            if (current.Value.Item1 == x) {
                return current;
            }
            current = current.Next;
        }
        return null;
    }
}