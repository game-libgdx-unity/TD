using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//#pragma warning disable 0168 // variable declared but not used.
//#pragma warning disable 0219 // variable assigned but not used.
//#pragma warning disable 0414 // private field assigned but not used.

using UnitedSolution;

namespace UnitedSolution {

	public delegate void SetPathCallbackTD(List<Vector3> wp);
	
	
	class SearchQueue{
		public NodeTD startNode;
		public NodeTD endNode;
		public NodeTD[] graph;
		public SetPathCallbackTD callBackFunc;
		
		public SearchQueue(NodeTD n1, NodeTD n2, NodeTD[] g, SetPathCallbackTD func){
			startNode=n1;
			endNode=n2;
			graph=g;
			callBackFunc=func;
		}
	}
	
	

	public class PathFinder : MonoBehaviour {

		private List<SearchQueue> searchQueueList=new List<SearchQueue>();
		
		private bool searching=false;
		
		private static PathFinder instance;
		
		public enum _PathSmoothing{None, LOS, Mean}
		private _PathSmoothing pathSmoothing=_PathSmoothing.Mean;
		
		private int ScanNodeLimitPerFrame=1000;
		
		
		
		public void Awake(){
			if(instance!=null) return;
			instance=this;
		}
		
		
		public static void Init(){
			if(instance!=null) return;
			
			GameObject obj=new GameObject();
			instance=obj.AddComponent<PathFinder>();
			
			obj.name="PathFinder";
		}
		
		
		
		
		
		void Update(){
			if(searchQueueList.Count>0 && !searching){
				SearchQueue qItem=searchQueueList[0];
				StartCoroutine(_SearchRoutine(qItem.startNode, qItem.endNode, qItem.graph, qItem.callBackFunc));
				searchQueueList.RemoveAt(0);
			}
		}
		
		
		public static NodeTD GetNearestNode(Vector3 point, NodeTD[] graph){
			return GetNearestNode(point, graph, 1);
		}
		public static NodeTD GetNearestNode(Vector3 point, NodeTD[] graph, int searchMode){
			float dist=Mathf.Infinity;
			float currentNearest=Mathf.Infinity;
			NodeTD nearestNode=null;
			foreach(NodeTD node in graph){
				if(searchMode==0){
					dist=Vector3.Distance(point, node.pos);
					if(dist<currentNearest){
						currentNearest=dist;
						nearestNode=node;
					}
				}
				else if(searchMode==1){
					if(node.walkable){
						dist=Vector3.Distance(point, node.pos);
						if(dist<currentNearest){
							currentNearest=dist;
							nearestNode=node;
						}
					}
				}
				else if(searchMode==2){
					if(!node.walkable){
						dist=Vector3.Distance(point, node.pos);
						if(dist<currentNearest){
							currentNearest=dist;
							nearestNode=node;
						}
					}
				}
			}
			return nearestNode;
		}
		
		
		public static void GetPath(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc){
			if(instance==null) Init();
			instance._GetPath(startN, endN, graph, callBackFunc);
		}
		public void _GetPath(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc){
			if(!searching){
				//commence search
				StartCoroutine(_SearchRoutine(startN, endN, graph, callBackFunc));
			}
			else{
				//if a serach is in progress, put current request into the queue list
				SearchQueue q=new SearchQueue(startN, endN, graph, callBackFunc);
				searchQueueList.Add(q);
			}
		}
		
		
		
