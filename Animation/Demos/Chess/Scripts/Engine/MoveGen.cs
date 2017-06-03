using System.Collections.Generic;


/// <summary>
/// All about move generation.
/// </summary>
public static class MoveGen {


    #region Table


    /// <summary>
    /// Knight occupancy bitboard on every square.
    /// </summary>
    public static ulong[] KnightAttacksDatabase = {
		0x20400, 0x50800, 0xa1100, 0x142200, 0x284400, 0x508800, 0xa01000, 0x402000,
		0x2040004, 0x5080008, 0xa110011, 0x14220022, 0x28440044, 0x50880088, 0xa0100010, 0x40200020,
		0x204000402, 0x508000805, 0xa1100110a, 0x1422002214, 0x2844004428, 0x5088008850, 0xa0100010a0, 0x4020002040,
		0x20400040200, 0x50800080500, 0xa1100110a00, 0x142200221400, 0x284400442800, 0x508800885000, 0xa0100010a000, 0x402000204000,
		0x2040004020000, 0x5080008050000,  0xa1100110a0000, 0x14220022140000, 0x28440044280000, 0x50880088500000, 0xa0100010a00000, 0x40200020400000,
		0x204000402000000, 0x508000805000000, 0xa1100110a000000, 0x1422002214000000, 0x2844004428000000, 0x5088008850000000, 0xa0100010a0000000, 0x4020002040000000,
		0x400040200000000, 0x800080500000000, 0x1100110a00000000, 0x2200221400000000, 0x4400442800000000, 0x8800885000000000, 0x100010a000000000, 0x2000204000000000,
		0x4020000000000, 0x8050000000000, 0x110a0000000000, 0x22140000000000, 0x44280000000000, 0x88500000000000, 0x10a00000000000, 0x20400000000000

	};


    /// <summary>
    /// King occupancy bitboard on every square.
    /// </summary>
    public static ulong[] KingAttacksDatabase = {
		0x302, 0x705, 0xe0a, 0x1c14, 0x3828, 0x7050, 0xe0a0, 0xc040,
		0x30203, 0x70507, 0xe0a0e, 0x1c141c, 0x382838, 0x705070, 0xe0a0e0, 0xc040c0,
		0x3020300, 0x7050700, 0xe0a0e00, 0x1c141c00, 0x38283800, 0x70507000, 0xe0a0e000, 0xc040c000,
		0x302030000, 0x705070000, 0xe0a0e0000,  0x1c141c0000, 0x3828380000, 0x7050700000, 0xe0a0e00000, 0xc040c00000,
		0x30203000000, 0x70507000000, 0xe0a0e000000, 0x1c141c000000, 0x382838000000, 0x705070000000, 0xe0a0e0000000, 0xc040c0000000,
		0x3020300000000, 0x7050700000000, 0xe0a0e00000000, 0x1c141c00000000, 0x38283800000000, 0x70507000000000, 0xe0a0e000000000, 0xc040c000000000,
		0x302030000000000, 0x705070000000000, 0xe0a0e0000000000, 0x1c141c0000000000, 0x3828380000000000, 0x7050700000000000, 0xe0a0e00000000000, 0xc040c00000000000,
		0x203000000000000, 0x507000000000000, 0xa0e000000000000, 0x141c000000000000, 0x2838000000000000, 0x5070000000000000, 0xa0e0000000000000, 0x40c0000000000000

	};



    #endregion


    /// <summary>
    /// Returns legal rook attack bitboard.
    /// </summary>
    public static ulong RookAttack(int sq, ulong occupancy)
    {
        ulong blockers = occupancy & Magics.OccupancyMaskRook[sq];
        ulong index = (blockers * Magics.MagicNumberRook[sq]) >> Magics.MagicNumberShiftsRook[sq];
        return Magics.MagicMoveRook[sq][index];
    }

    /// <summary>
    /// Returns legal bishop attack bitboard.
    /// </summary>
    public static ulong BishopAttack(int bit, ulong occupancy)
    {
        ulong blockers = occupancy & Magics.OccupancyMaskBishop[bit];
        ulong index = (blockers * Magics.MagicNumberBishop[bit]) >> Magics.MagicNumberShiftsBishop[bit];
        return Magics.MagicMoveBishop[bit][index];
    }

