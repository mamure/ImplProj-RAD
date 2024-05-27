public class CountSketch {
    private readonly IHash hashU4;
    private int m;
    private int[] sketch;
    public CountSketch(IHash hashU4) {
        this.hashU4 = hashU4;
        m = 1 << hashU4.BitLen();
        sketch = new int[m];
    }

    private ulong h_hash(ulong x) {
        return hashU4.Hash(x);
    }
    private int s_hash(ulong x) {
        var bx = hashU4.BigHash(x) >> hashU4.BigBitLen();
        return (int)(1 - 2 * bx);
    }


    
}