		IEnumerator _SearchRoutine(NodeTD startN, NodeTD endN, NodeTD[] graph, SetPathCallbackTD callBackFunc){
			
			//mark that a serac has started, any further query will be queued
			searching=true;
			bool pathFound=true;
			
			int searchCounter=0;	//used to count the total amount of node that has been searched
			int loopCounter=0;		//used to count how many node has been search in the loop, if it exceed a value, bring it to the next frame
			//float LoopTime=Time.realtimeSinceStartup;

			//closelist, used to store all the node that are on the path
			List<NodeTD> closeList=new List<NodeTD>();
			//openlist, all the possible node that yet to be on the path, the number can only be as much as the number of node in the garph
			NodeTD[] openList=new NodeTD[graph.Length];
			
			//an array use to record the element number in the open list which is empty after the node is removed to be use as currentNode,
			//so we can use builtin array with fixed length for openlist, also we can loop for the minimal amount of node in every search
			List<int> openListRemoved=new List<int>();
			//current count of elements that are occupied in openlist, openlist[n>openListCounter] are null
			int openListCounter=0;

			//set start as currentNode
			NodeTD currentNode=startN;
			
			//use to compare node in the openlist has the lowest score, alwyas set to Infinity when not in used
			float currentLowestF=Mathf.Infinity;
			int id=0;	//use element num of the node with lowest score in the openlist during the comparison process
			int i=0;		//universal int value used for various looping operation

			//loop start
			while(true){
			
				//if we have reach the destination
				if(currentNode==endN) break;
				
				//for gizmo debug purpose
				//currentNodeBeingProcess=currentNode;

				//move currentNode to closeList;
				closeList.Add(currentNode);
				currentNode.listState=_ListStateTD.Close;
				
				//loop through the neighbour of current loop, calculate  score and stuff
				currentNode.ProcessNeighbour(endN);
				
				//put all neighbour in openlist
				foreach(NodeTD neighbour in currentNode.neighbourNode){
					if(neighbour.listState==_ListStateTD.Unassigned && neighbour.walkable) {
						//set the node state to open
						neighbour.listState=_ListStateTD.Open;
						//if there's an open space in openlist, fill the space
						if(openListRemoved.Count>0){
							openList[openListRemoved[0]]=neighbour;
							//remove the number from openListRemoved since this element has now been occupied
							openListRemoved.RemoveAt(0);
						}
						//else just stack on it and increase the occupication counter
						else{
							openList[openListCounter]=neighbour;
							openListCounter+=1;
						}
					}
				}
				
				//clear the current node, before getting a new one, so we know if there isnt any suitable next node
				currentNode=null;
				
				//get the next point from openlist, set it as current point
				//just loop through the openlist until we reach the maximum occupication
				//while that, get the node with the lowest score
				currentLowestF=Mathf.Infinity;
				id=0;
				for(i=0; i<openListCounter; i++){
					if(openList[i]!=null){
						if(openList[i].scoreF<currentLowestF){
							currentLowestF=openList[i].scoreF;
							currentNode=openList[i];
							id=i;
						}
					}
				}
				
				//if there's no node left in openlist, path doesnt exist
				if(currentNode==null) {
					pathFound=false;
					break;
				}
				
				//remove the new currentNode from openlist
				openList[id]=null;
				//put the id into openListRemoved so we know there's an empty element that can be filled in the next loop
				openListRemoved.Add(id);
				
				//increase the counter
				searchCounter+=1;
				loopCounter+=1;
				
				//if exceed the search limit per frame, bring the search to the next frame
				if(loopCounter>ScanNodeLimitPerFrame){
					loopCounter=0;	//reset the loopCounter for the next frame
					yield return null;
				}
			}

			
			//trace back the path through closeList
			List<Vector3> p=new List<Vector3>();
				
			if(pathFound){
				//track back the node's parent to form a path
				while(currentNode!=null){
					p.Add(currentNode.pos);
					currentNode=currentNode.parent;
				}
				
				//since the path is now tracked from endN ot startN, invert the list
				p=InvertArray(p);
				p=SmoothPath(p);
			}
			
			callBackFunc(p);

			//clear searching so indicate the search has end and a new serach can be called
			searching=false;
			
			ResetGraph(graph);
		
		}
		
		
		
		
		
		
		//make cause system to slow down, use with care
		public static List<Vector3> ForceSearch(NodeTD startN, NodeTD endN, NodeTD blockN, NodeTD[] graph, int footprint=-1){
			if(blockN!=null) blockN.walkable=false;
			
			bool pathFound=true;
			
			int searchCounter=0;	//used to count the total amount of node that has been searched
			
			List<NodeTD> closeList=new List<NodeTD>();
			NodeTD[] openList=new NodeTD[graph.Length];
			
			List<int> openListRemoved=new List<int>();
			int openListCounter=0;

			NodeTD currentNode=startN;
			
			float currentLowestF=Mathf.Infinity;
			int id=0;	//use element num of the node with lowest score in the openlist during the comparison process
			int i=0;		//universal int value used for various looping operation
			
			while(true){
			
				if(currentNode==endN) break;
				
				closeList.Add(currentNode);
				currentNode.listState=_ListStateTD.Close;
				
				currentNode.ProcessNeighbour(endN);
				
				foreach(NodeTD neighbour in currentNode.neighbourNode){
					if(neighbour.listState==_ListStateTD.Unassigned && neighbour.walkable) {
						neighbour.listState=_ListStateTD.Open;
						if(openListRemoved.Count>0){
							openList[openListRemoved[0]]=neighbour;
							openListRemoved.RemoveAt(0);
						}
						else{
							openList[openListCounter]=neighbour;
							openListCounter+=1;
						}
					}
				}
				
				currentNode=null;
				
				currentLowestF=Mathf.Infinity;
				id=0;
				for(i=0; i<openListCounter; i++){
					if(openList[i]!=null){
						if(openList[i].scoreF<currentLowestF){
							currentLowestF=openList[i].scoreF;
							currentNode=openList[i];
							id=i;
						}
					}
				}
				
				if(currentNode==null) {
					pathFound=false;
					break;
				}
				
				openList[id]=null;
				openListRemoved.Add(id);

				searchCounter+=1;
				
			}

			
			List<Vector3> p=new List<Vector3>();
			
			if(pathFound){
				while(currentNode!=null){
					p.Add(currentNode.pos);
					currentNode=currentNode.parent;
				}
				
				p=InvertArray(p);
				p=SmoothPath(p);
			}
			
			if(blockN!=null)blockN.walkable=true; 
			
			ResetGraph(graph);
			
			return p;

		}
		
		
		
