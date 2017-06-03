using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using DG.Tweening;
using UnitedSolution;
/// <summary>
/// Handle Facebook from game, unity code, unity API
/// </summary>
public class FacebookHandler : SingletonBehaviour<FacebookHandler>
{
    //   Game State   // 
    private int? highScore;
    public int HighScore
    {
        get { return Instance.highScore.HasValue ? Instance.highScore.Value : 0; }
        set { Instance.highScore = value; }
    }
    public List<object> Friends;
    public Dictionary<string, Texture> FriendImages = new Dictionary<string, Texture>();
    public List<object> InvitableFriends = new List<object>();
    // Scores
    public bool ScoresReady;
    private List<object> scores;
    public List<object> Scores
    {
        get { return scores; }
        set { scores = value; ScoresReady = true; }
    }



    public FacebookUI facebookUI;
    public enum WhatToDo { Nothing, Share, Invite }
    public WhatToDo TO_DO = WhatToDo.Nothing;
    private static FacebookHandler instance;
    public static FacebookHandler Instance
    {
        get
        {
            if (!instance)
            {
                GameObject container = new GameObject("FacebookHandle");
                container.AddComponent<FacebookHandler>();
                DontDestroyOnLoad(container);
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    // Awake function from Unity's MonoBehavior
   protected override void Awake()
    {
        base.Awake();

        if (instance != null)
        {
            DestroyImmediate(this);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
            Instance = this;
            InitializeFacebook();
        }

    }
    private void InitializeFacebook()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(FBSDK_InitializationCallback, OnUnityDeactivated);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    /// <summary>
    /// Begin Logging to facebook
    /// </summary>
    public void FBLogin()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }

            FBGraph.GetPlayerInfo();

            if (TO_DO == WhatToDo.Invite)
                FBGraph.GetFriends();
            else if (TO_DO == WhatToDo.Share)
                FacebookHandler.Instance.ShareLink();
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
    /// <summary>
    /// Begin Logging to facebook with the publish permission
    /// </summary>
    /// <param name="result"></param>
    public void FBLoginForLeaderboard(FacebookDelegate<ILoginResult> result)
    {
        FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, result);
    }
    /// <summary>
    /// Share a link on Facebook
    /// </summary>
    public void ShareLink()
    {
        if (FB.IsLoggedIn)
        {
            FB.ShareLink(
            new Uri("https://play.google.com/store/apps/details?id=com.vinh.tap"),
            "Pet Up Up!!!",
            "I'm playing this great game!",
            callback: ShareCallback
             );
        }
        else
        {
            FacebookHandler.Instance.FBLogin();
            TO_DO = WhatToDo.Share;
        }
    }
    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink Error: " + result.Error);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log(result.PostId);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("ShareLink success!");
        }
    }
    private void FBSDK_InitializationCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    private void OnUnityDeactivated(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1f;
        }
    }
}
