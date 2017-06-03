using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour
{

    private Board board; //Chess board
    public SMove[] Moves { get; private set; } //Possible moves
    public PromotePiece PromoteTo = PromotePiece.Queen;

    private AI search; //Reference to the search

    /// <summary>
    /// Initializes chess with a default fen.
    /// </summary>
	public void InitChess()
    {
        InitializeChess("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 "); //Init chess with default FEN
    }

    /// <summary>
    /// Initializes chess with the custom fen.
    /// </summary>
    public void InitializeChess(string FEN)
    {
        Magics.Init(); //Initialize magics ;)
        Zobrist.Init(); //Initiliaze zobrist
        MvvLva.Init();
        //Initialize chess
        board = new Board(FEN);
        Moves = new SMove[256];
        search = new AI(board);

        OnTurnSwitched(64,64); //Call it on start

    }

    /// <summary>
    /// Try to make a move from index to index. Returns false if failed.
    /// </summary>
	public bool TryToMove(int From, int To)
    {
        int num = MoveGen.GenerateMoves(board, Moves);
        for (int i = 0; i < num; i++)
        {
            if (Moves[i].move.GetFrom() == From && Moves[i].move.GetTo() == To)
            {
                if (Moves[i].move.IsPromotion())
                {
                    if (Moves[i].move.GetPromo() == (int)PromoteTo)
                    {
                        return MakeMove(Moves[i].move);
                    }
                }
                else
                {
                    return MakeMove(Moves[i].move);
                }
            }
        }
        return false;
    }

    //Make a move
    private bool MakeMove(int move)
    {
        bool madeAMove = board.MakeMoveWithCheck(move);

        if (madeAMove)
        {
            OnTurnSwitched(move.GetFrom(), move.GetTo());
        }

        return madeAMove;
    }

    /// <summary>
    /// Plays the move with the small delay.
    /// </summary>
    private IEnumerator PlayMove(int thinkingTime)
    {
        yield return null;
        search.ThinkingTime = thinkingTime;
        int myMove = search.SearchPosition();
        board.MakeMoveWithCheck(myMove);
        OnComputerPlayed(myMove.GetFrom(), myMove.GetTo());
        OnTurnSwitched(myMove.GetFrom(), myMove.GetTo());
    }


    /// <summary>
    /// Call if you want a computer to make a move.
    /// </summary>
    public void ComputerPlay(int thinkingTime)
    {
        StartCoroutine(PlayMove(3));
    }


    /// <summary>
    /// Called when turn was switched.
    /// </summary>
    protected virtual void OnTurnSwitched(int from, int to)
    {

    }

    /// <summary>
    /// Called when computer has played.
    /// </summary>
    protected virtual void OnComputerPlayed(int from, int to)
    {

    }

    /// <summary>
    /// Whos turn?
    /// </summary>
    public PlayingSide SideToPlay
    {
        get
        {
            return (PlayingSide)board.SideToPlay;
        }
    }

    /// <summary>
    /// Is the king in check?
    /// </summary>
    public Squares IsInCheck()
    {
        return board.IsInCheck();
    }

    /// <summary>
    /// Get's the piece at the given position.
    /// </summary>
    public int GetPieceAt(int index)
    {
        return board.GetPieceAt(index);
    }

    /// <summary>
    /// Returns game state.
    /// </summary>
    public BoardState GameState()
    {
        return board.State;
    }

    /// <summary>
    /// All playable positions as one ulong.
    /// </summary>
	public ulong GetPlayablePositions(int From)
    {
        int num = MoveGen.GenerateMoves(board, Moves);
        ulong pos = 0;

        for (int i = 0; i < num; i++)
        {
            if (Moves[i].move.GetFrom() == From)
            {
                pos |= Ops.Pow2[(int)Moves[i].move.GetTo()];
            }
        }


        return pos;

    }

    /// <summary>
    /// Get the fen from the board.
    /// </summary>
    public string GetFen()
    {
        return FEN.FenFromBoard(board);
    }

    /// <summary>
    /// Undo last board move.
    /// </summary>
    public void UndoMove()
    {
        board.UndoMove();
    }


}
