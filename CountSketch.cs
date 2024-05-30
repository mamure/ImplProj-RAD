public class CountSketch {
    private readonly IHash hashU4;
    private int[] sketch;
    private int m;
    private int b;
    private ulong m_temp;
    private int b_temp;
    public CountSketch(IHash hashU4) {
        this.hashU4 = hashU4;
        m = 1 << hashU4.BitLen();
        b = hashU4.BigBitLen();
        m_temp = (ulong)(m - 1);
        b_temp = b - 1;

        sketch = new int[m];
        for (int i = 0; i < m; i++){
            sketch[i] = 0;
        }
    }

    private (ulong, int) Hash_hx_sx(ulong x) {
        var gx = hashU4.BigHash(x);
        var hx = (ulong)gx & m_temp;
        var sx = 1 - 2 * (int)(gx >> b_temp);
        return (hx, sx);
    }
    public void Process(IEnumerable<(ulong, int)> stream){
        foreach (var (x, d) in stream){
            var (hx, sx) = Hash_hx_sx(x);
            sketch[hx] += sx * d;
        }
    }
    public Int128 SquareSum(){
        Int128 X = 0;
        foreach (var xi in sketch){
            var t = (Int128)xi;
            X += t * t;
        }
        return X;
    }
}
