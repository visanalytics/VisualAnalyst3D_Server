using UnityEngine;
using System.Collections;

public class FlagData {
	
	private string Annotation;
	private int ID;
	private Vector3 DataPosition;
	private Vector3 WorldPosition;
	private int FlagImageIndex;
	private string FlagColorString;

	public FlagData(int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation, string Col, int Tex){
		this.ID = ID;
		this.Annotation = Annotation;
		this.DataPosition = DataPosition;
		this.WorldPosition = WorldPosition;
		this.FlagColorString = Col;
		this.FlagImageIndex = Tex;
	}

	// Getters/setters
	public void SetAnnotation(string val){this.Annotation = val;}
	public string GetAnnotation(){return this.Annotation;}
	public void SetID(int val){this.ID = val;}
	public int GetID(){return ID;}
	public void SetDataPos(Vector3 pos){this.DataPosition = pos;}
	public Vector3 GetDataPos(){return this.DataPosition;}
	public void SetWorldPos(Vector3 pos){
		this.WorldPosition = pos;
	}
	public Vector3 GetWorldPos(){return this.WorldPosition;}
	public void SetFlagImageIndex(int index){this.FlagImageIndex = index;}
	public int GetFlagImageIndex(){return this.FlagImageIndex;}
	public void SetFlagColorString(string val){this.FlagColorString = val;}
	public string GetFlagColorString(){return this.FlagColorString;}
}
