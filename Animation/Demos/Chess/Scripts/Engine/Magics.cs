

public static class Magics
{

    #region Tables

    /// <summary>
    /// Rook attack - occupancy on specific index.
    /// </summary>
    public static ulong[] OccupancyMaskRook = {
		0x101010101017eL, 0x202020202027cL, 0x404040404047aL, 0x8080808080876L, 0x1010101010106eL, 0x2020202020205eL, 0x4040404040403eL, 0x8080808080807eL, 0x1010101017e00L, 0x2020202027c00L, 0x4040404047a00L, 0x8080808087600L, 0x10101010106e00L, 0x20202020205e00L, 0x40404040403e00L, 0x80808080807e00L, 0x10101017e0100L, 0x20202027c0200L, 0x40404047a0400L, 0x8080808760800L, 0x101010106e1000L, 0x202020205e2000L, 0x404040403e4000L, 0x808080807e8000L, 0x101017e010100L, 0x202027c020200L, 0x404047a040400L, 0x8080876080800L, 0x1010106e101000L, 0x2020205e202000L, 0x4040403e404000L, 0x8080807e808000L, 0x1017e01010100L, 0x2027c02020200L, 0x4047a04040400L, 0x8087608080800L, 0x10106e10101000L, 0x20205e20202000L, 0x40403e40404000L, 0x80807e80808000L, 0x17e0101010100L, 0x27c0202020200L, 0x47a0404040400L, 0x8760808080800L, 0x106e1010101000L, 0x205e2020202000L, 0x403e4040404000L, 0x807e8080808000L, 0x7e010101010100L, 0x7c020202020200L, 0x7a040404040400L, 0x76080808080800L, 0x6e101010101000L, 0x5e202020202000L, 0x3e404040404000L, 0x7e808080808000L, 0x7e01010101010100L, 0x7c02020202020200L, 0x7a04040404040400L, 0x7608080808080800L, 0x6e10101010101000L, 0x5e20202020202000L, 0x3e40404040404000L, 0x7e80808080808000L 
	};

    /// <summary>
    /// Bishop attack - occupancy on specific index.
    /// </summary>
    public static ulong[] OccupancyMaskBishop = {
		0x40201008040200L, 0x402010080400L, 0x4020100a00L, 0x40221400L, 0x2442800L, 0x204085000L, 0x20408102000L, 0x2040810204000L, 0x20100804020000L, 0x40201008040000L, 0x4020100a0000L, 0x4022140000L, 0x244280000L, 0x20408500000L, 0x2040810200000L, 0x4081020400000L, 0x10080402000200L, 0x20100804000400L, 0x4020100a000a00L, 0x402214001400L, 0x24428002800L, 0x2040850005000L, 0x4081020002000L, 0x8102040004000L, 0x8040200020400L, 0x10080400040800L, 0x20100a000a1000L, 0x40221400142200L, 0x2442800284400L, 0x4085000500800L, 0x8102000201000L, 0x10204000402000L, 0x4020002040800L, 0x8040004081000L, 0x100a000a102000L, 0x22140014224000L, 0x44280028440200L, 0x8500050080400L, 0x10200020100800L, 0x20400040201000L, 0x2000204081000L, 0x4000408102000L, 0xa000a10204000L, 0x14001422400000L, 0x28002844020000L, 0x50005008040200L, 0x20002010080400L, 0x40004020100800L, 0x20408102000L, 0x40810204000L, 0xa1020400000L, 0x142240000000L, 0x284402000000L, 0x500804020000L, 0x201008040200L, 0x402010080400L, 0x2040810204000L, 0x4081020400000L, 0xa102040000000L, 0x14224000000000L, 0x28440200000000L, 0x50080402000000L, 0x20100804020000L, 0x40201008040200L     
	};

