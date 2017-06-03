using System.Collections;
using System.Collections.Generic;
using UnitedSolution;
using UnitedSolution;using UnityEngine;
using DG.Tweening;


public class TestGameSetting : LazyBehaviour
{
    public int testInt = 5; //default value
    public bool testBool = false; //default value
    public string testString = "empty"; //default value
    public float testFloat = .5f; //default value

    // Use this for initialization
    void Awake()
    {        
        int j = GameSetting.Instance.Get<int>("Test");
        Debug.Log("j :" + j);

        j += 4;

        GameSetting.Instance.Set<int>("Test", j);
        GameSetting.Instance.Save();
    }
}