		public static bool IsPathSmoothingOn(){
			if(instance.pathSmoothing!=_PathSmoothing.None) return true;
			return false;
		}
		public static List<Vector3> SmoothPath(List<Vector3> p){
			if(instance.pathSmoothing==_PathSmoothing.Mean){
				p=MeanSmoothPath(p);
			}
			else if(instance.pathSmoothing==_PathSmoothing.LOS){
				p=instance.LOSPathSmoothingBackward(p);
				p=instance.LOSPathSmoothingForward(p);
				
				//recreate the path by sampling the path with the spacing of gridSize
				//this is done so when checking if a new build point cross this path, it can be detected
				float gridSize=BuildManager.GetGridSize();
				List<Vector3> newP=new List<Vector3>{ p[0] };// 	newP.Add( p[0] );
				for(int i=1; i<p.Count; i++){
					while(Vector3.Distance(newP[newP.Count-1], p[i])>gridSize){
						newP.Add(newP[newP.Count-1]+(p[i]-p[i-1]).normalized*gridSize);
					}
					newP.Add(p[i]);
				}
				
				p=newP;
			}
			
			return p;
		}
		

		
		private static List<Vector3> InvertArray(List<Vector3> p){
			List<Vector3> pInverted=new List<Vector3>();
			for(int i=0; i<p.Count; i++){
				pInverted.Add(p[p.Count-(i+1)]);
			}
			return pInverted;
		}
		
		public static List<Vector3> MeanSmoothPath(List<Vector3> p){
			if(p.Count<=2) return p;
			
			for(int i=0; i<p.Count; i++){
				if(i==0){
					//if(p.Count>=2) p[i]=(p[i]+p[i+1])/2;
					// else break;
				}
				else if(i==p.Count-1){
					//if(p.Count>=3) p[i]=(p[i-1]+p[i])/2;
					//else break;
				}
				else p[i]=(p[i-1]+p[i]+p[i+1])/3;
			}
			
			return p;
		}
		
		
		
