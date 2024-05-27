public class HashTable {
    public LinkedList<(ulong, int)>[] buckets;
    private IHash hasher;

    public HashTable(IHash hasher) {
        this.hasher = hasher;
        buckets = new LinkedList<(ulong, int)>[(ulong)1 << hasher.BitLen()];
        for (int i = 0; i < buckets.Length; i++) {
            buckets[i] = new();
        }
    }

    public int? this[ulong x] {
        get { return Get(x); }
        set { Set(x, (int)value!); }
    }


    public int? Get(ulong x) {
        ulong index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (_, value) = node.Value;
            return value;
        }
        return null;
    }

    public void Set(ulong x, int v) {
        ulong index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (key, _) = node.Value;
            node.Value = (key, v);
        } else {
            buckets[index].AddLast((x, v));
        }
    }

    public int? Pop(ulong x) {
        ulong index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (_, value) = node.Value;
            buckets[index].Remove(node);
            return value;
        }
        return null;
    }

    public void Increment(ulong x, int d) {
        ulong index = hasher.Hash(x);
        var node = Find(x, index);
        if (node != null) {
            var (key, value) = node.Value;
            node.Value = (key, value + d);
        } else {
            buckets[index].AddLast((x, d));
        }
    }


    private LinkedListNode<(ulong, int)>? Find(ulong x, ulong index) {
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