    /// <summary>
    /// 64 - Number of occupied bits on specific index.
    /// </summary>
    public static int[] MagicNumberShiftsRook = {
        52,53,53,53,53,53,53,52,53,54,54,54,54,54,54,53,
        53,54,54,54,54,54,54,53,53,54,54,54,54,54,54,53,
        53,54,54,54,54,54,54,53,53,54,54,54,54,54,54,53,
        53,54,54,54,54,54,54,53,52,53,53,53,53,53,53,52
    };

    /// <summary>
    /// 64 - Number of occupied bits on specific index.
    /// </summary>
    public static int[] MagicNumberShiftsBishop = {
        58,59,59,59,59,59,59,58,59,59,59,59,59,59,59,59,
        59,59,57,57,57,57,59,59,59,59,57,55,55,57,59,59,
        59,59,57,55,55,57,59,59,59,59,57,57,57,57,59,59,
        59,59,59,59,59,59,59,59,58,59,59,59,59,59,59,58
    };

    /// <summary>
    /// Magic numbers for rook, that will produce attack index.
    /// </summary>
    public static ulong[] MagicNumberRook = {
        1188950371419062305, 8106497196332941440, 4719790002207227913, 180161579437260932, 5044033783020664864, 72061992218788096, 5332280105345483784, 72058693551685698, 2392538375815201, 4620834230050488450, 140806209929344, 9304578117525835776, 1162069991239975936, 9224638682844364928, 162692572576317952, 2814758495683597, 36070028725979268, 2310630283917000832, 5045176174796488960, 565149111959584, 2026761119695111168, 9223380838318605328, 3242596129887617424, 9009398282223892, 492617716499490, 3467772267142856704, 577096278616315008, 2305851951336669698, 2305851807454462081, 73746452489177092, 2317448371625136264, 9800980687887745157, 36029071905267840, 2314885461564194880, 2310355422116053024, 4644405843593216, 74309668796891168, 4400202383872, 3519605507113480, 1443403960315543684, 14555704366555234304, 18014535956856900, 576495936960790592, 281543696777248, 1125934275133444, 281492156776488, 9288682836131844, 1152921781699477505, 6018016580699456000, 9439615256434311232, 600367997944320, 9223584388628386304, 281509337662720, 72059795208929408, 2817632846848, 4686417629039057024, 11529392180200243201, 2307041477173190786, 1459342203544994881, 9944546133185449985, 1153484489457927170, 9223935553879146754, 37386214965508, 576463518266048770                                        
    };

    /// <summary>
    /// Magic numbers for bishop, that will produce attack index.
    /// </summary>
    public static ulong[] MagicNumberBishop = {
        1170972209615897472, 9223937332296679424, 2310365301763473664, 73238203238187520, 324824390867091458, 9836147601004036104, 1198811613503504, 72075495495704586, 17614621639680, 162694804617334912, 1161937509148532736, 1209541990874620036, 2207890114848, 564067182051328, 2927342098615255057, 2207630230048, 1125934535288832, 10698853931295746, 58548033197450400, 2322185804843072, 1442313042380529664, 565075979075875, 585608693408669760, 581037470008231936, 2882876332466835489, 307919359854848, 621540729847578656, 90076407778050080, 281543712981009, 13794475113464074, 36592024006447104, 1275571279824944, 9299953093404266688, 3514522956079075332, 571754905075744, 215521459438096, 9225626052888891648, 4509260394807360, 82191406180894756, 19140575441846408, 5842367219807290372, 9808911474385489952, 9369176144718332928, 9817847600387201280, 13633952782764048, 40708352868626496, 1522806090272637952, 45634697685893248, 13841251124466159616, 2323270353291300, 5773614868335722560, 382806038153396224, 72057870073741328, 4621857755920105472, 4902177300397703688, 9224093321348284417, 4666294367429395650, 35476439828514, 9225624387532132368, 4629700434188173824, 9223374245810276480, 5085326213252, 90076407911285760, 2315009784228873344
    };

    //Attack Database
    public static ulong[][] MagicMoveRook;
    public static ulong[][] MagicMoveBishop;

    #endregion


    #region Methods

