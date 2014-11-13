﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AstarSearch {

	private List<NodeClass> openList;		//list of nodes to analyze
	private List<NodeClass> closeList;		//list of nodes already analyzed
	private List<NodeClass> path;
	private Vector3 startingPosition;		//starting position of the algorithm
	private Vector3 arrivingPosition;		//position the algorithm try to reach
	private NodeClass nodeTemp;				//used to insert new node in lists
	private int lowestFvalue;				//use to store the lowest f value of the open list elements
	private int lowestFindex;				//use to store the index of the element with the lowest f value in the open list
	private Vector3[] movement = new [] {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};

	//function called to initialize the algorithme
	public List<NodeClass> init (Vector3 currPosition, Vector3 arrPosition)
	{
		//initialization of the variables
		openList = new List<NodeClass> ();
		closeList = new List<NodeClass>();
		path = new List<NodeClass>();
		startingPosition = currPosition;
		arrivingPosition = arrPosition;
		
		//gathering of the starting point informations
		nodeTemp = new NodeClass (startingPosition);
		nodeTemp.setH (System.Convert.ToInt32 (Mathf.Abs (arrivingPosition.x - nodeTemp.getNodePosition().x) + Mathf.Abs (arrivingPosition.z - nodeTemp.getNodePosition().z)));
		nodeTemp.setG (0);
		nodeTemp.setF (nodeTemp.getH () + nodeTemp.getG ());
		nodeTemp.setParent (new Vector3 (666, 666, 666));		//setting parent to a default position
		Debug.Log("Starting point info : " + nodeTemp.getNodePosition() + "  " + nodeTemp.getH() + "  " + nodeTemp.getG() + "  " + nodeTemp.getF());
		
		//make sure the openList and the closeList are empty at the beginning of the algorithm
		openList.Clear ();
		closeList.Clear();
		
		//add it to the openList
		openList.Add(nodeTemp);
		
		//launch of the algorithme
		Astar ();

		return path;
	}

	//function running the algorithm
	private void Astar()
	{
		//do the algorithm until we reach the arriving position or the open list is empty
		do
		{
			lowestFvalue = 1000;
			lowestFindex = 1000;
			//look for the lowest f value in the open list
			for (int i = 0; i < openList.Count; i++)
			{
				//if the current f value is lower than lowestFvalue, then put it into the variable
				if(openList[i].getF() < lowestFvalue)
				{
					lowestFindex = i;
					lowestFvalue = openList[i].getF();
				}
			}
			//add the element with the lowest f value of the open list in the close list
			closeList.Add (openList [lowestFindex]);
			Debug.Log ("arriving Position : " + arrivingPosition);
			Debug.Log("last element of close list : " + closeList.Last().getNodePosition());
			//then remove it from the open list
			openList.RemoveAt (lowestFindex);
			//if the element added in the close list is the arriving point, then stop the algorithm
			if (closeList.Last ().getNodePosition () == arrivingPosition)
			{
				findPath();
				Debug.Log("arriving position reached");
			}
			//else continue
			else
			{
				bool[] movementAllowed;
				//check the collision around the current position
				movementAllowed = checkCollision();
				//compute the children depending on the movements allowed
				computeChildren(movementAllowed);
			}
		}while(closeList.Last().getNodePosition() != arrivingPosition || openList.Count <= 0);
	}

	//function checking the collisions around a position
	private bool[] checkCollision()
	{
		bool[] allowMovement =  new [] {true, true, true, true};
		RaycastHit hit;
		//watch every element of the table
		for (int i = 0; i < allowMovement.Length; i++)
		{
			//Ray used to test if there is any collision on the left
			Ray collisionRay = new Ray (closeList.Last ().getNodePosition (), movement [i]);
			
			//Check if there is collision between the ray and any element and check if the tag is "EnvElement", if yes then don't allow the movement
			if (Physics.Raycast (collisionRay, out hit, 1f) && (hit.collider.tag == "EnvElement" || (hit.collider.tag == "Player" && hit.collider.transform.position != startingPosition)))
				allowMovement [i] = false;
		}
		return allowMovement;
	}

	//function conputing children of a node depending on the movements allowed from this node
	private void computeChildren(bool[] allowMovement)
	{
		bool check = true;
		for(int i = 0; i < allowMovement.Length; i++)
		{
			//if the movement is allowed
			if(allowMovement[i] == true)
			{
				//create a new node with all the child informations
				NodeClass nodeTemp2 = new NodeClass(new Vector3 (closeList.Last().getNodePosition().x + movement[i].x, closeList.Last().getNodePosition().y + movement[i].y, closeList.Last().getNodePosition().z + movement[i].z));
				nodeTemp2.setH(System.Convert.ToInt32 (Mathf.Abs (arrivingPosition.x - nodeTemp2.getNodePosition().x) + Mathf.Abs (arrivingPosition.z - nodeTemp2.getNodePosition().z)));
				nodeTemp2.setParent(closeList.Last().getNodePosition());
				nodeTemp2.setParentG(closeList.Last().getG());
				nodeTemp2.setG(nodeTemp2.getParentG() + 1 );
				nodeTemp2.setF(nodeTemp2.getG() + nodeTemp2.getH());
				check = true;
				for(int j = 0; j < closeList.Count; j++)
				{

					if(closeList[j].getNodePosition() == nodeTemp2.getNodePosition())
					   check = false;
					Debug.Log("<<<<<<<<<<<<<<<closeList element : " + closeList[j].getNodePosition() + "  " + nodeTemp2.getNodePosition() + "  " + check + j);
				}
				for(int k = 0; k < openList.Count; k++)
				{
					if(openList[k].getNodePosition() == nodeTemp2.getNodePosition() && openList[k].getF() < nodeTemp2.getF())
						check = false;
					Debug.Log("<<<<<<<<<<<<<<<openList element : " + openList[k].getNodePosition() + openList[k].getF() +  "  " + nodeTemp2.getNodePosition() + nodeTemp2.getF() + "  " + check + k);
				}
				if(check == true)
				{
					Debug.Log("-----------child and weight : " +  nodeTemp2.getNodePosition() + "  " + nodeTemp2.getF());
					//and add it to the openlist
					openList.Add(nodeTemp2);
				}
			}
		}
	}

	private void findPath()
	{
//		int i = 0;
//		path.Add(closeList.Last());
//		do
//		{
//			for(int j = 0; j < closeList.Count; j++)
//			{
//				if(path[i].getParent() == closeList[j].getNodePosition())
//					path.Add(closeList[j]);
//			}
//			i++;
//		}while(path[i-1].getNodePosition() != startingPosition);
//		for (int k = 0; k < path.Count; k++)
//		{
//			Debug.Log("Path " + k + " : " + path[k].getNodePosition());
//		}
	}
	
}
