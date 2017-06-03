using UnitedSolution;using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//#pragma warning disable 0168 // variable declared but not used.
//#pragma warning disable 0219 // variable assigned but not used.
//#pragma warning disable 0414 // private field assigned but not used.

using UnitedSolution;

namespace UnitedSolution {

	public class NodeGenerator : MonoBehaviour {
		private bool connectDiagonalNeighbour=false;
		
		private static NodeGenerator instance;
		
		private static Transform thisT;
		
		public void Awake(){
			if(instance!=null) return;
			
			instance=this;
			thisT=transform;
		}
		
		
		public static void Init(){
			if(instance!=null) return;
			
			GameObject obj=new GameObject();
			instance=obj.AddComponent<NodeGenerator>();
			
			obj.name="NodeGenerator";
		}
		
		
		
		public static NodeTD[] GenerateNode(PlatformTD platform, float heightOffset){
			if(instance==null) Init();
			
			Transform platformT=platform.thisT;
			
			float gridSize=BuildManager.GetGridSize();
			
			float scaleX=platform.thisT.localScale.x;
			float scaleZ=platform.thisT.localScale.z;
			
			int countX=(int)(scaleX/gridSize);
			int countZ=(int)(scaleZ/gridSize);
			
			
			float x=-scaleX/2/scaleX;
			float z=-scaleZ/2/scaleZ;
			
			
			Vector3 point=platformT.TransformPoint(new Vector3(x, 0, z));
			
			thisT.position=point;
			thisT.rotation=platformT.rotation;
			
			thisT.position=thisT.TransformPoint(new Vector3(gridSize/2, heightOffset, gridSize/2));
			
			NodeTD[] nodeGraph=new NodeTD[countZ*countX];
			
			int counter=0;
			for(int i=0; i<countZ; i++){
				for(int j=0; j<countX; j++){
					Vector3 pos=thisT.position;
					pos.y=pos.y+5000;
					
					LayerMask mask=1<<LayerManager.LayerTower();
					RaycastHit hit1;
					if(Physics.Raycast(pos, Vector3.down, out hit1, Mathf.Infinity, ~mask)) {
						nodeGraph[counter]=new NodeTD(new Vector3(pos.x, hit1.point.y+heightOffset, pos.z), counter);
					}
					else{
						nodeGraph[counter]=new NodeTD(pos, counter);
						nodeGraph[counter].walkable=false;
					}
					
					counter+=1;
					
					thisT.position=thisT.TransformPoint(new Vector3(gridSize, 0, 0));
				}
				thisT.position=thisT.TransformPoint(new Vector3(-(countX)*gridSize, 0, gridSize));
			}
			
			thisT.position=Vector3.zero;
			thisT.rotation=Quaternion.identity;
			
			
			counter=0;
			foreach(NodeTD cNode in nodeGraph){
				if(cNode.walkable){
					//check if there's anything within the point
					LayerMask mask=1<<LayerManager.LayerPlatform();
					mask|=1<<LayerManager.LayerTower();
					if(LayerManager.LayerTerrain()>=0) mask|=1<<LayerManager.LayerTerrain();
					Collider[] cols=Physics.OverlapSphere(cNode.pos, gridSize*0.45f, ~mask);
					if(cols.Length>0){
						cNode.walkable=false;
						counter+=1;
					}
				}
			}
			
			
			float neighbourDistance=0;
			float neighbourRange;
			if(instance.connectDiagonalNeighbour) neighbourRange=gridSize*1.5f;
			else neighbourRange=gridSize*1.1f;
			
			counter=0;
			//assign the neighouring  node for each node in the grid
			foreach(NodeTD currentNode in nodeGraph){
				//only if that node is walkable
				if(currentNode.walkable){
				
					//create an empty array
					List<NodeTD> neighbourNodeList=new List<NodeTD>();
					List<float> neighbourCostList=new List<float>();
					
					NodeTD[] neighbour=new NodeTD[8];
					int id=currentNode.ID;
					
					if(id>countX-1 && id<countX*countZ-countX){
						//print("middle rows");
						if(id!=countX) neighbour[0]=nodeGraph[id-countX-1];
						neighbour[1]=nodeGraph[id-countX];
						neighbour[2]=nodeGraph[id-countX+1];
						neighbour[3]=nodeGraph[id-1];
						neighbour[4]=nodeGraph[id+1];
						neighbour[5]=nodeGraph[id+countX-1];
						neighbour[6]=nodeGraph[id+countX];
						if(id!=countX*countZ-countX-1)neighbour[7]=nodeGraph[id+countX+1];
					}
					else if(id<=countX-1){
						//print("first row");
						if(id!=0) neighbour[0]=nodeGraph[id-1];
						if(nodeGraph.Length>id+1) neighbour[1]=nodeGraph[id+1];
						if(countZ>0){
							if(nodeGraph.Length>id+countX-1)	neighbour[2]=nodeGraph[id+countX-1];
							if(nodeGraph.Length>id+countX)	neighbour[3]=nodeGraph[id+countX];
							if(nodeGraph.Length>id+countX+1)	neighbour[4]=nodeGraph[id+countX+1];
						}
					}
					else if(id>=countX*countZ-countX){
						//print("last row");
						neighbour[0]=nodeGraph[id-1];
						if(id!=countX*countZ-1) neighbour[1]=nodeGraph[id+1];
						if(id!=countX*(countZ-1))neighbour[2]=nodeGraph[id-countX-1];
						neighbour[3]=nodeGraph[id-countX];
						neighbour[4]=nodeGraph[id-countX+1];
					}
					


					//scan through all the node in the grid
					foreach(NodeTD node in neighbour){
						//if this the node is not currentNode
						if(node!=null && node.walkable){
							//if this node is within neighbour node range
							neighbourDistance=GetHorizontalDistance(currentNode.pos, node.pos);
							if(neighbourDistance<neighbourRange){
								//if nothing's in the way between these two
								LayerMask mask=1<<LayerManager.LayerPlatform();
								mask|=1<<LayerManager.LayerTower();
								if(!Physics.Linecast(currentNode.pos, node.pos, ~mask)){
									//if the slop is not too steep
									//if(Mathf.Abs(GetSlope(currentNode.pos, node.pos))<=maxSlope){
										//add to list
										//if(!node.walkable) Debug.Log("error");
										neighbourNodeList.Add(node);
										neighbourCostList.Add(neighbourDistance);
									//}//else print("too steep");
								}//else print("something's in the way");
							}//else print("out of range "+neighbourDistance);
						}//else print("unwalkable");
					}

					//set the list as the node neighbours array
					currentNode.SetNeighbour(neighbourNodeList, neighbourCostList);
					
					//if(neighbourNodeList.Count==0)
						//Debug.Log("no heighbour. node number "+counter+"  "+neighbourNodeList.Count);
				}
				
				counter+=1;
			}
			
			return nodeGraph;
		}

		
		
