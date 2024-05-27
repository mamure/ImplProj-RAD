using System.Numerics;
using System.Globalization;


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
    private readonly ulong a;

    public MultiplyShiftHash(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        a = 0x5b23e0fb16ba8143;
    }
    public int BitLen() {
        return l;
    }
    public ulong Hash(ulong x) {
        return (a * x) >> (64 - l);
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
        a = BigInteger.Parse("1BFAB7F9A831C0BBEDD1FAA", NumberStyles.HexNumber);
        b = BigInteger.Parse("1029E3F5CB3358C17BCD08F", NumberStyles.HexNumber);
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
        a = BigInteger.Parse("0826B0099616EA28858AFF6", NumberStyles.HexNumber);
        b = BigInteger.Parse("1A42CCF11B6E739098548E8", NumberStyles.HexNumber);
        c = BigInteger.Parse("0C04F304420CC92641A42B3", NumberStyles.HexNumber);
        d = BigInteger.Parse("0A5A5F338A0EE164C3E9C5E", NumberStyles.HexNumber);
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
        y += (y & p) + (y >> q);
        
        // y = a + bx + cx^2 + dx^3 mod p
        y = y * x + a;
        y += (y & p) + (y >> q);
        if (y >= p) y -= p;

        return y;
    }
}