    /// <summary>
    /// Generates all moves
    /// </summary>
    /// <param name="board">Board reference</param>
    /// <param name="moves">Already initialized moves array( new SMove[Defs.MaxMoves] )</param>
    /// <param name="StartAt">Array index offset</param>
    /// <param name="depth">Used in search, should not be changed otherwise( used for move killers)</param>
    /// <returns>Returns end index relative to the start</returns>
	public static int GenerateMoves(Board board, SMove[] moves, int StartAt = 0, int depth = 0) {

		int counter = StartAt;

        //Occupancy
        ulong Occupancy = board.WOcc | board.BOcc;
        int[] pieces = board.pieces;

		if (board.SideToPlay == 1) //WHITE
        {

            #region Castling move generation
            
            //Have right to castle
            if ((board.CastlePermission & Defs.CastleRightsKWCa) != 0) //King side
            {
                if ((Occupancy & 0x6000000000000000) == 0) //Not occupied
                {
                    if (!board.IsAttacked(0x7000000000000000, 0)) //Not attacked
                    {
                        //Add move
                        moves[counter].move = (60 | (62 << 6) | (Defs.WKing << 12) | (Defs.CastleMove << 20));
                        moves[counter].score = 1000;
                        counter++;
                    }
                }
            }

            if ((board.CastlePermission & Defs.CastleRightsQWCa) != 0)//Queen side
            {
                if ((Occupancy & 0xe00000000000000) == 0) //Not occupied
                {
                    if (!board.IsAttacked(0x1c00000000000000, 0)) //Not attacked
                    {
                        //Add move
                        moves[counter].move = (60 | (58 << 6) | (Defs.WKing << 12) | (Defs.CastleMove << 20));
                        moves[counter].score = 1000;
                        counter++;
                    }
                }
            }

            #endregion

            #region White knights move generation

            ulong Knights = board.WKnight;

            while (Knights != 0)
            {

                int From = Ops.FirstBit(Knights);
                Knights ^= Ops.Pow2[From];

                ulong Moves = KnightAttacksDatabase[From] & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.WKnight << 12) | (capturePiece << 16));
                    moves[counter].move = (From | (To << 6) | (Defs.WKnight << 12) | (capturePiece << 16));

                    if (capturePiece == 0)
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;




                    }
                    else {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion

            #region White bishops move generation

            ulong Bishops = board.WBishop;

            while (Bishops != 0)
            {

                int From = Ops.FirstBit(Bishops);
                Bishops ^= Ops.Pow2[From];

                ulong Moves = BishopAttack(From, Occupancy) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.WBishop << 12) | (capturePiece << 16));
                    moves[counter].move = move;

