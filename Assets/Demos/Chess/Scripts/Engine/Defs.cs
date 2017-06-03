
/// <summary>
/// Chess board squares.
/// </summary>
public enum Squares : int{
	None = 64,
	A1 = 56, B1 = 57, C1 = 58, D1 = 59, E1 = 60, F1 = 61, G1 = 62, H1 = 63,
	A2 = 48, B2 = 49, C2 = 50, D2 = 51, E2 = 52, F2 = 53, G2 = 54, H2 = 55,
	A3 = 40, B3 = 41, C3 = 42, D3 = 43, E3 = 44, F3 = 45, G3 = 46, H3 = 47,
	A4 = 32, B4 = 33, C4 = 34, D4 = 35, E4 = 36, F4 = 37, G4 = 38, H4 = 39,
	A5 = 24, B5 = 25, C5 = 26, D5 = 27, E5 = 28, F5 = 29, G5 = 30, H5 = 31,
	A6 = 16, B6 = 17, C6 = 18, D6 = 19, E6 = 20, F6 = 21, G6 = 22, H6 = 23,
	A7 = 8, B7 = 9, C7 = 10, D7 = 11, E7 = 12, F7 = 13, G7 = 14, H7 = 15,
	A8 = 0, B8 = 1, C8 = 2, D8 = 3, E8 = 4, F8 = 5, G8 = 6, H8 = 7, 
}


/// <summary>
/// Possible promotion piece.
/// </summary>
public enum PromotePiece : int{
    Queen = Defs.PromoQueen,
    Rook = Defs.PromoRook,
    Bishop = Defs.PromoBishop,
    Knight = Defs.PromoKnight,
}


public static class Defs
{


    public static int[] Mirror64 = { //Mirrors the index
        56	,	57	,	58	,	59	,	60	,	61	,	62	,	63	,
        48	,	49	,	50	,	51	,	52	,	53	,	54	,	55	,
        40	,	41	,	42	,	43	,	44	,	45	,	46	,	47	,
        32	,	33	,	34	,	35	,	36	,	37	,	38	,	39	,
        24	,	25	,	26	,	27	,	28	,	29	,	30	,	31	,
        16	,	17	,	18	,	19	,	20	,	21	,	22	,	23	,
        8	,	9	,	10	,	11	,	12	,	13	,	14	,	15	,
        0	,	1	,	2	,	3	,	4	,	5	,	6	,	7
                                   };


    public const int MaxDepth = 64; //max depth that search can get
    public const int MaxMoves = 256; //max moves that one board position can have

    #region CastleRights

    /// <summary>
    /// Every castle right included.
    /// </summary>
    public const int CastleRightsAll = 8;

    /// <summary>
    /// Qween white castle right.
    /// </summary>
    public const int CastleRightsQWCa = 1;

    /// <summary>
    /// King white castle right.
    /// </summary>
    public const int CastleRightsKWCa = 2;

    /// <summary>
    /// Qween black castle right.
    /// </summary>
    public const int CastleRightsQBCa = 4;

    /// <summary>
    /// King black castle right.
    /// </summary>
    public const int CastleRightsKBCa = 8;

    /// <summary>
    /// No castle right included.
    /// </summary>
    public const int CastleRightsNone = 0;

    #endregion

    #region Pieces
    public const int Empty = 0;           
    public const int WPawn = 1;           
    public const int WKnight = 2;           
    public const int WBishop = 3;        
    public const int WRook = 4;        
    public const int WQueen = 5;           
    public const int WKing = 6;          
    public const int BPawn = 7;           
    public const int BKnight = 8;          
    public const int BBishop = 9;        
    public const int BRook = 10;         
    public const int BQueen = 11;          
    public const int BKing = 12;

    public const int CastleMove = 2;
    public const int PromoQueen = 7;
    public const int PromoRook = 6;
    public const int PromoBishop = 5;
    public const int PromoKnight = 4;

    public const int White = 1;
    public const int Black = 0;

    #endregion
}