		public static float GetHorizontalDistance(Vector3 p1, Vector3 p2){
			p1.y=0;
			p2.y=0;
			return Vector3.Distance(p1, p2);
		}

		
		public static bool ConnectDiagonalNeighbour(){ return instance.connectDiagonalNeighbour; }

	}



	public enum _ListStateTD{Unassigned, Open, Close};

	public class NodeTD{
		public int ID;
		public Vector3 pos;
		public NodeTD[] neighbourNode;
		public float[] neighbourCost;
		public NodeTD parent;
		public bool walkable=true;
		public float scoreG;
		public float scoreH;
		public float scoreF;
		public _ListStateTD listState=_ListStateTD.Unassigned;
		public float tempScoreG=0;
		
		public NodeTD(){}
		
		public NodeTD(Vector3 position, int id){
			pos=position;
			ID=id;
		}
		
		public void SetNeighbour(List<NodeTD> arrNeighbour, List<float> arrCost){
			neighbourNode = arrNeighbour.ToArray();
			neighbourCost = arrCost.ToArray();
		}
		
		public void ProcessNeighbour(NodeTD node){
			ProcessNeighbour(node.pos);
		}
		
		//call during a serach to scan through all neighbour, check their score against the position passed
		public void ProcessNeighbour(Vector3 pos){
			for(int i=0; i<neighbourNode.Length; i++){
				//if the neightbour state is clean (never evaluated so far in the search)
				if(neighbourNode[i].listState==_ListStateTD.Unassigned){
					//check the score of G and H and update F, also assign the parent to currentNode
					neighbourNode[i].scoreG=scoreG+neighbourCost[i];
					neighbourNode[i].scoreH=Vector3.Distance(neighbourNode[i].pos, pos);
					neighbourNode[i].UpdateScoreF();
					neighbourNode[i].parent=this;
				}
				//if the neighbour state is open (it has been evaluated and added to the open list)
				else if(neighbourNode[i].listState==_ListStateTD.Open){
					//calculate if the path if using this neighbour node through current node would be shorter compare to previous assigned parent node
					tempScoreG=scoreG+neighbourCost[i];
					if(neighbourNode[i].scoreG>tempScoreG){
						//if so, update the corresponding score and and reassigned parent
						neighbourNode[i].parent=this;
						neighbourNode[i].scoreG=tempScoreG;
						neighbourNode[i].UpdateScoreF();
					}
				}
			}
		}
		
		void UpdateScoreF(){
			scoreF=scoreG+scoreH;
		}
		
	}


	
}