                    if (capturePiece == 0)
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }

                    counter++;

                }

            }

            #endregion

            #region White rooks move generation

            ulong Rooks = board.WRook;

            while (Rooks != 0)
            {

                int From = Ops.FirstBit(Rooks);
                Rooks ^= Ops.Pow2[From];

                ulong Moves = RookAttack(From, Occupancy) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.WRook << 12) | (capturePiece << 16));
                    moves[counter].move = move;

                    if (capturePiece == 0)
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else
                    {
                        //Quiet move

                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }

                    counter++;

                }

            }

            #endregion

            #region White queens move generation

            ulong Queens = board.WQueen;

            while (Queens != 0)
            {

                int From = Ops.FirstBit(Queens);
                Queens ^= Ops.Pow2[From];

                ulong Moves = (BishopAttack(From, Occupancy) | RookAttack(From, Occupancy)) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.WQueen << 12) | (capturePiece << 16));
                    moves[counter].move = (From | (To << 6) | (Defs.WQueen << 12) | (capturePiece << 16));

                    if (capturePiece == 0)
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion

            #region White pawn move generation

            //PAWNS
            ulong Pawns = board.WPawn; //Copied

            while (Pawns != 0)
            {

                int From = Ops.FirstBit(Pawns);
                Pawns ^= Ops.Pow2[From];

                ulong Pawn = Ops.Pow2[From];

                ulong Addes = Pawn >> 8;
                Addes &= ~Occupancy; //Can't move on a piece

                if (Addes != 0) { //We have pawn push to add
                
                    //ADD MOVE
                    int To = From - 8;

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoQueen << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoRook << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoKnight << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoBishop << 20));
                        moves[counter].score = 1000000;
                        counter++;

                    }
                    else
                    {
                        int move = (From | (To << 6) | (Defs.WPawn << 12));
                        moves[counter].move = move;
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                        counter++;
                    }
                }

                ulong ThirdRow = 0xff0000000000; //White side

                ulong DoubleAddes = (Addes & ThirdRow) >> 8;
                DoubleAddes &= ~Occupancy;

                if (DoubleAddes != 0) { //We have double push to add

                    //ADD MOVE
                    int To = From - 16;

                    moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12));
                    moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                    counter++;
                }

                ulong Attacks = ((Pawn >> 7) & 0xfefefefefefefefe) | (Pawn >> 9 & 0x7f7f7f7f7f7f7f7f);

                //Check en passant SQ
				if (board.EnPassantSq != Squares.None)
                {
					ulong EpSq = Ops.Pow2[(int)board.EnPassantSq];

                    if ((Attacks & EpSq) != 0) { //Add en passant attack

                        //ADD MOVE
						int To = (int)board.EnPassantSq;

                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.WPawn << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000; //CAPTURE



                        counter++;
                    
                    }
                }

                Attacks &= board.BOcc; //Can attack enemy pieces only

                while (Attacks != 0) {
                    int To = Ops.FirstBit(Attacks);
                    Attacks ^= Ops.Pow2[To];

                    int capturePiece = pieces[To];

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoQueen << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoRook << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoBishop << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoKnight << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;


                    }
                    else
                    {
                        //ADD MOVE
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }


            #endregion

            #region White kings move generation

            ulong King = board.WKing;

            while (King != 0)
            {

                int From = Ops.FirstBit(King);
                King ^= Ops.Pow2[From];

                ulong Moves = KingAttacksDatabase[From] & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.WKing << 12) | (capturePiece << 16));
                    moves[counter].move = move;

                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else
                    {
                        //Quiet move

                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion


        }
        else { //BLACK

            #region Castling move generation

            //Have right to castle
            if ((board.CastlePermission & Defs.CastleRightsKBCa) != 0) //King side
            {
                if ((Occupancy & 0x60) == 0) //Not occupied
                {
                    if (!board.IsAttacked(0x70, 1)) //Not attacked
                    {
                        //Add move
                        moves[counter].move = (4 | (6 << 6) | (Defs.BKing << 12) | (Defs.CastleMove << 20));
                        moves[counter].score = 1000;
                        counter++;
                    }
                }
            }

            if ((board.CastlePermission & Defs.CastleRightsQBCa) != 0)//Queen side
            {
                if ((Occupancy & 0xe) == 0) //Not occupied
                {
                    if (!board.IsAttacked(0x1c, 1)) //Not attacked
                    {
                        //Add move
                        moves[counter].move = (4 | (2 << 6) | (Defs.BKing << 12) | (Defs.CastleMove << 20));
                        moves[counter].score = 1000;
                        counter++;
                    }
                }
            }

            #endregion

            #region Black knights move generation

            ulong Knights = board.BKnight;

            while (Knights != 0)
            {

                int From = Ops.FirstBit(Knights);
                Knights ^= Ops.Pow2[From];
                ulong Moves = KnightAttacksDatabase[From] & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.BKnight << 12) | (capturePiece << 16));
                    moves[counter].move = move;
                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else
                    {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion

            #region Black bishops move generation

            ulong Bishops = board.BBishop;

            while (Bishops != 0)
            {

                int From = Ops.FirstBit(Bishops);
                Bishops ^= Ops.Pow2[From];
                ulong Moves = BishopAttack(From, Occupancy) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.BBishop << 12) | (capturePiece << 16));
                    moves[counter].move = move;
                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else
                    {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion

            #region Black rooks move generation

            ulong Rooks = board.BRook;

            while (Rooks != 0)
            {

                int From = Ops.FirstBit(Rooks);
                Rooks ^= Ops.Pow2[From];
                ulong Moves = RookAttack(From, Occupancy) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    int move = (From | (To << 6) | (Defs.BRook << 12) | (capturePiece << 16));
                    moves[counter].move = move;
                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;

                    }
                    else
                    {
                        //Quiet move
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }

                    counter++;

                }

            }

            #endregion

            #region Black queens move generation

            ulong Queens = board.BQueen;

            while (Queens != 0)
            {

                int From = Ops.FirstBit(Queens);
                Queens ^= Ops.Pow2[From];
                ulong Moves = (BishopAttack(From, Occupancy) | RookAttack(From, Occupancy)) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];
                    //Add move
                    int move = (From | (To << 6) | (Defs.BQueen << 12) | (capturePiece << 16));
                    moves[counter].move = move;
                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                    }
                    else {
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }

                    counter++;

                }

            }

            #endregion

            #region Black pawn move generation

            //PAWNS
            ulong Pawns = board.BPawn; //Copied

            while (Pawns != 0)
            {

                int From = Ops.FirstBit(Pawns);
                Pawns ^= Ops.Pow2[From];
                ulong Pawn = Ops.Pow2[From];

                ulong Addes = Pawn << 8;
                Addes &= ~Occupancy; //Can't move on a piece

                if (Addes != 0)
                { //We have pawn push to add

                    //ADD MOVE
                    int To = From + 8;

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff00000000000000) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoQueen << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoRook << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoKnight << 20));
                        moves[counter].score = 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoBishop << 20));
                        moves[counter].score = 1000000;
                        counter++;

                    }
                    else
                    {
                        int move = (From | (To << 6) | (Defs.BPawn << 12));
                        moves[counter].move = move;
                        
                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                        counter++;
                    }
                }

                ulong ThirdRow = 0xff0000; //Black side

                ulong DoubleAddes = (Addes & ThirdRow) << 8;
                DoubleAddes &= ~Occupancy;

                if (DoubleAddes != 0)
                { //We have double push to add

                    //ADD MOVE
                    int To = From + 16;

                    moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12));
                    moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                    counter++;
                }

                ulong Attacks = ((Pawn << 7) & 0x7f7f7f7f7f7f7f7f) | (Pawn << 9 & 0xfefefefefefefefe);

                //Check en passant SQ
				if (board.EnPassantSq != Squares.None)
                {
					ulong EpSq = Ops.Pow2[(int)board.EnPassantSq];

                    if ((Attacks & EpSq) != 0)
                    { //Add en passant attack

                        //ADD MOVE
						int To = (int)board.EnPassantSq;

                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.WPawn << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;

                            

                        counter++;

                    }
                }

                Attacks &= board.WOcc; //Can attack enemy pieces only

                while (Attacks != 0)
                {
                    int To = Ops.FirstBit(Attacks);
                    Attacks ^= Ops.Pow2[To];

                    int capturePiece = board.pieces[To];

                    //Check for promotion
					if ((Ops.Pow2[To] & 0xff00000000000000) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoQueen << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;

                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoRook << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;

                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoBishop << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;

                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoKnight << 20));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;
                        counter++;

                    }
                    else
                    {
                        //ADD MOVE
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[pieces[To]][pieces[From]] + 1000000;



                        counter++;
                    }
                }

            }
            #endregion

            #region Black kings move generation

            ulong King = board.BKing;

            while (King != 0)
            {

                int From = Ops.FirstBit(King);
                King ^= Ops.Pow2[From];

                ulong Moves = KingAttacksDatabase[From] & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];
                    //Add move
                    int move = (From | (To << 6) | (Defs.BKing << 12) | (capturePiece << 16));
                    moves[counter].move = move;

                    if (capturePiece != 0) //Capturing move
                    {
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;



                    }
                    else
                    {
                        //Quiet move

                        if (board.SearchKillers[0][depth] == move)
                        {
                            moves[counter].score = 900000;
                        }
                        else if (board.SearchKillers[1][depth] == move)
                        {
                            moves[counter].score = 800000;
                        }
                        else
                        {
                            moves[counter].score = board.SearchHistory[board.pieces[From]][To];
                        }
                    }
                    counter++;

                }

            }

            #endregion


        }

        //Returns end index relative to the start
		return counter- StartAt;
	}

    public static int GenerateCapturingMoves(Board board, SMove[] moves, int StartAt = 0)
    {

        int counter = StartAt;

        //Occupancy
        ulong Occupancy = board.WOcc | board.BOcc;
        int[] pieces = board.pieces;

        if (board.SideToPlay == 1) //WHITE
        {


            #region White rooks move generation

            ulong Rooks = board.WRook;

            while (Rooks != 0)
            {

                int From = Ops.FirstBit(Rooks);
                Rooks ^= Ops.Pow2[From];

                ulong Moves = RookAttack(From, Occupancy) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.BKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.WRook << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }

                }

            }

            #endregion

            #region White knights move generation

            ulong Knights = board.WKnight;

            while (Knights != 0)
            {

                int From = Ops.FirstBit(Knights);
                Knights ^= Ops.Pow2[From];

                ulong Moves = KnightAttacksDatabase[From] & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.BKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.WKnight << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region White bishops move generation

            ulong Bishops = board.WBishop;

            while (Bishops != 0)
            {

                int From = Ops.FirstBit(Bishops);
                Bishops ^= Ops.Pow2[From];

                ulong Moves = BishopAttack(From, Occupancy) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.BKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.WBishop << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }

                }

            }

            #endregion

            #region White queens move generation

            ulong Queens = board.WQueen;

            while (Queens != 0)
            {

                int From = Ops.FirstBit(Queens);
                Queens ^= Ops.Pow2[From];

                ulong Moves = (BishopAttack(From, Occupancy) | RookAttack(From, Occupancy)) & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.BKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.WQueen << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region White pawn move generation

            //PAWNS
            ulong Pawns = board.WPawn; //Copied

            while (Pawns != 0)
            {

                int From = Ops.FirstBit(Pawns);
                Pawns ^= Ops.Pow2[From];

                ulong Pawn = Ops.Pow2[From];

                ulong Addes = Pawn >> 8;
                Addes &= ~Occupancy; //Can't move on a piece

                if (Addes != 0)
                { //We have pawn push to add

                    //ADD MOVE
                    int To = From - 8;

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoQueen << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoRook << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoKnight << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.PromoBishop << 20));
                        moves[counter].score = 0;
                        counter++;

                    }
                }

                ulong Attacks = ((Pawn >> 7) & 0xfefefefefefefefe) | (Pawn >> 9 & 0x7f7f7f7f7f7f7f7f);

                //Check en passant SQ
                if (board.EnPassantSq != Squares.None)
                {
                    ulong EpSq = Ops.Pow2[(int)board.EnPassantSq];

                    if ((Attacks & EpSq) != 0)
                    { //Add en passant attack

                        //ADD MOVE
                        int To = (int)board.EnPassantSq;

                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (Defs.WPawn << 20));
                        moves[counter].score = MvvLva.Table[Defs.WPawn][pieces[From]];
                        counter++;

                    }
                }

                Attacks &= board.BOcc; //Can attack enemy pieces only

                while (Attacks != 0)
                {
                    int To = Ops.FirstBit(Attacks);
                    Attacks ^= Ops.Pow2[To];

                    int capturePiece = pieces[To];

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoQueen << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoRook << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoBishop << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16) | (Defs.PromoKnight << 20));
                        moves[counter].score = 0;
                        counter++;


                    }
                    else
                    {
                        //ADD MOVE
                        moves[counter].move = (From | (To << 6) | (Defs.WPawn << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }


            #endregion

            #region White kings move generation

            ulong King = board.WKing;

            while (King != 0)
            {

                int From = Ops.FirstBit(King);
                King ^= Ops.Pow2[From];

                ulong Moves = KingAttacksDatabase[From] & ~board.WOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.BKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.WKing << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }

                }

            }

            #endregion


        }
        else
        { //BLACK


            #region Black rooks move generation

            ulong Rooks = board.BRook;

            while (Rooks != 0)
            {

                int From = Ops.FirstBit(Rooks);
                Rooks ^= Ops.Pow2[From];
                ulong Moves = RookAttack(From, Occupancy) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.WKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.BRook << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region Black knights move generation

            ulong Knights = board.BKnight;

            while (Knights != 0)
            {

                int From = Ops.FirstBit(Knights);
                Knights ^= Ops.Pow2[From];
                ulong Moves = KnightAttacksDatabase[From] & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.WKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.BKnight << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region Black bishops move generation

            ulong Bishops = board.BBishop;

            while (Bishops != 0)
            {

                int From = Ops.FirstBit(Bishops);
                Bishops ^= Ops.Pow2[From];
                ulong Moves = BishopAttack(From, Occupancy) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];

                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.WKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.BBishop << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region Black queens move generation

            ulong Queens = board.BQueen;

            while (Queens != 0)
            {

                int From = Ops.FirstBit(Queens);
                Queens ^= Ops.Pow2[From];
                ulong Moves = (BishopAttack(From, Occupancy) | RookAttack(From, Occupancy)) & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];
                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.WKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.BQueen << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion

            #region Black pawn move generation

            //PAWNS
            ulong Pawns = board.BPawn; //Copied

            while (Pawns != 0)
            {

                int From = Ops.FirstBit(Pawns);
                Pawns ^= Ops.Pow2[From];
                ulong Pawn = Ops.Pow2[From];

                ulong Addes = Pawn << 8;
                Addes &= ~Occupancy; //Can't move on a piece

                if (Addes != 0)
                { //We have pawn push to add

                    //ADD MOVE
                    int To = From + 8;

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff00000000000000) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoQueen << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoRook << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoKnight << 20));
                        moves[counter].score = 0;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.PromoBishop << 20));
                        moves[counter].score = 0;
                        counter++;

                    }
                }

                ulong Attacks = ((Pawn << 7) & 0x7f7f7f7f7f7f7f7f) | (Pawn << 9 & 0xfefefefefefefefe);

                //Check en passant SQ
                if (board.EnPassantSq != Squares.None)
                {
                    ulong EpSq = Ops.Pow2[(int)board.EnPassantSq];

                    if ((Attacks & EpSq) != 0)
                    { //Add en passant attack

                        //ADD MOVE
                        int To = (int)board.EnPassantSq;

                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (Defs.WPawn << 20));
                        moves[counter].score = MvvLva.Table[Defs.WPawn][pieces[From]];
                        counter++;

                    }
                }

                Attacks &= board.WOcc; //Can attack enemy pieces only

                while (Attacks != 0)
                {
                    int To = Ops.FirstBit(Attacks);
                    Attacks ^= Ops.Pow2[To];

                    int capturePiece = board.pieces[To];

                    //Check for promotion
                    if ((Ops.Pow2[To] & 0xff00000000000000) != 0)
                    {
                        //Add all 4 possible promotion
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoQueen << 20));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoRook << 20));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoBishop << 20));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16) | (Defs.PromoKnight << 20));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;

                    }
                    else
                    {
                        //ADD MOVE
                        moves[counter].move = (From | (To << 6) | (Defs.BPawn << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }
            #endregion

            #region Black kings move generation

            ulong King = board.BKing;

            while (King != 0)
            {

                int From = Ops.FirstBit(King);
                King ^= Ops.Pow2[From];

                ulong Moves = KingAttacksDatabase[From] & ~board.BOcc; //Can't step on a friendly pieces

                while (Moves != 0)
                {
                    int To = Ops.PopFirstBit(ref Moves);
                    int capturePiece = pieces[To];
                    //Add move
                    if (capturePiece != 0 && capturePiece != Defs.WKing)
                    {
                        moves[counter].move = (From | (To << 6) | (Defs.BKing << 12) | (capturePiece << 16));
                        moves[counter].score = MvvLva.Table[capturePiece][pieces[From]] + 1000000;
                        counter++;
                    }
                }

            }

            #endregion


        }
        return counter - StartAt;
    }

}
