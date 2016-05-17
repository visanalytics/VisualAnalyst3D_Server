using UnityEngine;
using System.Collections;
using System;

public class GridData : MonoBehaviour {
	
	private string Name;
	private Vector3 DataPosition;
	private Vector3[] GridPositions;
	private Vector3[] GridDataPositions;
	private int ID;
	private int Orientation;
	private Vector3 WorldPosition;
	private int PositionIndex;
	
	float[][] dataBounds;
	float[][] worldBounds;
	
	public GridData(int ID, int Orientation, string Name, int PositionIndex){
		this.ID = ID;
		this.Orientation = Orientation;
		this.Name = Name;
		this.PositionIndex = PositionIndex;
	}
	
	// Getters/setters
	public void SetName(string val){this.Name = val;}
	public string GetName(){return this.Name;}
	public void SetID(int val){this.ID = val;}
	public int GetID(){return ID;}
	public Vector3 GetWorldPos(){return this.WorldPosition;}
	public void SetPositionIndex(int val){this.PositionIndex = val;}
	public int GetPositionIndex(){return PositionIndex;}
	public void SetOrientation(int val){this.Orientation = val;}
	public int GetOrientation(){return this.Orientation;}
}