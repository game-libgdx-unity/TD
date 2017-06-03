using UnityEngine;
using System.Collections;


/// <summary>
/// Holds data about the move and the position at which move was made.
/// </summary>
public struct PVEntry
{
    public int move;
    public long hash;
}

public class PVTable
{
    public PVEntry[] data;
    public int numEntries;
}