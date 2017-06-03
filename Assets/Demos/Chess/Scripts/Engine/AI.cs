using UnityEngine;
using System.Collections;
using System.Diagnostics;
    
/// <summary>
/// Computer chess analayzing.
/// </summary>
public class AI
{
    private Board board;
    public int ThinkingTime;
    private SMove[] Moves;

    private int NodesEvaluated;
    private int QNodes;
    private Stopwatch Watch;

    private float FailHigh;
    private float FailHighFirst;

    private bool RunOutOfTime;
    private string bestScore;
    private int pvMoves;

    public AI(Board board) {
        this.board = board;
        Moves = new SMove[Defs.MaxMoves * 64];
    }


    //Iterative depending, search init
    public int SearchPosition()
    {


        int bestMove = 0;
        RunOutOfTime = false;
        ClearForSearch();

        Watch = new Stopwatch();
        Watch.Start();

        for (int i = 1; i <= 20; i++) { //Max depth is 20

            Watch.Start();

            AlphaBeta(-5000000, 5000000, i);

            if (RunOutOfTime == true) { //If we runned out of time stop and print the depth we got to
                UnityEngine.Debug.Log("DEPTH " + (i-1)); //Which is depth before
                break;
            }

            board.GetPVLine(i);
            bestMove = board.PvArray[0];

            /*
            
            float order = 0;
            if (FailHigh != 0)
                order = FailHighFirst / FailHigh;
            UnityEngine.Debug.Log("Depth: " + i + " score: " + bestScore + " move:" + Move.PrintMove(bestMove) + " nodes: " + NodesEvaluated +" qnodes: " + QNodes + " ordering: " + order + " time: " + Watch.Elapsed);
            pvMoves = board.GetPVLine(i);

            string moves = "";
            for (int pv = 0; pv < pvMoves; pv++)
            {
                moves += Move.PrintMove(board.PvArray[pv]) + " ";
            }
            UnityEngine.Debug.Log(pvMoves + ":   " + moves);
            
             */


            NodesEvaluated = 0;
            QNodes = 0;

            //Watch.Stop();
        }

        Watch.Stop();

        return bestMove;
    }

    /// <summary>
    /// Prepares for the search.
    /// </summary>
    public void ClearForSearch()
    {
        for (int i = 0; i < 14; i++) {
            for(int sq=0; sq<64; sq++){
                board.SearchHistory[i][sq] = 0;
            }
        }

        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < Defs.MaxDepth; j++) {
                board.SearchKillers[i][j] = 0;
            }
        }

        board.ClearPVTable();
        FailHigh = 0;
        FailHighFirst = 0;

    }

    /// <summary>
    /// Negamax alpha beta recursion.
    /// </summary>
    public int AlphaBeta(int alpha, int beta, int depth)
    {
        if (depth == 0)
        {
            return Quiescence(alpha, beta, 63); //Try to find calm position
        }

        NodesEvaluated++;

        //If there is repetition
        if (board.IsRepetition() || board.FiftyMove >= 100)
            return 0;

        //Check time
        CheckTime();
        
        //Have we run of time yet?
        if (RunOutOfTime) {
            return 0;
        }


        int OldAlpha = alpha;
        int BestMove = 0;
        int Score = -5000000;
        int MadeALegalMove = 0;

        int add = depth * Defs.MaxMoves;
        int num = MoveGen.GenerateMoves(board, Moves, add, depth) + add;
        int move;

        //Score the move from previous search with the highest score - will be searched first
        int PvMove = board.ProbePVTable();
        if (PvMove != 0)
        {
            for (int i = add; i < num; i++) {
                if (Moves[i].move == PvMove)
                {
                    Moves[i].score = 2000000;
                }
            }
        
        }

        for (int i = add; i < num; i++)
        {
            PickBestMove(i, num);
            move = Moves[i].move;
            board.MakeMove(move);
            if (!board.MoveWasIllegal())
            {
                MadeALegalMove++;
                Score = -AlphaBeta(-beta, -alpha, depth - 1);
                board.UndoMove();

                if (Score > alpha)
                {

                    if (Score >= beta)
                    {

                        if (MadeALegalMove == 1)
                        {
                            FailHighFirst++;
                        }
                        FailHigh++;

                        if (!move.IsCapture())
                        {
                            //Not a capturing move
                            board.SearchKillers[1][depth] = board.SearchKillers[0][depth];
                            board.SearchKillers[0][depth] = move;
                        }

                        return beta;
                    }

                    if (!move.IsCapture())
                    {
                        //Not a capturing move
                        board.SearchHistory[board.pieces[move.GetFrom()]][move.GetTo()] += depth;
                    }

                    alpha = Score;
                    BestMove = move;
                }
            }
            else
                board.UndoMove();
        }


        //Check if mate
        if (MadeALegalMove == 0)
        {

            if (board.IsAttacked(board.BKing, 1))
            {
                //Mate
                if ((int)board.SideToPlay == 0) //Black
                {
                    return -32767 - depth;
                }

                return 32767 + depth;
            }
            else if (board.IsAttacked(board.WKing, 0))
            {
                //Mate
                if ((int)board.SideToPlay == 0) //Black
                {
                    return 32767 + depth;
                }

                return -32767 - depth;

            }
            else
            {
                //Stale mate
                return 0;
            }

        }

        //Store move
        if (alpha != OldAlpha) {
            board.StorePVMove(BestMove);
        }


        return alpha;
    }

    /// <summary>
    /// Searches only capturing moves.
    /// </summary>
    public int Quiescence(int alpha, int beta, int depth) {

        int Score = (Evaluation.Evaluate(board) * ((board.SideToPlay * 2) - 1));
        QNodes++;

        CheckTime();

        if (RunOutOfTime)
        {
            return 0;
        }

        if (Score >= beta) {
            return beta;
        }

        if (Score > alpha) {
            alpha = Score;
        }

        if (depth <= 21) { //Should not happen, but it's here anyway
            return Score;
        }


        int MadeALegalMove = 0;
        Score = -5000000;
        
        int add = depth * Defs.MaxMoves;
        int num = MoveGen.GenerateCapturingMoves(board, Moves, add) + add; //Generates only capturing moves
        int move;

        for (int i = add; i < num; i++)
        {
            PickBestMove(i, num);
            move = Moves[i].move;

            board.MakeMove(move);
            if (!board.MoveWasIllegal())
            {
                MadeALegalMove++;
                Score = -Quiescence(-beta, -alpha, depth - 1);

                if (Score > alpha)
                {
                    if (Score >= beta)
                    {

                        if (MadeALegalMove == 1)
                        {
                            FailHighFirst++;
                        }
                        FailHigh++;

                        board.UndoMove();
                        return beta;
                    }

                    alpha = Score;
                }
            }
            board.UndoMove();
        }


        return alpha;
    }

    /// <summary>
    /// Picks the best move from the list
    /// </summary>
    public void PickBestMove(int pos, int max) {
        SMove temp;
        int bestScore = -1;
        int bestIndex = pos;
        for (int i = pos; i < max; i++) {

            if (Moves[i].score >= bestScore) {
                //Store reference
                bestIndex = i;
                bestScore = Moves[i].score;
            }
        }
        //Set it at the bottom
        temp = Moves[pos];
        Moves[pos] = Moves[bestIndex];
        Moves[bestIndex] = temp;
    }

    /// <summary>
    /// Checks the time.
    /// </summary>
    public void CheckTime()
    {
        if (Watch.Elapsed.Seconds >= ThinkingTime)
        {
            RunOutOfTime = true;
        }
    }

}

