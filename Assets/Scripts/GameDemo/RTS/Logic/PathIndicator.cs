using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnitedSolution;

namespace UnitedSolution {

	public class PathIndicator : MonoBehaviour {
		
		public PathTD path;
		public ParticleSystem pSystem;
		private Transform indicatorT;
		
		public float delayBeforeStart=2;
		
		public float speed=5;
		public float updateRate=0.1f;
		
		private List<Vector3> subPath=new List<Vector3>();
		private int waypointID=1;
		private int subWaypointID=0;
		
		
		
		// Use this for initialization
		void Start () {
			indicatorT=pSystem.transform;
			pSystem.emissionRate=0;
			
			StartCoroutine(Move());
		}
		
		IEnumerator EmitRoutine(){
			while(true){
				yield return new WaitForSeconds(updateRate);
				pSystem.startRotation=(indicatorT.rotation.eulerAngles.y)*Mathf.Deg2Rad;
				pSystem.Emit(1);
			}
		}
		
		
		IEnumerator Move(){
			Reset(true);
			
			yield return new WaitForSeconds(delayBeforeStart);
			
			StartCoroutine(EmitRoutine());
			
			while(true){
				//move to next point, return true if reach
				if(MoveToPoint(indicatorT, subPath[subWaypointID])){
					subWaypointID+=1;								//sub waypoint reach, get the next subwaypoint
					if(subWaypointID>=subPath.Count){		//if reach subpath destination, get subpath for next waypoint
						subWaypointID=0;
						waypointID+=1;
						if(waypointID>=path.GetPathWPCount()){	//if reach path destination, reset to starting pos
							Reset();
						}
						else{													//else get next subpath
							subPath=path.GetWPSectionPath(waypointID);
						}
					}
				}
				
				yield return null;
			}
		}
		//more the indicator transform
		public bool MoveToPoint(Transform particleT,Vector3 point){
			float dist=Vector3.Distance(point, indicatorT.position);
			
			indicatorT.LookAt(point);
			indicatorT.Translate(Vector3.forward*Mathf.Min(speed*Time.deltaTime, dist));
			
			if(dist<0.1f) return true;
			
			return false;
		}
		
		
		//when there's a change in path
		void OnEnable(){
			SubPath.onPathChangedE += OnSubPathChanged;
		}
		void OnDisable(){
			SubPath.onPathChangedE -= OnSubPathChanged;
		}
		void OnSubPathChanged(SubPath platformSubPath){
			if(platformSubPath.parentPath==path && platformSubPath.wpIDPlatform==waypointID){
				subPath=path.GetWPSectionPath(waypointID);
				subWaypointID=Mathf.Min(subWaypointID, subPath.Count-1);
			}
		}
		
		//flag passed indicate initial reset, only true in the first call
		void Reset(bool initial=false){
			//if use path-looping, use loop point otherwise use the starting point
			if(path.loop && !initial) waypointID=path.GetLoopPoint();
			else waypointID=1;
			subWaypointID=0;
			subPath=path.GetWPSectionPath(waypointID);
			//only reset position if not using path-looping or it's the initial reset
			if(!path.loop || initial) indicatorT.position=path.GetSpawnPoint().position;
		}
		
	}


}