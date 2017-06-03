using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class LoadFriends : MonoBehaviour
{
    public Text defaultText_FriendList;
    public Transform contentT;
    public Text[] players;

    void Start()
    {
        gameObject.SetActive(false);
    }
    // Use this for initialization
    void Load()
    {
        print("Friends :" + FacebookHandler.Instance.Friends.Count);
        if (FBGraph.IsFriendListAvailable())
        {
            Destroy(defaultText_FriendList.gameObject);
            foreach (object friend in FacebookHandler.Instance.Friends)
            {
                string name = (friend as Dictionary<string, object>)["name"] as string;
                GameObject textObj = new GameObject(name);
                Text t = textObj.AddComponent<Text>();
                t.text = name;
                textObj.transform.parent = contentT;
            }
        }
        else
        {
            defaultText_FriendList.text = "Invite your friends!";
            FBGraph.Invite();
        }
    }

    void LoadScore()
    {
        List<object> friends = FacebookHandler.Instance.Friends;
        print("Friends :" + friends.Count);
        if (FBGraph.IsFriendListAvailable())
        {
            Destroy(defaultText_FriendList.gameObject);
            for (int i = 0; i < players.Length; i++)
            {
                players[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < friends.Count; i++)
            {
                var score = ((friends[i] as Dictionary<string, object>)["score"]) as string;
                var user = ((Dictionary<string, object>)friends[i])["user"] as Dictionary<string, object>;
                string name = ((user as Dictionary<string, object>)["name"]) as string;
                score = "Stage " + score + " " + name;
                Debug.Log(score);
                players[i].gameObject.SetActive(true);
                players[i].text = score.ToString();

            }
        }
    }

    void OnEnable()
    {
        FBGraph.OnFriendListFetched += OnFriendListFetched;
    }

    void OnDisable()
    {
        FBGraph.OnFriendListFetched -= OnFriendListFetched;

    }

    private void OnFriendListFetched(List<object> obj)
    {
        //Load();
        LoadScore();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
