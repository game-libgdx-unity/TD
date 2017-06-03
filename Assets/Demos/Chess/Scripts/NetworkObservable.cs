using UnityEngine;
using System.Collections;
using System;

public class NetworkObservable : MonoBehaviour, IPunObservable
{

    #region Fields
    private bool dirty = false;
    private int lastFrom;
    private int lastTo;
    #endregion

    #region Public Members

    public PlayingSide Side
    {
        get
        {
            return PhotonNetwork.isMasterClient ? PlayingSide.White : PlayingSide.Black;
        }
    }

    public int LastFrom
    {
        get
        {
            return lastFrom;
        }

        set
        {
            if (value != lastFrom)
            {
                lastFrom = value;
                dirty = true;
            }
        }
    }

    public int LastTo
    {
        get
        {
            return lastTo;
        }

        set
        {
            if (value != lastTo)
            {
                lastTo = value;
                dirty = true;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.isWriting)
        {
            if (dirty)
            {
                // We own this player: send the others our data
                stream.SendNext(lastFrom);
                stream.SendNext(lastTo);

                Debug.Log("Send data: " + lastFrom + ":" + lastTo);
                dirty = false;
            }
        }
        else
        {
            // Network player, receive data
            int lastFrom = (int)stream.ReceiveNext();
            int lastTo = (int)stream.ReceiveNext();

            ChessEngine.Instance.Move(lastFrom, lastTo);

            Debug.Log("Receive data: " + lastFrom + ":" + lastTo);
        }
    }
    #endregion
}

