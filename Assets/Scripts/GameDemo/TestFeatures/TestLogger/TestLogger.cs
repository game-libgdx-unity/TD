using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitedSolution
{
    public class TestLogger : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        { 
        }

        // Update is called once per frame
        void Update()
        {
            this.debug("Time now: " + DateTime.Now.ToShortTimeString());
            this.info("Time now: " + DateTime.Now.ToShortTimeString());
            this.trace("Time now: " + DateTime.Now.ToShortTimeString());
            this.warn("Time now: " + DateTime.Now.ToShortTimeString());
            this.error("Time now: " + DateTime.Now.ToShortTimeString());
        }
    }
}