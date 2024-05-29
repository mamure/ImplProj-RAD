public class Generator {
    private Random rnd;
    private ulong seed;
    private int n;
    private int l;
    public Generator(int n , int l) {
        this.n = n;
        this.l = l;
        rnd = new();
        seed = 0;
        byte[] b = new byte[8];
        rnd.NextBytes(b);
        for (int i = 0; i < 8; i++) {
            seed = (seed << 8) + b[i];
        }
    }

    public IEnumerable<(ulong, int)> CreateStream() {
        // We demand that our random number has 30 zeros on the least
        // significant bits and then a one.
        var a = (seed | ((1ul << 31) - 1ul)) ^ ((1ul << 30) - 1ul);
        
        ulong x = 0;
        for (int i = 0; i < n/3; i++) {
            x = x + a;
            yield return (x & (((1ul << l) - 1ul) << 30), 1);
        }
        for (int i = 0; i < (n+1)/3; i++) {
            x = x + a;
            yield return (x & (((1ul << l) - 1ul) << 30), -1);
        }
        for (int i = 0; i < (n+2)/3; i++) {
            x = x + a;
            yield return (x & (((1ul << l) - 1ul) << 30), 1) ;
        }
    }
}
