using System.Numerics;


public class BigRandom() {
    private static Random rng = new();
    public static BigInteger Get(int bits) {
        // Generete 1 extra byte in case of rounding
        int bytesLen = (bits + 7) / 8;
        byte[] bytes = new byte[bytesLen];
        rng.NextBytes(bytes);

        // Remove excess bits in the extra byte
        int excessBits = bytesLen * 8 - bits;
        byte excessMask = (byte)(((byte)excessBits << 1) - 1);
        bytes[bytesLen - 1] &= excessMask;

        return new BigInteger(bytes);
    }
}

public interface IHash {
    public int BitLen();
    public ulong Hash(ulong x);

    int BigBitLen() {
        return BitLen();
    }
    BigInteger BigHash(ulong x) {
        return new BigInteger(Hash(x));
    }
}




public class MultiplyShiftHash : IHash {
    private readonly int l;
    private readonly int q;
    private readonly ulong a;

    public MultiplyShiftHash(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        q = 64;
        a = (ulong)BigRandom.Get(q - 1); // TODO LOOK AT BIGRANDOM AND MAKE SURE IT STAYS WITHIN BOUNDS
        // REMOVE THE -1 IF NEEDED
    }
    public int BitLen() {
        return l;
    }
    public ulong Hash(ulong x) {
        return (a * x) >> (q - l);
    }
}


public class MultiplyModPrime : IHash {
    private readonly int l;
    private readonly ulong mask;
    private readonly int q;
    private readonly BigInteger p;
    private readonly BigInteger a;
    private readonly BigInteger b;

    public MultiplyModPrime(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        mask = (1ul << l) - 1;
        q = 89;
        p = (new BigInteger(1) << q) - 1;
        a = BigRandom.Get(q);
        b = BigRandom.Get(q);
    }
    public int BitLen() {
        return l;
    }
    public int BigBitLen() {
        return q;
    }
    public ulong Hash(ulong x) {
        // Select lower l bits
        return (ulong)(BigHash(x) & mask);
    }
    public BigInteger BigHash(ulong x) {
        // a + bx mod p
        var y = a + b*x;
        y = (y & p) + (y >> q);
        if (y >= p) y -= p;
        return y;
    }
}


public class PolynomialModPrime : IHash {
    private readonly int l;
    private readonly ulong mask;
    private readonly int q;
    private readonly BigInteger p;
    private readonly BigInteger a;
    private readonly BigInteger b;
    private readonly BigInteger c;
    private readonly BigInteger d;

    public PolynomialModPrime(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        mask = (1ul << l) - 1;
        q = 89;
        p = (new BigInteger(1) << q) - 1;
        a = BigRandom.Get(q);
        b = BigRandom.Get(q);
        c = BigRandom.Get(q);
        d = BigRandom.Get(q);
    }
    public int BitLen() {
        return l;
    }
    public int BigBitLen() {
        return q;
    }
    public ulong Hash(ulong x) {
        // select lower 64 bits
        return (ulong)(BigHash(x) & mask);
    }
    public BigInteger BigHash(ulong x) {
        // y = c + dx mod p
        var y = d * x + c;
        y = (y & p) + (y >> q);
        
        // y = b + cx + dx^2 mod p
        y = y * x + b;
        y = (y & p) + (y >> q);
        
        // y = a + bx + cx^2 + dx^3 mod p
        y = y * x + a;
        y = (y & p) + (y >> q);
        if (y >= p) y -= p;

        return y;
    }
}
