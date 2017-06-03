using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BoardState { 
    InProgress,
    BlackMate,
    WhiteMate,
    StaleMate,
    Repetition,
}

public sealed class Board {

    /// <summary>
    /// Current board state.
    /// </summary>
    public BoardState State { get; set; }

	//64 bit ulong for each square and piece
	//Two elements in an array for black(0) and white(1) side
    public ulong WPawn, BPawn;
    public ulong WBishop, BBishop;
    public ulong WKnight, BKnight;
    public ulong WRook, BRook;
    public ulong WQueen, BQueen;
    public ulong WKing, BKing;
    public ulong WOcc, BOcc;

    /// <summary>
    /// Pieces on board.
    /// </summary>
    public int[] pieces;

    /// <summary>
    /// Position hash.
    /// </summary>
    public long Position;

	/// <summary>
	/// En passant square.
	/// </summary>
	public Squares EnPassantSq;

	/// <summary>
	/// Current side to play.
	/// </summary>
	public int SideToPlay;

	/// <summary>
	/// Castling permission.
	/// </summary>
	public int CastlePermission;

    /// <summary>
    /// Fifty move rule.
    /// </summary>
    public int FiftyMove;

	/// <summary>
	/// History moves.
	/// </summary>
	public HMove[] History;
	public int ply = 0;

    /// <summary>
    /// Principal variatian.
    /// </summary>
    public PVTable PvTable;
    public int[] PvArray;

    public int[][] SearchHistory;
    public int[][] SearchKillers;

	/// <summary>
	/// Construct board based on FEN.
	/// </summary>
	public Board(string Fen)
	{

        pieces = new int[64]; //Every square for every piece
        PvArray = new int[Defs.MaxDepth];

        History = new HMove[2056];
        for (int i = 0; i < History.Length; i++) {
            History[i] = new HMove();
        }

        InitPVTable();
        SearchHistory = new int[14][];
        SearchKillers = new int[2][];
        for (int i = 0; i < 14; i++) {
            SearchHistory[i] = new int[64];
        }

        SearchKillers[0] = new int[Defs.MaxDepth];
        SearchKillers[1] = new int[Defs.MaxDepth];

		//Init board from FEN
		this.BoardFromFen(Fen);

        //Get hash key
        Position = Zobrist.GetHashPosition(this);

        UpdateOccupancy();

	}

