using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateBehavviour : StateMachineBehaviour
{
    bool initialized = false;
    UnitStat stat;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!initialized)
        {
            this.stat = animator.GetComponent<UnitStat>();
            initialized = true;
        }
        stat.OnIdleStateEnter();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stat.OnIdleStateExit();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        stat.OnIdleStateUpdate();
    }
}
