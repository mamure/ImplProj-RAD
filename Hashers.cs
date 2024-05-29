using System.Numerics;


public class Seeder() {
    private static Random rng = new();
    public static UInt128 Random(int bits) {
        if (bits < 0 || bits > 127) {
            throw new ArgumentOutOfRangeException(nameof(bits));
        }

        // Generete 1 extra byte in case of rounding
        int bytesLen = 1 + bits / 8;
        byte[] bytes = new byte[bytesLen];
        rng.NextBytes(bytes);
        
        // Remove excess bits in the extra byte
        int excessBits = bytesLen * 8 - bits;
        byte excessMask = (byte)(0xff >> excessBits);
        bytes[bytesLen - 1] &= excessMask;

        return (UInt128)new BigInteger(bytes, isUnsigned: true, isBigEndian: false);
    }
    public static UInt128 Mersenne(int q) {
        return ((UInt128)1 << q) - 1;
    }
}

public interface IHash {
    public int BitLen();
    public ulong Hash(ulong x);

    int BigBitLen() {
        return BitLen();
    }
    UInt128 BigHash(ulong x) {
        return (UInt128)Hash(x);
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
        a = (ulong)Seeder.Random(q);
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
    private readonly UInt128 p;
    private readonly UInt128 a;
    private readonly UInt128 b;

    public MultiplyModPrime(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        mask = (1ul << l) - 1;
        q = 89;
        p = Seeder.Mersenne(q);
        a = Seeder.Random(q);
        b = Seeder.Random(q);
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
    public UInt128 BigHash(ulong x) {
        // a + bx mod p
        var y = a + b * x;
        y = (y & p) + (y >> q);
        if (y >= p) y -= p;
        return y;
    }
}


public class PolynomialModPrime : IHash {
    private readonly int l;
    private readonly ulong mask;
    private readonly int q;
    private readonly UInt128 p;
    private readonly UInt128 a;
    private readonly UInt128 b;
    private readonly UInt128 c;
    private readonly UInt128 d;

    public PolynomialModPrime(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        mask = (1ul << l) - 1;
        q = 89;
        p = Seeder.Mersenne(q);
        a = Seeder.Random(q);
        b = Seeder.Random(q);
        c = Seeder.Random(q);
        d = Seeder.Random(q);
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
    public UInt128 BigHash(ulong x) {
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
