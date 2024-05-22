using System;
using System.Numerics;
using System.Globalization;


public class MultiplyModPrime<T> : IHash<T> where T : INumber<T> {
    private readonly int l;
    private readonly UInt64 mask;
    private readonly int q;
    private readonly BigInteger p;
    private readonly BigInteger a;
    private readonly BigInteger b;

    public MultiplyModPrime(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        this.mask = ((UInt64)1 << l) - 1;
        
        this.q = 89;
        this.p = (new BigInteger(1) << q) - 1;
        this.a = BigInteger.Parse("1029E3F5CB3358C17BCD08F", NumberStyles.HexNumber);
        this.b = BigInteger.Parse("1BFAB7F9A831C0BBEDD1FAA", NumberStyles.HexNumber);
    }
    public int BitLen() {
        return this.l;
    }
    public UInt64 Hash(T x) {
        var k = Convert.ToUInt64(x);
        var t = a * k + b;
        var y = (t & p) + (t >> q);
        if (y >= p) y -= p;
        return (UInt64)(y & mask);
    }
}