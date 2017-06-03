

/// <summary>
/// Bit and other chess operations.
/// </summary>
public static class Ops
{

    #region Tables

    /// <summary>
    /// Power of 2 lookup table. Used to set a bit at the specific index.
    /// </summary>
    public static ulong[] Pow2 = new ulong[] {
		1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864, 134217728, 268435456, 536870912, 1073741824, 2147483648, 4294967296, 8589934592, 17179869184, 34359738368, 68719476736, 137438953472, 274877906944, 549755813888, 1099511627776, 2199023255552, 4398046511104, 8796093022208, 17592186044416, 35184372088832, 70368744177664, 140737488355328, 281474976710656, 562949953421312, 1125899906842624, 2251799813685248, 4503599627370496, 9007199254740992, 18014398509481984, 36028797018963968, 72057594037927936, 144115188075855872, 288230376151711744, 576460752303423488, 1152921504606846976, 2305843009213693952, 4611686018427387904, 9223372036854775808
	};


    private static int[] Index64 = new int[] { 
        0, 47,  1, 56, 48, 27,  2, 60, 57, 49, 41, 37, 28, 16,  3, 61, 54, 58, 35, 52, 50, 42, 21, 44, 38, 32, 29, 23, 17, 11,  4, 62, 46, 55, 26, 59, 40, 36, 15, 53, 34, 51, 20, 43, 31, 22, 10, 45, 25, 39, 14, 33, 19, 30,  9, 24, 13, 18,  8, 12,  7,  6,  5, 63
	};

    #endregion



    #region BIT OPERATIONS

    /// <summary>
    /// Returns true if there is set bit at the index in the given bitboard.
    /// </summary>
    public static bool GetBit(int index, ulong bitboard)
    {
        return ((bitboard >> index) & 1) != 0;
    }

    /// <summary>
    /// Returns index of the first set bit in the given bitboard.
    /// </summary>
    public static int FirstBit(ulong bitboard)
    {
        return Index64[((bitboard ^ (bitboard - 1)) * 0x03f79d71b4cb0a89) >> 58];
    }

    /// <summary>
    /// Returns index of the first set bit in the given bitboard and removes that bit at the same time.
    /// </summary>
    public static int PopFirstBit(ref ulong bitboard)
    {
        int MoveIndex = FirstBit(bitboard);
        bitboard ^= Pow2[MoveIndex];
        return MoveIndex;
    }

    /// <summary>
    /// Returns number of set bits in the given bitboard.
    /// </summary>
    public static int NumberOfSetBits(ulong bitboard)
    {
        bitboard = bitboard - ((bitboard >> 1) & 0x5555555555555555);
        bitboard = (bitboard & 0x3333333333333333) + ((bitboard >> 2) & 0x3333333333333333);
        return (int)((((bitboard + (bitboard >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56);
    }

    #endregion




}




