public class CountSketch {
    private int m;
    private readonly IHash hashU4;
    private int[] sketch;
    public CountSketch(IHash hashU4) {
        this.hashU4 = hashU4;
        m = 1 << hashU4.BitLen();
        sketch = new int[m];
        for (int i = 0; i < m; i++){
            sketch[i] = 0;
        }
    }

    private ulong h_hash(ulong x) {
        return hashU4.Hash(x);
    }
    private int s_hash(ulong x) {
        var bx = hashU4.BigHash(x) >> (hashU4.BigBitLen() - 1);
        return (int)(1 - 2 * bx);
    }

    public ulong CalculateEstimate(){
        ulong X = 0;
        for (int y = 0; y < m; y++){
            X += (ulong)sketch[y] * (ulong)sketch[y];
        }
        return X;
    }

    public void Process(IEnumerable<(ulong, int)> stream){
        foreach (var (x,d) in stream){
            sketch[h_hash(x)] += s_hash(x) * d;
        }
    } 
}