		public static List<Vector3> MeanSmoothPath5(List<Vector3> p){
			//~ p=pathFinder.LOSPathSmoothingBackward(p);
			//~ p=pathFinder.LOSPathSmoothingForward(p);
			
			for(int i=1; i<p.Count-1; i++){
				if(i==0){
					if(p.Count>=3) p[i]=(p[i]+p[i+1]+p[i+2])/3;
					else break;
				}
				else if(i==1){
					if(p.Count==3) p[i]=(p[i-1]+p[i]+p[i+1])/3;
					if(p.Count>=4) p[i]=(p[i-1]+p[i]+p[i+1]+p[i+2])/4;
					else break;
				}
				else if(i==p.Count-2){
					if(p.Count==3) p[i]=(p[i-1]+p[i]+p[i+1])/3;
					if(p.Count>=4) p[i]=(p[i-2]+p[i-1]+p[i]+p[i+1])/4;
					else break;
				}
				else if(i==p.Count-1){
					if(p.Count>=3) p[i]=(p[i-2]+p[i-1]+p[i])/3;
					else break;
				}
				else p[i]=(p[i-2]+p[i-1]+p[i]+p[i+1]+p[i+2])/5;
			}
			
			return p;
		}
		
		//pathSmoothing forward
		private List<Vector3> LOSPathSmoothingForward(List<Vector3> p){
			float gridSize=BuildManager.GetGridSize();
			int num=0;
			float allowance=gridSize*0.4f;
			while (num+2<p.Count){
				bool increase=false;
				Vector3 p1=p[num];
				Vector3 p2=p[num+2];
				RaycastHit hit;
				Vector3 dir=p2-p1;
				
				LayerMask mask=1<<LayerManager.LayerTerrain();
				if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1), ~mask)){
					if(p1.y==p2.y) p.RemoveAt(num+1);
					else increase=true;
				}
				else {
					increase=true;
				}
				
				if(increase) num+=1;

			}
			return p;
		}



		//pathSmoothing backward
		private List<Vector3> LOSPathSmoothingBackward(List<Vector3> p){
			float gridSize=BuildManager.GetGridSize();
			int num=p.Count-1;
			float allowance=gridSize*0.4f;
			while (num>1){
				bool decrease=false;
				Vector3 p1=p[num];
				Vector3 p2=p[num-2];
				RaycastHit hit;
				Vector3 dir=p2-p1;
				
				if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1))){
					if(p1.y==p2.y) p.RemoveAt(num-1);
					else decrease=true;
				}
				else {
					decrease=true;
				}
				
				num-=1;
				if(decrease) num-=1;

			}
			return p;
		}
		
		public static void ResetGraph(NodeTD[] nodeGraph){
			foreach(NodeTD node in nodeGraph){
				node.listState=_ListStateTD.Unassigned;
				node.parent=null;
			}
		}
		
		
		
		
		
		
		//not in used
		/*
		public static List<NodeTD> GetNodeInFootprint(NodeTD origin, int footprint){
			if(footprint<=0) return new List<NodeTD>();
			
			bool connectDNeighbour=NodeGenerator.ConnectDiagonalNeighbour();
			
			List<NodeTD> currentList=new List<NodeTD>();
			List<NodeTD> openList=new List<NodeTD>();
			List<NodeTD> closeList=new List<NodeTD>();
			
			
			if(connectDNeighbour){
				closeList.Add(origin);
				foreach(NodeTD node in origin.neighbourNode){
					currentList.Add(node);
				}
				
				for(int i=0; i<footprint; i++){
					openList=currentList;
					currentList=new List<NodeTD>();
					foreach(NodeTD node in openList){
						foreach(NodeTD neighbour in node.neighbourNode){
							if(!openList.Contains(neighbour) && !closeList.Contains(neighbour)){
								currentList.Add(neighbour);
							}
						}
						closeList.Add(node);
					}
				}
			}
			else{
				closeList.Add(origin);
				foreach(NodeTD node in origin.neighbourNode){
					currentList.Add(node);
				}
				
				float range=1*(footprint)*BuildManager.GetGridSize()+BuildManager.GetGridSize()*0.25f;
				
				for(int i=0; i<footprint*2; i++){
					openList=currentList;
					currentList=new List<NodeTD>();
					foreach(NodeTD node in openList){
						foreach(NodeTD neighbour in node.neighbourNode){
							if(Mathf.Abs(origin.pos.x-neighbour.pos.x)<=range && Mathf.Abs(origin.pos.z-neighbour.pos.z)<=range){
								if(!openList.Contains(neighbour) && !closeList.Contains(neighbour)){
									currentList.Add(neighbour);
								}
							}
						}
						closeList.Add(node);
					}
				}
			}
			 
			return closeList;
		}
		*/
		
		
		
		
	}

	

}