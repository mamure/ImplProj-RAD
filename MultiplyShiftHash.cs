using System;

public class MultiplyShiftHash {
    private readonly ulong a;
    private readonly int l;

    // Initialize a and l
    public MultiplyShiftHash(int l) {
        if (l <= 0 || l >= 64) {
            throw new ArgumentOutOfRangeException(nameof(l))
        }

        this.l = l;
        this.a = GenerateRandomOdd64();
    }

    private static GenerateRandomOdd64() {
        Random random = new Random();
        byte[] bytes = new byte[8];
        random.NextBytes(byte);
        bytes[7] |= 1; // Last bit is 1 to ensure uneven
        return BitConverter.ToUInt64(bytes, 0);
    }

    public ulong Hash(ulong x) {
        return (a * x) >> (64 - l);
    }
}