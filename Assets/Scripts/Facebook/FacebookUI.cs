using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class FacebookUI : MonoBehaviour
{

    public static Texture UserPicture;
    public static string UserName;
    public RawImage profilePic;
    public Text playerName;

    public static int ID;

    void UpdateUI()
    {
        print("UpdateUI");
        if (UserPicture)
            profilePic.texture = UserPicture;

        playerName.text = UserName;
    }

    void OnEnable()
    {
        FBGraph.OnFacebookLoggedInUpdated += UpdateUI; 
    }

    void OnDisable()
    {
        FBGraph.OnFacebookLoggedInUpdated -= UpdateUI;
    }
}
