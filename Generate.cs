public class Generate {
    public static IEnumerable<(ulong, int)> CreateStream(int n , int l) {
        // We generate a random uint64 number.
        Random rnd = new();
        ulong a = 0;
        byte[] b = new byte[8];
        rnd.NextBytes(b);
        for (int i = 0; i < 8; i++) {
            a = (a << 8) + (ulong)b[i];
        }

        // We demand that our random number has 30 zeros on the least
        // significant bits and then a one.
        a = (a | ((1ul << 31) - 1ul)) ^ ((1ul << 30) - 1ul);
        
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
