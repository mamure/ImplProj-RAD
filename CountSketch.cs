public class CountSketch {
    private int m;
    private int b;
    private readonly IHash hashU4;
    private int[] sketch;
    public CountSketch(IHash hashU4) {
        this.hashU4 = hashU4;
        b = hashU4.BigBitLen();
        m = 1 << hashU4.BitLen();
        sketch = new int[m];
        for (int i = 0; i < m; i++){
            sketch[i] = 0;
        }
    }

    private (ulong, int) Hash_hx_sx(ulong x) {
        var gx = hashU4.BigHash(x);
        var hx = (ulong)(gx & (m - 1));
        var sx = (int)(1 - 2 * (gx >> (b - 1)));
        return (hx, sx);
    }
    public void Process(IEnumerable<(ulong, int)> stream){
        foreach (var (x, d) in stream){
            var (hx, sx) = Hash_hx_sx(x);
            sketch[hx] += sx * d;
        }
    }
    public ulong SquareSum(){
        ulong X = 0;
        foreach (var xi in sketch){
            X += (ulong)(xi * xi);
        }
        return X;
    }
}
