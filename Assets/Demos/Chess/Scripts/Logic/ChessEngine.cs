using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public enum MatchType
{
    PvP,
    PvAI
}

public enum PlayingSide
{
    Black,
    White
}

[Serializable]
public class PieceSet
{
    public Sprite Pawn;
    public Sprite Bishop;
    public Sprite Knight;
    public Sprite Rook;
    public Sprite Queen;
    public Sprite King;
    public Sprite Square;
}

public static class Extension
{
    public static void verticalFlip(this Transform tf)
    {
        Vector3 vt = tf.localScale;
        tf.localScale = new Vector3(vt.x, vt.y * -1, vt.z);
    }
}

public class ChessEngine : Engine
{
    #region Fields

    public MatchType MatchType;
    private PlayingSide CurrentTurn;

    private Vector2 origPieceSize = Vector2.zero;
    private Squares LastSquareFrom = Squares.None;
    private Squares LastSquareTo = Squares.None;
    private Squares LastSquare = Squares.None;

    [SerializeField]
    private PieceSet WhitePieces;
    [SerializeField]
    private PieceSet BlackPieces;

    [HideInInspector]
    public Transform[] SquaresObj;
    [SerializeField]
    private Transform CanvasTr;
    [SerializeField]
    private Transform root;
    [SerializeField]
    private Transform board;
    [SerializeField]
    private Transform pieces;
    [SerializeField]
    private Transform blackToMove;
    [SerializeField]
    private Transform whiteToMove;

    [SerializeField]
    private GameObject SquareObj;
    [SerializeField]
    private Sprite NoPieceSprite;

    public int ComputerThinkingTime = 3;

    public string StartingFen = FEN.Default;

    [HideInInspector]
    public int DragingFrom; //We are dragging piece from this position

    public static ChessEngine Instance;
    //UI
    public Text GameStateUI;
    public InputField TxtNotification;
    public Text PlayerName1;
    public Text PlayerName2;

    #endregion

    #region Initialization

    private void Awake()
    {
        Instance = this; // initialize the single ton
    }

    private void Start()
    {
        DragingFrom = -1; //Not dragging from any square

        SquaresObj = new Transform[64]; //Inti squares

        InitializeBoard();

        InitializeChess(StartingFen); //Calls the init in the engine class

        UpdateBoard(); //Updates the board changes

        StartCoroutine(UpdatePlayerName());

        UpdatePlayerLayout();
    }

    private void UpdatePlayerLayout()
    {
        if (PhotonNetwork.connected)
        {
            if (!PhotonNetwork.isMasterClient)
            {
                root.verticalFlip();
                foreach (Transform tf in SquaresObj)
                    tf.GetChild(0).verticalFlip();
                board.verticalFlip();
                swapPosition(blackToMove, whiteToMove);
            }
        }
    }

    private void swapPosition(Transform blackToMove, Transform whiteToMove)
    {
        Vector3 temp = blackToMove.transform.localPosition;
        blackToMove.transform.localPosition = whiteToMove.transform.localPosition;
        whiteToMove.transform.localPosition = temp;

    }

    private IEnumerator UpdatePlayerName()
    {
        if (PhotonNetwork.connected)
        {
            yield return new WaitUntil(() => PhotonNetwork.playerList[0] != null);
            if (PhotonNetwork.playerList[0].IsMasterClient)
                PlayerName1.text = PhotonNetwork.playerList[0].NickName;
            else
                PlayerName2.text = PhotonNetwork.playerList[0].NickName;

            yield return new WaitUntil(() => PhotonNetwork.playerList[1] != null);
            if (PhotonNetwork.playerList[1].IsMasterClient)
                PlayerName1.text = PhotonNetwork.playerList[1].NickName;
            else
                PlayerName2.text = PhotonNetwork.playerList[1].NickName;
        }
    }

