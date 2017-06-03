
using UnityEngine;
using System;
using System.Collections.Generic;
using Facebook.Unity;

/// <summary>
/// Work with Facebook Graph API
/// </summary>
public static class FBGraph
{
    #region PlayerInfo

    public static void GetPlayerInfo()
    {
        string queryString = "/me?fields=id,first_name,picture.width(120).height(120)";
        FB.API(queryString, HttpMethod.GET, GetPlayerInfoCallback);
        //GetFriends();
    }

    private static void GetPlayerInfoCallback(IGraphResult result)
    {
        Debug.Log("GetPlayerInfoCallback");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);

        // Save player name
        string name;
        if (result.ResultDictionary.TryGetValue("first_name", out name))
        {
            FacebookUI.UserName = name;
        }
        int id;
        if (result.ResultDictionary.TryGetValue("id", out id))
        {
            FacebookUI.ID = id;
        }

        //Fetch player profile picture from the URL returned
        string playerImgUrl = GraphUtil.DeserializePictureURL(result.ResultDictionary);
        GraphUtil.LoadImgFromURL(playerImgUrl, delegate (Texture pictureTexture)
        {
            // Setup the User's profile picture
            if (pictureTexture != null)
            {
                FacebookUI.UserPicture = pictureTexture;
            }
            // Redraw the UI
            if (OnFacebookLoggedInUpdated != null)
                OnFacebookLoggedInUpdated();
        });
    }
    public static Action OnFacebookLoggedInUpdated;

    public static void GetPlayerPicture()
    {
        FB.API(GraphUtil.GetPictureQuery("me", 128, 128), HttpMethod.GET, delegate (IGraphResult result)
        {
            Debug.Log("PlayerPictureCallback");
            if (result.Error != null)
            {
                Debug.LogError(result.Error);
                return;
            }
            if (result.Texture == null)
            {
                Debug.Log("PlayerPictureCallback: No Texture returned");
                return;
            }

            // Setup the User's profile picture
            FacebookUI.UserPicture = result.Texture;

            // Redraw the UI
            if (OnFacebookLoggedInUpdated != null)
                OnFacebookLoggedInUpdated();
        });
    }
    #endregion

    #region Friends

    public static void Invite()
    {
        FB.Mobile.AppInvite(new System.Uri("https://fb.me/400236157029312"), new System.Uri("http://brainivore.com/Images/Logo.png"), InviteCallback);
    }

    static void InviteCallback(IAppInviteResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Invite cancelled.");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error in invite:" + result.Error);
        }
        else
        {
            Debug.Log("Invite was successful:" + result.RawResult);
        }
    }

    public static void GetFriends()
    {
        string queryString = "/me/friends";
        FB.API(queryString, HttpMethod.GET, GetFriendsCallback);
    }

    private static void GetFriendsCallback(IGraphResult result)
    {
        Debug.Log("GetFriendsCallback");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);
        // Store /me/friends result
        object dataList;
        if (result.ResultDictionary.TryGetValue("data", out dataList))
        {
            var friendsList = (List<object>)dataList;

            CacheFriends(friendsList);

            if (OnFriendListFetched != null)
            {
                OnFriendListFetched(friendsList);
            }
        }
    }

    public static Action<List<object>> OnFriendListFetched;

    public static void GetInvitableFriends()
    {
        string queryString = "/me/invitable_friends?fields=id,name&limit=100";
        FB.API(queryString, HttpMethod.GET, GetInvitableFriendsCallback);
    }

    private static void GetInvitableFriendsCallback(IGraphResult result)
    {
        Debug.Log("GetInvitableFriendsCallback");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);

        // Store /me/invitable_friends result
        object dataList;
        if (result.ResultDictionary.TryGetValue("data", out dataList))
        {
            var invitableFriendsList = (List<object>)dataList;
            CacheFriends(invitableFriendsList);

            if (OnFriendListFetched != null)
            {
                OnFriendListFetched(invitableFriendsList);
            }
        }
    }
    public static bool HavePublishActions
    {
        get
        {
            return (FB.IsLoggedIn &&
                   (AccessToken.CurrentAccessToken.Permissions as List<string>).Contains("publish_actions")) ? true : false;
        }
        private set { }
    }

    private static void CacheFriends(IList<object> newFriends)
    {
        Debug.Log("newFriends: " + newFriends.Count);

        if (IsFriendListAvailable())
        {
            FacebookHandler.Instance.Friends.AddRange(newFriends);
        }
        else
        {
            FacebookHandler.Instance.Friends = (List<object>)newFriends;
        }
    }

    public static bool IsFriendListAvailable()
    {
        return FacebookHandler.Instance.Friends != null && FacebookHandler.Instance.Friends.Count > 0;
    }
    #endregion

    #region Scores

    public static void PostScore(int score, Action callback = null)
    {
        var query = new Dictionary<string, string>();
        query["score"] = score.ToString();
        FB.API(
            "/me/scores",
            HttpMethod.POST,
            delegate (IGraphResult result)
            {
                Debug.Log("PostScore Result: " + result.RawResult);
                // Fetch fresh scores to update UI
                FBGraph.GetScores();
            },
        query
        );
    }

    public static void GetScores()
    {
        FB.API("/app/scores?fields=score,user.limit(20)", HttpMethod.GET, GetScoresCallback);
    }

    private static void GetScoresCallback(IGraphResult result)
    {
        Debug.Log("GetScoresCallback");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);

        // Parse scores info
        var scoresList = new List<object>();

        object scoresh;
        if (result.ResultDictionary.TryGetValue("data", out scoresh))
        {
            scoresList = (List<object>)scoresh;
        }

        // Parse score data
        HandleScoresData(scoresList);

        CacheFriends(scoresList);

        if (OnFriendListFetched != null)
        {
            OnFriendListFetched(scoresList);
        }
    }

    private static void HandleScoresData(List<object> scoresList)
    {
        var structuredScores = new List<object>();
        foreach (object scoreItem in scoresList)
        {
            var entry = (Dictionary<string, object>)scoreItem;
            var user = (Dictionary<string, object>)entry["user"];
            string userId = (string)user["id"];

            if (string.Equals(userId, AccessToken.CurrentAccessToken.UserId))
            {
                // This entry is the current player
                int playerHighScore = GraphUtil.GetScoreFromEntry(entry);
                Debug.Log("Local players score on server is " + playerHighScore);

                entry["score"] = playerHighScore.ToString();
                FacebookHandler.Instance.HighScore = playerHighScore;
            }

            structuredScores.Add(entry);
            if (!FacebookHandler.Instance.FriendImages.ContainsKey(userId))
            {
                // We don't have this players image yet, request it now
                LoadFriendImgFromID(userId, pictureTexture =>
               {
                   if (pictureTexture != null)
                   {
                       FacebookHandler.Instance.FriendImages.Add(userId, pictureTexture);
                   }
               });
            }
        }

        FacebookHandler.Instance.Scores = structuredScores;
        // Redraw the UI
        if (OnFacebookLoggedInUpdated != null)
            OnFacebookLoggedInUpdated();
    }

    // Graph API call to fetch friend picture from user ID returned from FBGraph.GetScores()
    //
    // Note: /me/invitable_friends returns invite tokens instead of user ID's,
    // which will NOT work with this /{user-id}/picture Graph API call.
    private static void LoadFriendImgFromID(string userID, Action<Texture> callback)
    {
        FB.API(GraphUtil.GetPictureQuery(userID, 128, 128),
               HttpMethod.GET,
               delegate (IGraphResult result)
        {
            if (result.Error != null)
            {
                Debug.LogError(result.Error + ": for friend " + userID);
                return;
            }
            if (result.Texture == null)
            {
                Debug.Log("LoadFriendImg: No Texture returned");
                return;
            }
            callback(result.Texture);
        });
    }
    #endregion
}
