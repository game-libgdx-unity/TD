using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;


namespace UnitedSolution {
	
	[RequireComponent (typeof (Animator))]
	public class UnitTowerAnimation : MonoBehaviour {

		public UnitTower tower;
		
		//for mecanim
		[HideInInspector] public Animator anim;
		public AnimationClip clipConstruct;
		public AnimationClip clipDeconstruct;
		public AnimationClip clipShoot;	
		
		
		public bool enableShoot=false;
		public bool enableConstruct=false;
		public bool enableDeconstruct=false;
		
		
		public float shootDelay=0;
		
		
		void Start(){
			anim=gameObject.GetComponent<Animator>();
			if(anim!=null){
				if(enableShoot) tower.playShootAnimation=this.PlayShoot;
				if(enableConstruct) tower.playConstructAnimation=this.PlayConstruct;
				if(enableDeconstruct) tower.playDeconstructAnimation=this.PlayDeconstruct;
				
				AnimatorOverrideController overrideController = new AnimatorOverrideController();
				overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
		 
				overrideController["DefaultTowerConstruct"] = clipConstruct!=null ? clipConstruct : null;
				overrideController["DefaultTowerDeconstruct"] = clipDeconstruct!=null ? clipDeconstruct : null;
				overrideController["DefaultTowerShoot"] = clipShoot!=null ? clipShoot : null;
				
				anim.runtimeAnimatorController = overrideController;
			}
		}
		
		
		void OnEnable(){
			
		}
		
		
		public float PlayShoot(){
			anim.SetTrigger("Shoot");
			return shootDelay;
		}
		public void PlayConstruct(){
			anim.SetTrigger("Construct");
		}
		public void PlayDeconstruct(){
			anim.SetTrigger("Deconstruct");
		}
		
		
	}
	
}