    public void Move(int from, int to)
    {
        if (from >= 64)
            return;
        PieceMover st = SquaresObj[from].GetComponent<PieceMover>();
        if (GetPieceAt(from) != Defs.Empty)
        {
            st.PieceMouseDown(from);
            st.PieceMouseUp(to);
        }
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 64; i++)
        {
            int x = i % 8;
            int y = i / 8;

            //Create square
            GameObject obj = Instantiate(SquareObj, Vector3.zero, Quaternion.identity) as GameObject;
            obj.transform.SetParent(pieces, true); //set parent
            obj.transform.localPosition = new Vector3(-244 + 64 * x, 204 - 64 * y, 0); //Position it

            PieceMover pm = obj.GetComponent<PieceMover>(); //Get piece component
            pm.index = i; //Set index 
            pm.manager = this; //Set manager reference
            pm.UpdateBoard = UpdateBoard;

            if ((i + y) % 2 == 0) //Change sprites according to squares
            {
                obj.GetComponent<Image>().sprite = WhitePieces.Square; //White square
            }
            else
                obj.GetComponent<Image>().sprite = BlackPieces.Square; //Black square

            //Save a square in a squares array at right index
            SquaresObj[i] = obj.transform;
        }
    }

    #endregion

    #region Public Members

    public UnityEvent OnWhiteTurn; //Event
    public UnityEvent OnBlackTurn; //Event
    
    public void LeaveRoom()
    {
        SceneManager.LoadScene("launcher");
    }

    #endregion

    #region Overriden Members

    protected override void OnTurnSwitched(int from, int to) //Turn was switched
    {
        if (PhotonNetwork.connected && NetworkService.Instance)
        {
            if (NetworkService.Instance.Player.Side != SideToPlay)
            {
                NetworkService.Instance.Player.LastFrom = from;
                NetworkService.Instance.Player.LastTo = to;
            }
        }
        //Who's turn?
        if (SideToPlay == PlayingSide.White) //White
        {
            if (OnWhiteTurn != null)
                OnWhiteTurn.Invoke();
        }
        else
        {
            if (OnBlackTurn != null)
                OnBlackTurn.Invoke();

            if (MatchType == MatchType.PvAI)
            {
                ComputerPlay(ComputerThinkingTime); //Computer should play as black
            }
        }

        GameStateUI.text = Regex.Replace(GameState().ToString(), "[A-Z]", " $0"); //Space before capital letter
        TxtNotification.text = GetFen();

        //Check if in check
        Squares sq = IsInCheck();
        if (sq != Squares.None)
        { //If in check mark square
            LastSquare = sq;
            SquaresObj[(int)LastSquare].gameObject.GetComponent<Image>().color = new Color32(255, 15, 15, 255);
        }
        else if (LastSquare != Squares.None)
        {//Remove last marked square
            SquaresObj[(int)LastSquare].gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    protected override void OnComputerPlayed(int from, int to)
    {
        //Mark the squares that piece traveled
        SquaresObj[(int)from].gameObject.GetComponent<Image>().color = new Color32(73, 35, 0, 255);
        SquaresObj[(int)to].gameObject.GetComponent<Image>().color = new Color32(73, 30, 0, 255);

        //Reset colors
        if ((int)LastSquareFrom != from && (int)LastSquareFrom != to && LastSquareFrom != Squares.None)
        {
            SquaresObj[(int)LastSquareFrom].gameObject.GetComponent<Image>().color = Color.white;
        }

        if ((int)LastSquareTo != to && (int)LastSquareTo != from && LastSquareTo != Squares.None)
        {
            SquaresObj[(int)LastSquareTo].gameObject.GetComponent<Image>().color = Color.white;
        }

        //Store marked squares
        LastSquareFrom = (Squares)from;
        LastSquareTo = (Squares)to;

        //Since board was changed by computer update board
        UpdateBoard();
    }

    #endregion

    #region Private Members

    private void UpdateBoard()
    {
        //Initialize grid
        for (int i = 0; i < 64; i++)
        {
            int piece = GetPieceAt(i);

            switch (piece)
            {
                //Button will shrink texture, which creates much better 'piece' effect
                case Defs.WPawn: CreatePiece(WhitePieces.Pawn, i); break;
                case Defs.WBishop: CreatePiece(WhitePieces.Bishop, i); break;
                case Defs.WKnight: CreatePiece(WhitePieces.Knight, i); break;
                case Defs.WRook: CreatePiece(WhitePieces.Rook, i); break;
                case Defs.WQueen: CreatePiece(WhitePieces.Queen, i); break;
                case Defs.WKing: CreatePiece(WhitePieces.King, i); break;
                case Defs.BPawn: CreatePiece(BlackPieces.Pawn, i); break;
                case Defs.BBishop: CreatePiece(BlackPieces.Bishop, i); break;
                case Defs.BKnight: CreatePiece(BlackPieces.Knight, i); break;
                case Defs.BRook: CreatePiece(BlackPieces.Rook, i); break;
                case Defs.BQueen: CreatePiece(BlackPieces.Queen, i); break;
                case Defs.BKing: CreatePiece(BlackPieces.King, i); break;
                case Defs.Empty:
                    CreatePiece(NoPieceSprite, i);
                    break;
            }

        }
    }

    private void Undo()
    {
        //Calls the engine undo move
        UndoMove();

        //Reset color
        SquaresObj[(int)LastSquareFrom].gameObject.GetComponent<Image>().color = Color.white;
        SquaresObj[(int)LastSquareTo].gameObject.GetComponent<Image>().color = Color.white;

        //Sinde the board has changed update the move
        UpdateBoard();
    }

    private void CreatePiece(Sprite piece, int index, float scale = 1f)
    {
        Image image = SquaresObj[index].GetChild(0).gameObject.GetComponent<Image>();

        //Get the size of the first piece and store it 
        if (origPieceSize == Vector2.zero)
            origPieceSize = image.rectTransform.sizeDelta;

        image.rectTransform.sizeDelta = origPieceSize * scale;
        image.sprite = piece;

    }

    #endregion
}