    /// <summary>
    /// Rook attacks on a given square with occupancy bitboard.
    /// </summary>
    public static ulong RookAttack(int sq, ulong block)
    {

        ulong result = 0;
        int rank = sq / 8, file = sq % 8;

        for (int r = rank + 1; r <= 7; r++)
        {
            result |= (1UL << (file + r * 8));
            if ((block & (1UL << (file + r * 8))) != 0) break;
        }

        for (int r = rank - 1; r >= 0; r--)
        {
            result |= (1UL << (file + r * 8));
            if ((block & (1UL << (file + r * 8))) != 0) break;
        }

        for (int f = file + 1; f <= 7; f++)
        {
            result |= (1UL << (f + rank * 8));
            if ((block & (1UL << (f + rank * 8))) != 0) break;
        }

        for (int f = file - 1; f >= 0; f--)
        {
            result |= (1UL << (f + rank * 8));
            if ((block & (1UL << (f + rank * 8))) != 0) break;
        }

        return result;

    }


    /// <summary>
    /// Bishop attacks on a given square with occupancy bitboard.
    /// </summary>
    public static ulong Bishopattack(int sq, ulong block)
    {
        ulong result = 0UL;
        int rk = sq / 8, fl = sq % 8, r, f;
        for (r = rk + 1, f = fl + 1; r <= 7 && f <= 7; r++, f++)
        {
            result |= (1UL << (f + r * 8));
            if ((block & (1UL << (f + r * 8))) != 0) break;
        }
        for (r = rk + 1, f = fl - 1; r <= 7 && f >= 0; r++, f--)
        {
            result |= (1UL << (f + r * 8));
            if ((block & (1UL << (f + r * 8))) != 0) break;
        }
        for (r = rk - 1, f = fl + 1; r >= 0 && f <= 7; r--, f++)
        {
            result |= (1UL << (f + r * 8));
            if ((block & (1UL << (f + r * 8))) != 0) break;
        }
        for (r = rk - 1, f = fl - 1; r >= 0 && f >= 0; r--, f--)
        {
            result |= (1UL << (f + r * 8));
            if ((block & (1UL << (f + r * 8))) != 0) break;
        }
        return result;
    }


    /// <summary>
    /// Initialize attack database.
    /// </summary>
    public static void Init()
    {

        MagicMoveRook = new ulong[64][];
        MagicMoveBishop = new ulong[64][];

        for (int i = 0; i < 64; i++)
        {
            MagicMoveRook[i] = new ulong[4096];
            MagicMoveBishop[i] = new ulong[4096];

            GenerateAttackDatabase(i, MagicNumberRook[i], MagicNumberBishop[i], ref MagicMoveRook[i], ref MagicMoveBishop[i]);

        }
    }


    public static void GenerateAttackDatabase(int sq, ulong magicRook, ulong magicBishop, ref ulong[] rook, ref ulong[] bishop)
    {

        //B - database, A - attacks
        ulong mask = OccupancyMaskRook[sq];

        int n = Ops.NumberOfSetBits(mask);

        for (int i = 0; i < (1 << n); i++)
        {
            ulong block = IndexToBitboard(i, n, mask);
            int j = MagicIndex(block, magicRook, n);
            rook[j] = RookAttack(sq, block);

        }

        mask = OccupancyMaskBishop[sq];

        n = Ops.NumberOfSetBits(mask);

        for (int i = 0; i < (1 << n); i++)
        {
            ulong block = IndexToBitboard(i, n, mask);
            int j = MagicIndex(block, magicBishop, n);
            bishop[j] = Bishopattack(sq, block);

        }


    }


    private static ulong IndexToBitboard(int index, int bits, ulong m)
    {
        int i, j;
        ulong result = 0UL;
        for (i = 0; i < bits; i++)
        {
            j = Ops.PopFirstBit(ref m);
            if ((index & (1 << i)) != 0) result |= (1UL << j);
        }
        return result;
    }


    private static int MagicIndex(ulong b, ulong magic, int bits)
    {
        return (int)((b * magic) >> (64 - bits));
    }


    #endregion

}

