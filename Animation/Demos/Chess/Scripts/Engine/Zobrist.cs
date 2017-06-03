using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Zobrist hashing.
/// </summary>
public static class Zobrist {

    public static long[][] Pieces; // Square | Pieces
    public static long[] CastleRights; //16 different cases
    public static long[] SideToPlay; //Only for one side
    public static long[] EPSquares; //Can occur only on 18 squares but will fill be fileld with all 64 squares since it's easier

    /// <summary>
    /// Initializes the values.
    /// </summary>
    public static void Init() {

        System.Random rand = new System.Random(700); //Same generation everywhere, in case of openinig book
        Pieces = new long[64][];
        CastleRights = new long[16];
        EPSquares = new long[64];
        SideToPlay = new long[2];

        //Castle rights
        for (int c = 0; c < 16; c++)
        {
            CastleRights[c] = rand.NextInt64();
        }

        //Every square
        for (int i = 0; i < 64; i++) {

            Pieces[i] = new long[13];

            //For every piece
            for (int p = 0; p < Pieces[i].Length; p++)
            {
                Pieces[i][p] = rand.NextInt64();
            }

            EPSquares[i] = rand.NextInt64();
        }


        SideToPlay[0] = rand.NextInt64();
        SideToPlay[1] = rand.NextInt64();

    }

    /// <summary>
    /// Get almost unique hash position.
    /// </summary>
    public static long GetHashPosition(this Board board) {
        long pos = 0;

        //Get data from pieces array
        for (int i = 0; i < 64; i++) {
            pos ^= Pieces[i][board.pieces[i]];
        }

        //Side
        pos ^= SideToPlay[board.SideToPlay];

        //EP
        if (board.EnPassantSq != Squares.None)
            pos ^= EPSquares[(int)board.EnPassantSq];

        return pos;
    }

    /// <summary>
    /// Random number generator.
    /// </summary>
    public static long NextInt64(this System.Random rnd)
    {
        var buffer = new byte[sizeof(Int64)];
        rnd.NextBytes(buffer);
        return Math.Abs(BitConverter.ToInt64(buffer, 0));
    }

}