    public void InitPVTable(float TableSizeInMB = 2.0f)
    {
        int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(PVEntry));
        int numOfElements = (int)((TableSizeInMB * (1024f * 1024f)) / size);
        //Debug.Log(numOfElements);    
        PvTable = new PVTable() { data = new PVEntry[numOfElements], numEntries = numOfElements };
    }


    public void ClearPVTable() {
        for (int i = 0; i < PvTable.data.Length; i++) {
            PvTable.data[i].hash = 0L;
            PvTable.data[i].move = 0;
        }
    }

    /// <summary>
    /// Stores a move at the board hash position.
    /// </summary>
    public void StorePVMove(int move)
    {
        int index = (int)(Position % PvTable.numEntries);
        if (index >= 0 && index < PvTable.numEntries) {
            PvTable.data[index].move = move;
            PvTable.data[index].hash = Position;
        }
    }

    /// <summary>
    /// Check if the move exsist at the board.
    /// </summary>
    public bool MoveExists(int move) {
        int num = MoveGen.GenerateMoves(this, CheckingMoves);
        //For every move
        for (int i = 0; i < num; i++)
        {
            int myMove = CheckingMoves[i].move;
            MakeMove(myMove);
            if (!MoveWasIllegal())
            {
                if (myMove == move)
                {
                    UndoMove();
                    return true;
                }
            }
            UndoMove();
        }
        return false;
    }

    /// <summary>
    /// Get the move at this hash position.
    /// </summary>
    public int ProbePVTable() { 
        int index = (int)(Position % PvTable.numEntries);

        if (index >= 0 && index < PvTable.numEntries)
        {
            if (PvTable.data[index].hash == Position)
            {
                return PvTable.data[index].move;
            }
        }

        return 0;
    }

    /// <summary>
    /// Gets the PV line.
    /// </summary>
    public int GetPVLine(int depth) {

        int move = ProbePVTable();
        int count = 0;
        
        while (move != 0 && count < depth) {

            if (MoveExists(move))
            {
                MakeMove(move);
                PvArray[count] = move;
                count++;
            }
            else
            {
                break;
            }

            move = ProbePVTable();
        }
        int temp = count;
        while (temp>0)
        {
            UndoMove();
            temp--;
        }
        return count;
    }

    /// <summary>
    /// Updates the occupancy BB.
    /// </summary>
    public void UpdateOccupancy()
	{

		BOcc = BPawn | BBishop | BKnight | BRook | BQueen | BKing;
        WOcc = WPawn | WBishop | WKnight | WRook | WQueen | WKing;

	}

	/// <summary>
	/// Gets the piece at given side in the given square.
	/// </summary>
	public int GetPieceAt(int index)
	{
        return pieces[index];
	}

	/// <summary>
	/// Makes a move.
	/// </summary>
	public void MakeMove(int move){

        HMove hisMove = History[ply];
        hisMove.castle = CastlePermission;
        hisMove.ep = (int)EnPassantSq;
        hisMove.move = move;
        hisMove.pos = Position;
        hisMove.fiftyMove = FiftyMove;

		//Move was made no more en passant square
		EnPassantSq = Squares.None;

        //increase it, but it may reset
        FiftyMove++;

        int From = move & 0x3f;
        int To = (move >> 6) & 0x3f;
        int movePiece = (move >> 12) & 0xf;

        //Removes the current castle permission from hash key since that might change during the move making
        Position ^= Zobrist.CastleRights[CastlePermission];

		switch(movePiece)
        {
            #region WHITE
            case Defs.WPawn:
			//Moved to the desired position
			WPawn ^= Ops.Pow2[From];
			WPawn |= Ops.Pow2[To];

            //Zobrist
            Position ^= Zobrist.Pieces[From][Defs.WPawn];
            Position ^= Zobrist.Pieces[To][Defs.WPawn];

            //Reset fifty move
            FiftyMove = 0;

            pieces[From] = 0;
            pieces[To] = Defs.WPawn;

            if ((move & 0x700000) == 0x100000) // En passan capture
            {
				//Remove pawn
                BPawn ^= Ops.Pow2[To + 8];
                pieces[To + 8] = 0;

                //Removes only
                Position ^= Zobrist.Pieces[To+8][Defs.BPawn];
			}
			else if(From - To == 16){ //Double pawn move
				EnPassantSq = (Squares)(To+8);
			}
            else if ((move & 0x700000) > 0x200000) //Move is promotion
            {
                #region promo
                switch ((move >> 20) & 0xf)
                {
                    case Defs.PromoQueen:
                        WQueen ^= Ops.Pow2[To];
                        WPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.WQueen;
                        Position ^= Zobrist.Pieces[To][Defs.WPawn];
                        Position ^= Zobrist.Pieces[To][Defs.WQueen];
                        break;
                    case Defs.PromoBishop:
                        WBishop ^= Ops.Pow2[To];
                        WPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.WBishop;
                        Position ^= Zobrist.Pieces[To][Defs.WPawn];
                        Position ^= Zobrist.Pieces[To][Defs.WBishop];
                        break;
                    case Defs.PromoKnight:
                        WKnight ^= Ops.Pow2[To];
                        WPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.WKnight;
                        Position ^= Zobrist.Pieces[To][Defs.WPawn];
                        Position ^= Zobrist.Pieces[To][Defs.WKnight];
                        break;
                    case Defs.PromoRook:
                        WRook ^= Ops.Pow2[To];
                        WPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.WRook;
                        Position ^= Zobrist.Pieces[To][Defs.WPawn];
                        Position ^= Zobrist.Pieces[To][Defs.WRook];
                        break;
                }
                #endregion
            }


            break;
            case Defs.WBishop:
			WBishop ^= Ops.Pow2[From];
            WBishop |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.WBishop];
            Position ^= Zobrist.Pieces[To][Defs.WBishop];

            pieces[From] = 0;
            pieces[To] = Defs.WBishop;

			break;
            case Defs.WKnight:
            WKnight ^= Ops.Pow2[From];
            WKnight |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.WKnight];
            Position ^= Zobrist.Pieces[To][Defs.WKnight];

            pieces[From] = 0;
            pieces[To] = Defs.WKnight;

			break;
            case Defs.WRook:
            WRook ^= Ops.Pow2[From];
            WRook |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.WRook];
            Position ^= Zobrist.Pieces[To][Defs.WRook];

            pieces[From] = 0;
            pieces[To] = Defs.WRook;

			if(From == 56){
				CastlePermission &= ~Defs.CastleRightsQWCa;
			}
			else if(From == 63){
				CastlePermission &= ~Defs.CastleRightsKWCa;
			}
			
			break;
            case Defs.WQueen:
            WQueen ^= Ops.Pow2[From];
            WQueen |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.WQueen];
            Position ^= Zobrist.Pieces[To][Defs.WQueen];

            pieces[From] = 0;
            pieces[To] = Defs.WQueen;

			break;
            case Defs.WKing:
            WKing ^= Ops.Pow2[From];
            WKing |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.WKing];
            Position ^= Zobrist.Pieces[To][Defs.WKing];


            pieces[From] = 0;
            pieces[To] = Defs.WKing;


            if ((move & 0x700000) == 0x200000)
            {
                if (To == 58)
                {
                    WRook ^= Ops.Pow2[56];
                    WRook ^= Ops.Pow2[59];
                    pieces[56] = 0;
                    pieces[59] = Defs.WRook;
                    Position ^= Zobrist.Pieces[56][Defs.WRook];
                    Position ^= Zobrist.Pieces[59][Defs.WRook];
                }
                else if (To == 62)
                {
                    WRook ^= Ops.Pow2[61];
                    WRook ^= Ops.Pow2[63];
                    pieces[63] = 0;
                    pieces[61] = Defs.WRook;
                    Position ^= Zobrist.Pieces[61][Defs.WRook];
                    Position ^= Zobrist.Pieces[63][Defs.WRook];
                }
            }

			CastlePermission &= ~(Defs.CastleRightsKWCa | Defs.CastleRightsQWCa);
								
			break;
            #endregion

            #region BLACK
            case Defs.BPawn:
            //Moved to the desired position
            BPawn ^= Ops.Pow2[From];
            BPawn |= Ops.Pow2[To];

            Position ^= Zobrist.Pieces[From][Defs.BPawn];
            Position ^= Zobrist.Pieces[To][Defs.BPawn];

            //Reset fifty move
            FiftyMove = 0;

            pieces[From] = 0;
            pieces[To] = Defs.BPawn;

            if ((move & 0x700000) == 0x100000)
            {
                //Remove pawn
                WPawn ^= Ops.Pow2[To - 8];
                pieces[To - 8] = 0;

                //Removes only
                Position ^= Zobrist.Pieces[To + 8][Defs.BPawn];

            }
            else if (From - To == -16)
            {
                EnPassantSq = (Squares)(To - 8);
            }
            else if ((move & 0x700000) > 0x200000)
                #region promo
                switch ((move >> 20) & 0xf)
                {
                    case Defs.PromoQueen:
                        BQueen ^= Ops.Pow2[To];
                        BPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.BQueen;
                        Position ^= Zobrist.Pieces[To][Defs.BPawn];
                        Position ^= Zobrist.Pieces[To][Defs.BQueen];

                        break;
                    case Defs.PromoBishop:
                        BBishop ^= Ops.Pow2[To];
                        BPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.BBishop;
                        Position ^= Zobrist.Pieces[To][Defs.BPawn];
                        Position ^= Zobrist.Pieces[To][Defs.BBishop];

                        break;
                    case Defs.PromoKnight:
                        BKnight ^= Ops.Pow2[To];
                        BPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.BKnight;
                        Position ^= Zobrist.Pieces[To][Defs.BPawn];
                        Position ^= Zobrist.Pieces[To][Defs.BKnight];

                        break;
                    case Defs.PromoRook:
                        BRook ^= Ops.Pow2[To];
                        BPawn ^= Ops.Pow2[To];
                        pieces[To] = Defs.BRook;
                        Position ^= Zobrist.Pieces[To][Defs.BPawn];
                        Position ^= Zobrist.Pieces[To][Defs.BRook];

                        break;
                }
                #endregion


            break;
            case Defs.BBishop:
            BBishop ^= Ops.Pow2[From];
            BBishop |= Ops.Pow2[To];

            pieces[From] = 0;
            pieces[To] = Defs.BBishop;

            Position ^= Zobrist.Pieces[From][Defs.BBishop];
            Position ^= Zobrist.Pieces[To][Defs.BBishop];


            break;
            case Defs.BKnight:
            BKnight ^= Ops.Pow2[From];
            BKnight ^= Ops.Pow2[To];

            pieces[From] = 0;
            pieces[To] = Defs.BKnight;

            Position ^= Zobrist.Pieces[From][Defs.BKnight];
            Position ^= Zobrist.Pieces[To][Defs.BKnight];


            break;
            case Defs.BRook:
            BRook ^= Ops.Pow2[From];
            BRook |= Ops.Pow2[To];

            pieces[From] = 0;
            pieces[To] = Defs.BRook;

            Position ^= Zobrist.Pieces[From][Defs.BRook];
            Position ^= Zobrist.Pieces[To][Defs.BRook];


            if (From == 0)
            {
                CastlePermission &= ~Defs.CastleRightsQBCa;
            }
            else if (From == 7)
            {
                CastlePermission &= ~Defs.CastleRightsKBCa;
            }

            break;
            case Defs.BQueen:
            BQueen ^= Ops.Pow2[From];
            BQueen |= Ops.Pow2[To];

            pieces[From] = 0;
            pieces[To] = Defs.BQueen;

            Position ^= Zobrist.Pieces[From][Defs.BQueen];
            Position ^= Zobrist.Pieces[To][Defs.BQueen];


            break;
            case Defs.BKing:
            BKing ^= Ops.Pow2[From];
            BKing |= Ops.Pow2[To];

            pieces[From] = 0;
            pieces[To] = Defs.BKing;

            Position ^= Zobrist.Pieces[From][Defs.BKing];
            Position ^= Zobrist.Pieces[To][Defs.BKing];


            if ((move & 0x700000) == 0x200000)
            {
                if (To == 2)
                {
                    BRook ^= Ops.Pow2[3];
                    BRook ^= Ops.Pow2[0];
                    pieces[0] = 0;
                    pieces[3] = Defs.BRook;

                    Position ^= Zobrist.Pieces[0][Defs.BRook];
                    Position ^= Zobrist.Pieces[3][Defs.BRook];

                }
                else if (To == 6)
                {
                    BRook ^= Ops.Pow2[5];
                    BRook ^= Ops.Pow2[7];
                    pieces[7] = 0;
                    pieces[5] = Defs.BRook;
                    Position ^= Zobrist.Pieces[5][Defs.BRook];
                    Position ^= Zobrist.Pieces[7][Defs.BRook];

                }
            }
            CastlePermission &= ~(Defs.CastleRightsKBCa | Defs.CastleRightsQBCa);

            break;
            #endregion
        }

        if ((move & 0xf0000) != 0) //Move was capture
        {
            #region capture
            switch ((move >> 16) & 0xf)
            {
                #region white
                case Defs.WPawn:
                    WPawn ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.WPawn];
                    break;
                case Defs.WKnight:
                    WKnight ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.WKnight];
                    break;
                case Defs.WBishop:
                    WBishop ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.WKnight];
                    break;
                case Defs.WRook:
                    WRook ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.WRook];

                    if (To == 56)
                        CastlePermission &= ~Defs.CastleRightsQWCa;
                    else if (To == 63)
                        CastlePermission &= ~Defs.CastleRightsKWCa;

                    break;
                case Defs.WQueen:
                    WQueen ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.WQueen];

                    break;
                #endregion
                #region black
                case Defs.BPawn:
                    BPawn ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.BPawn];

                    break;
                case Defs.BKnight:
                    BKnight ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.BKnight];

                    break;
                case Defs.BBishop:
                    BBishop ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.BBishop];

                    break;
                case Defs.BRook:
                    BRook ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.BRook];

                    if (To == 0)
                        CastlePermission &= ~Defs.CastleRightsQBCa;
                    else if (To == 7)
                        CastlePermission &= ~Defs.CastleRightsKBCa;
                    break;
                case Defs.BQueen:
                    BQueen ^= Ops.Pow2[To];
                    Position ^= Zobrist.Pieces[To][Defs.BQueen];

                    break;
                #endregion

            }
            #endregion

            //Reset fifty move
            FiftyMove = 0;
        }

        //Castle permission
        Position ^= Zobrist.CastleRights[CastlePermission];

        Position ^= Zobrist.CastleRights[SideToPlay];

		//Side to play
		SideToPlay = 1-SideToPlay;

        Position ^= Zobrist.CastleRights[SideToPlay];

		//Updates occupancy
        BOcc = BPawn | BBishop | BKnight | BRook | BQueen | BKing;
        WOcc = WPawn | WBishop | WKnight | WRook | WQueen | WKing;

		History[ply] = hisMove;
		ply ++;

	}

	public void UndoMove(){

		if(ply == 0) //No moves to undo
			return;

		ply--;

		HMove hisMove = History[ply];
        int move = hisMove.move;

        int From = move & 0x3f;
        int To = (move >> 6) & 0x3f;
        int movePiece = (move >> 12) & 0xf;

        switch (movePiece)
        {

            #region WHITE
            case Defs.WPawn:

                if ((move & 0x700000) > 0x200000)
                {
                    #region promo
                    switch ((move >> 20) & 0xf)
                    {
                        case Defs.PromoQueen:
                            WQueen ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoBishop:
                            WBishop ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoKnight:
                            WKnight ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoRook:
                            WRook ^= Ops.Pow2[To];
                            break;
                    }
                    #endregion
                }
                else
                    WPawn ^= Ops.Pow2[To];

                //Moved to the desired position
                WPawn ^= Ops.Pow2[From];

                pieces[To] = 0;
                pieces[From] = Defs.WPawn;

                if (move.IsEnPassantCapt())
                {
                    BPawn ^= Ops.Pow2[To + 8];
                    pieces[To + 8] = Defs.BPawn;
                }


                break;
            case Defs.WBishop:
                WBishop ^= Ops.Pow2[From];
                WBishop ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.WBishop;

                break;
            case Defs.WKnight:
                WKnight ^= Ops.Pow2[From];
                WKnight ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.WKnight;

                break;
            case Defs.WRook:
                WRook ^= Ops.Pow2[From];
                WRook ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.WRook;

                break;
            case Defs.WQueen:
                WQueen ^= Ops.Pow2[From];
                WQueen ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.WQueen;

                break;
            case Defs.WKing:
                WKing ^= Ops.Pow2[From];
                WKing ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.WKing;


                if ((move & 0x700000) == 0x200000)
                {
                    if (To == 58)
                    {
                        WRook ^= Ops.Pow2[56];
                        WRook ^= Ops.Pow2[59];
                        pieces[56] = Defs.WRook;
                        pieces[59] = Defs.Empty;

                    }
                    else if (To == 62)
                    {
                        WRook ^= Ops.Pow2[61];
                        WRook ^= Ops.Pow2[63];
                        pieces[63] = Defs.WRook;
                        pieces[61] = Defs.Empty;

                    }

                }
                break;
            #endregion

            #region BLACK
            case Defs.BPawn:

                if ((move & 0x700000) > 0x200000)
                {
                    #region promo
                    switch ((move >> 20) & 0xf)
                    {
                        case Defs.PromoQueen:
                            BQueen ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoBishop:
                            BBishop ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoKnight:
                            BKnight ^= Ops.Pow2[To];
                            break;
                        case Defs.PromoRook:
                            BRook ^= Ops.Pow2[To];
                            break;
                    }
                    #endregion
                }
                else
                    BPawn ^= Ops.Pow2[To];


                //Moved to the desired position
                BPawn ^= Ops.Pow2[From];

                pieces[To] = 0;
                pieces[From] = Defs.BPawn;

                if (move.IsEnPassantCapt())
                {
                    WPawn ^= Ops.Pow2[To - 8];
                    pieces[To - 8] = Defs.WPawn;
                }


                break;
            case Defs.BBishop:
                BBishop ^= Ops.Pow2[From];
                BBishop ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.BBishop;

                break;
            case Defs.BKnight:
                BKnight ^= Ops.Pow2[From];
                BKnight ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.BKnight;

                break;
            case Defs.BRook:
                BRook ^= Ops.Pow2[From];
                BRook ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.BRook;

                break;
            case Defs.BQueen:
                BQueen ^= Ops.Pow2[From];
                BQueen ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.BQueen;

                break;
            case Defs.BKing:
                BKing ^= Ops.Pow2[From];
                BKing ^= Ops.Pow2[To];

                pieces[To] = 0;
                pieces[From] = Defs.BKing;


                if ((move & 0x700000) == 0x200000)
                {
                    if (To == 2)
                    {
                        BRook ^= Ops.Pow2[3];
                        BRook ^= Ops.Pow2[0];
                        pieces[0] = Defs.BRook;
                        pieces[3] = Defs.Empty;


                    }
                    else if (To == 6)
                    {
                        BRook ^= Ops.Pow2[5];
                        BRook ^= Ops.Pow2[7];
                        pieces[7] = Defs.BRook;
                        pieces[5] = Defs.Empty;


                    }
                }
                    

                



                break;
            #endregion
        }

        if ((move & 0xf0000) != 0 )
        {
            #region capture
            switch ((move >> 16) & 0xf)
            {
                #region white
                case Defs.WPawn:
                    WPawn ^= Ops.Pow2[To];
                    pieces[To] = Defs.WPawn;
                    break;
                case Defs.WKnight:
                    WKnight ^= Ops.Pow2[To];
                    pieces[To] = Defs.WKnight;
                    break;
                case Defs.WBishop:
                    WBishop ^= Ops.Pow2[To];
                    pieces[To] = Defs.WBishop;
                    break;
                case Defs.WRook:
                    WRook ^= Ops.Pow2[To];
                    pieces[To] = Defs.WRook;
                    break;
                case Defs.WQueen:
                    WQueen ^= Ops.Pow2[To];
                    pieces[To] = Defs.WQueen;
                    break;
                #endregion
                #region black
                case Defs.BPawn:
                    BPawn ^= Ops.Pow2[To];
                    pieces[To] = Defs.BPawn;
                    break;
                case Defs.BKnight:
                    BKnight ^= Ops.Pow2[To];
                    pieces[To] = Defs.BKnight;
                    break;
                case Defs.BBishop:
                    BBishop ^= Ops.Pow2[To];
                    pieces[To] = Defs.BBishop;
                    break;
                case Defs.BRook:
                    BRook ^= Ops.Pow2[To];
                    pieces[To] = Defs.BRook;
                    break;
                case Defs.BQueen:
                    BQueen ^= Ops.Pow2[To];
                    pieces[To] = Defs.BQueen;
                    break;
                #endregion

            }
            #endregion   
        
        }


		//Updates occupancy
        BOcc = BPawn | BBishop | BKnight | BRook | BQueen | BKing;
        WOcc = WPawn | WBishop | WKnight | WRook | WQueen | WKing;

        CastlePermission = hisMove.castle;
        EnPassantSq = (Squares)hisMove.ep;
        Position = hisMove.pos;
        FiftyMove = hisMove.fiftyMove;

		SideToPlay = 1-SideToPlay;

	}

    /// <summary>
    /// This should be called when move should be checked for mate, stale mate and other rules. Always!
    /// </summary>
    private SMove[] CheckingMoves = new SMove[Defs.MaxMoves];

    public bool MakeMoveWithCheck(int move) {

        if (State != BoardState.InProgress)
            return false;

        //Stale mate
        if (FiftyMove >= 100) {
            State = BoardState.StaleMate;
            return false;
        }


        MakeMove(move);

        //Check if the move is legal
        if (MoveWasIllegal())
        {
            UndoMove();
            return false;
        }
        else
        {
            //Checks for the mate            
            int num = MoveGen.GenerateMoves(this, CheckingMoves);
            bool hasMadeALegalMove = false;
            //For every move
            for (int i = 0; i < num; i++)
            {
                int myMove = CheckingMoves[i].move;
                MakeMove(myMove);
                if (!MoveWasIllegal())
                {
                    hasMadeALegalMove = true;
                }
                UndoMove();
            }


            //Check if mate
            if (!hasMadeALegalMove)
            {

                if (IsAttacked(BKing, 1))
                {
                    //Mate
                    if (SideToPlay == 0) //Black
                        State = BoardState.BlackMate;
                }
                else if (IsAttacked(WKing, 0))
                {
                    //Mate
                    if (SideToPlay == 1) //Black
                        State = BoardState.WhiteMate;
                }
                else
                {
                    //Stale mate
                    State = BoardState.StaleMate;
                }
            }

            //Checks if this is repetition
            if (IsRepetition())
            {
                State = BoardState.Repetition;
            }

        }
        return true;
    }

	/// <summary>
	/// Returns true if the position BB is attacked by the given side.
	/// </summary>
    public bool IsAttacked(ulong pos, int attackedBySide)
    {
		//Pawn attacks
        ulong Attacks = (WPawn >> 7 & 0xfefefefefefefefe) | (WPawn >> 9 & 0x7f7f7f7f7f7f7f7f), Bishops = WBishop, Queens = WQueen, Rooks = WRook, Kings = WKing, Knights = WKnight;


        if (attackedBySide == 0)
        {
            //Black pawn attacks
            Attacks = (BPawn << 7 & 0x7f7f7f7f7f7f7f7f) | (BPawn << 9 & 0xfefefefefefefefe);
            Bishops = BBishop; Queens = BQueen; Rooks = BRook; Kings = BKing; Knights = BKnight;
        }

		//Check if there is any attack on a given pos
		if ((pos & Attacks) != 0)
			return true;

		//Set all occupancy
		ulong AllOccupancy = BOcc | WOcc;

		while (pos != 0) {

			//Square index
			int index = Ops.FirstBit(pos);
            pos ^= Ops.Pow2[index];

			//Bishop attack & queen
			ulong attack = MoveGen.BishopAttack(index, AllOccupancy) & (Bishops | Queens);

			if (attack != 0)
				return true;

			//Rook attack & queen
			attack = MoveGen.RookAttack(index, AllOccupancy) & (Rooks | Queens);

			if (attack != 0)
				return true;

			//Knight attack
			attack = MoveGen.KnightAttacksDatabase[index] & Knights;

			if (attack != 0)
				return true;

			//King attack
			attack = MoveGen.KingAttacksDatabase[index] & Kings;

			if (attack != 0)
				return true;
		}


		return false;
	}

	public bool MoveWasIllegal(){
        switch (SideToPlay)
        {
            case 1:
                if (IsAttacked(BKing, 1))
                    return true;
                
                return false;
            case 0:
                if (IsAttacked(WKing, 0))
                    return true;

                return false;
            default:
                return false;
        }

	}


    public Squares IsInCheck()
    {

        if (SideToPlay == 0)
        {
            if (IsAttacked(BKing, 1))
                return (Squares)Ops.FirstBit(BKing);

        }
        else
        {
            if (IsAttacked(WKing, 0))
                return (Squares)Ops.FirstBit(WKing);
        }


        return Squares.None;
    }


    public bool IsRepetition()
    {
        int found = 0;
        for (int i = ply-1; i > ply - FiftyMove; i--) {
            if (History[i].pos == Position) {
                found++;
                if (found >= 3)
                    return true;
            }
        }
        return false;
    }

}
