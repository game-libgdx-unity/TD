using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Analayzing function. When I last tested the reasults were ok. 
/// </summary>
public class Perft {


	private Board board;
	private Stopwatch watch;
    private SMove[] Moves;

    /// <summary>
    /// How many moves/nodes are there at the fixed depth.
    /// </summary>
	public Perft(Board board, int Depth){
		this.board = board;
        Moves = new SMove[Defs.MaxMoves * (Depth + 1)];

		watch = new Stopwatch();
		watch.Start();
		int number = Analayze(Depth);
		UnityEngine.Debug.Log("Nodes:" + number);
		watch.Stop();
		UnityEngine.Debug.Log("Analyzing delay: " + watch.Elapsed);
	}


    /// <summary>
    /// Recursive analyzing.
    /// </summary>
	private int Analayze(int depth){
		int nodes = 0;
		if(depth == 0) return 1;
        int add = depth * Defs.MaxMoves;
        int num = MoveGen.GenerateMoves(board, Moves, add) + add;
        int move;
        for (int i = add; i < num; i++)
        {
            move = Moves[i].move;
			board.MakeMove(move);
			if(!board.MoveWasIllegal()){
				nodes += Analayze(depth-1);
			}
			board.UndoMove();
		}
		return nodes;
	}



}
