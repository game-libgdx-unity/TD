using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class for useful operations when working with the Graph API
/// <summary>
/// Help with Facebook Graph API
/// </summary>
public class GraphUtil : ScriptableObject
{
    // Generate Graph API query for a user/friend's profile picture
    public static string GetPictureQuery(string facebookID, int? width = null, int? height = null, string type = null, bool onlyURL = false)
    {
        string query = string.Format("/{0}/picture", facebookID);
        string param = width != null ? "&width=" + width.ToString() : "";
        param += height != null ? "&height=" + height.ToString() : "";
        param += type != null ? "&type=" + type : "";
        if (onlyURL) param += "&redirect=false";
        if (param != "") query += ("?g" + param);
        return query;
    }

    // Download an image using WWW from a given URL
    public static void LoadImgFromURL (string imgURL, Action<Texture> callback)
    {
        // Need to use a Coroutine for the WWW call, using Coroutiner convenience class
        Coroutiner.StartCoroutine(
            LoadImgEnumerator(imgURL, callback)
        );
    }
    
    public static IEnumerator LoadImgEnumerator (string imgURL, Action<Texture> callback)
    {
        WWW www = new WWW(imgURL);
        yield return www;
        
        if (www.error != null)
        {
            Debug.LogError(www.error);
            yield break;
        }
        callback(www.texture);
    }

    public static string DeserializePictureURL(object userObject)
    {
        // friendObject JSON format in this situation
        // {
        //   "first_name": "Chris",
        //   "id": "10152646005463795",
        //   "picture": {
        //      "data": {
        //          "url": "https..."
        //      }
        //   }
        // }
        var user = userObject as Dictionary<string, object>;

        object pictureObj;
        if (user.TryGetValue("picture", out pictureObj))
        {
            var pictureData = (Dictionary<string, object>)(((Dictionary<string, object>)pictureObj)["data"]);
            return (string)pictureData["url"];
        }
        return null;
    }

    // Pull out score from a JSON user entry object constructed in FBGraph.GetScores()
    public static int GetScoreFromEntry(object obj)
    {
        Dictionary<string,object> entry = (Dictionary<string,object>) obj;
        return Convert.ToInt32(entry["score"]);
    }
}
