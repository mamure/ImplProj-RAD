using System;
using System.Numerics;

public class MultiplyShiftHash<T> : IHash<T> where T : INumber<T> {
    private readonly int l;
    private readonly UInt64 a;

    public MultiplyShiftHash(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l));
        }
        this.l = l;
        this.a = 0x5b23e0fb16ba8143;
    }
    public int BitLen() {
        return this.l;
    }
    public UInt64 Hash(T x) {
        UInt64 k = a * Convert.ToUInt64(x);
        return (a * k) >> (64 - l);
    }
}