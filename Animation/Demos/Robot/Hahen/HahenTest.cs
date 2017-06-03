using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RobotDemo
{
    public class HahenTest : MonoBehaviour {

        // Use this for initialization
        void Start() {
            StartCoroutine(loop());
        }

        // Update is called once per frame
        IEnumerator loop() {
            while (true)
            {
                HahenRenderer.Instance.Invoke(CV.Vector3Zero);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}