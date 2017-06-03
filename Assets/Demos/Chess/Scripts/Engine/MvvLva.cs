using UnityEngine;
using System.Collections;

/// <summary>
/// Most valuable victim / Least valuable attacker. Used for better move ordering. Pawn takes queen is the best move.
/// </summary>
public static class MvvLva {

    /// <summary>
    /// Lookup table.
    /// </summary>
    public static int[][] Table;

    /// <summary>
    /// Victim sqore - none, pawn, knight, bishop, rook, queen, king, then black peices.....
    /// </summary>
    public static int[] VictimScore = new int[13] { 0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600}; 

    /// <summary>
    /// Initializes the lookup table.
    /// </summary>
    public static void Init() {

        Table = new int[14][];
        for (int i = 0; i<Table.Length; i++) {
            Table[i] = new int[13];
        }

        for (int attacker = 0; attacker < 13; attacker++) {
            for (int victim = 0; victim < 13; victim++)
            {
                Table[victim][attacker] = VictimScore[victim] + 6 - (VictimScore[attacker] / 100);
            }
        }
    }

}
