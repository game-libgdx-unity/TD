using UnityEngine;
using System.Collections;
using System;

public class PieceMover : MonoBehaviour
{

    /// <summary>
    /// Reference.
    /// </summary>
    public ChessEngine manager;

    /// <summary>
    /// Square index from 0-63.
    /// </summary>
    public int index;

    /// <summary>
    /// Reference to the moving piece.
    /// </summary>
    public Transform piece;

    /// <summary>
    /// Are we dragging the piece?
    /// </summary>
    public bool isDragging = false;

    public Action UpdateBoard { get; set; }

    public void Update()
    {

        if (isDragging)
        {
            piece.transform.position = Input.mousePosition;
        }
    }


    public void OnMouseDownEvent()
    {
        if (ChessEngine.Instance.MatchType == MatchType.PvP)
        {
            if (manager.SideToPlay != NetworkService.Instance.Player.Side)
            {
                return;
            }
        }
        //Check if there is a piece on this position
        if (manager.GetPieceAt(index) != Defs.Empty)
        {
            PieceMouseDown(index);
        }
    }

    public void PieceMouseDown(int index)
    {
        //No longer parented to the square
        piece = transform.GetChild(0);
        piece.SetParent(transform.parent, true);

        //We are dragging from here
        manager.DragingFrom = index;
        isDragging = true;
    }

    public void OnMouseUpEvent()
    {

        //Only if we are dragging
        if (!isDragging)
            return;

        //Try to get index at which square this peice got
        int x = Mathf.Abs(Mathf.RoundToInt((-224 - piece.transform.localPosition.x) / 64)); //Left to right
        int y = 7 - Mathf.Abs(Mathf.RoundToInt((-224 - piece.transform.localPosition.y) / 64)); //From top down to the bottom

        //Get index
        int myindex = x + y * 8;
        //Try to make the move
        PieceMouseUp(myindex);
        transform.localScale = Vector3.one;
    }

    public void PieceMouseUp(int myindex)
    {
        if (manager.TryToMove(manager.DragingFrom, myindex))
        {
            //We made the move

            MovePiece(manager.SquaresObj[myindex]); //Update the piece position
            if (UpdateBoard != null)
            {
                UpdateBoard();
            }
        }
        else
            ReturnPiece(); //Return piece to the original square

        //We are not dragging anymore
        manager.DragingFrom = -1;
        isDragging = false;
    }

    /// <summary>
    /// Returns piece to the original square.
    /// </summary>
    private void ReturnPiece()
    {
        if (piece)
        {
            piece.SetParent(manager.SquaresObj[manager.DragingFrom], false);
            piece.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Moves the piece to the new square.
    /// </summary>
    public void MovePiece(Transform to)
    {
        var t = to.GetChild(0); //Get the piece from the square we are trying to move the piece to
        if (t)
        { //If there is a piece

            //Switch it
            //EACH SQUARE MUST HAVE PIECE AS A CHILD
            t.SetParent(transform, true);
            t.localPosition = Vector3.zero;
        }

        //Our piece that was moved should also have it parent changed
        piece.SetParent(to, true);
        piece.localPosition = Vector3.zero;

    }

}
