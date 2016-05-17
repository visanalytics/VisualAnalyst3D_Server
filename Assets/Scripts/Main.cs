using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class Main : MonoBehaviour {
	
	int Port = 25061;
	private const string TypeName = "DeakinDataVis";
	private string RoomName = "SAS-VAC";
	private bool NamingServer = false;
	private bool SettingMasterServer = false;
	string MasterServerIPString = "";
	string MasterServerPortString = "";
	bool ShowFailedMasterServerConnect = false;
	string ServerIP;

	public GameObject PlayerObject;

	List<Color> Colors = new List<Color>(){Color.blue,
		Color.green,
		Color.magenta,
		Color.yellow,
		new Color(102f/255f,1.0f,178f/255f),
		new Color(102f/255f,0f,204f/255f),
		new Color(1.0f,204f/255f,204f/255f)
	};
	int colorIt = 0;

	string CurrentDataSource = "Victoria",
	CurrentTerrainType = "Smooth",
	CurrentFilename = "Age",
	CurrentPreset = "Peaks";
	bool CurrentRegen = false;
	float CurrentScale = 0.5f;
	float CurrentColorScale = 1f;

	#region Track Networkable Objects (e.g. flags, grids, axes)

	List<FlagData> Flags = new List<FlagData>();
	List<GridData> Grids = new List<GridData>();
	float AxisGridSizeX = 1f, AxisGridSizeY = 1f, AxisGridSizeZ = 1f;
	bool AxisOn = false;
	bool AxisLabelsOn = false;
	int NumDataPoints = 1;
	bool DataPointsOn = false;

	#endregion

	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		//Open user GUI to name the Server's Room name;
		SettingMasterServer = false;
		NamingServer = true;
	}
	
	// Update is called once per frame
	void Update () {

	}

	#region GUI values

	float sw = Screen.width;
	float sh = Screen.height;
	float tw = Screen.width/6f;//100f;//Screen.width/4f;
	float th = Screen.height/15f;//20f;//Screen.height/8f;
	string ServerNameFieldText = "";

	#endregion

	void OnGUI(){
		/*if(SettingMasterServer){
			GUI.Label(new Rect(sw*0.5f - tw*0.75f, sh*0.5f - th*1.5f, tw*2, th), "Enter Master Server IP:");
			MasterServerIPString = GUI.TextField(new Rect(sw*0.5f - tw*0.75f, sh*0.5f - th*0.5f, tw*1.5f, th), MasterServerIPString, 32);

			if(GUI.Button(new Rect(new Rect(sw*0.5f - tw*0.5f, sh*0.5f + th*0.5f, tw*1f, th)), "Continue")){
				MasterServer.ipAddress = MasterServerIPString;
				MasterServer.port = 23466;//int.Parse(MasterServerPortString);
				Network.natFacilitatorIP = MasterServerIPString;
				Network.natFacilitatorPort = 50005;
				NamingServer = true;
				SettingMasterServer = false;
			}
		}else*/
		if(NamingServer){
			GUI.Label(new Rect(sw*0.5f - tw*0.75f, sh*0.5f - th*1.5f, tw*2, th), "Enter Server Name:");
			ServerNameFieldText = GUI.TextField(new Rect(sw*0.5f - tw*0.75f, sh*0.5f - th*0.5f, tw*1.5f, th), ServerNameFieldText, 32);
			if(GUI.Button(new Rect(sw*0.5f - tw*0.5f, sh*0.5f + th*0.75f, tw, th), "Start Server")){
				InitServer(ServerNameFieldText);
				NamingServer = false;
			}
		}else{
			// Options to stop the server broadcasting
			if(GUI.Button(new Rect(tw, th, tw, th), "Stop Server")){
				Network.Disconnect(250);
				NamingServer = true;
			}

			GUI.Label(new Rect(tw*2.5f, th, tw*4f, th), "Server IP: " + ServerIP);
			/// TODO: options to change terrain
			/// list of players connected by computer name & IP address
			NetworkPlayer[] connections = Network.connections; 
			string PlayerStatusString = connections.Length > 0 ? connections.Length.ToString() + " players connected." : "No players connected";
			GUI.Label(new Rect(tw*0.75f, th*2.5f, tw*3f, th), PlayerStatusString);
			for(int i=0; i<connections.Length; i++){
				string CurrentPlayerString =
					(i+1).ToString() + ". IP: " + connections[i].ipAddress + ":" +connections[i].port;
				GUI.Label(new Rect(tw*0.75f, th*((float)i+4f), tw*2f, th), CurrentPlayerString);
				if(GUI.Button(new Rect(tw*3f, th*((float)i+4f), tw, th), "Disconnect")){
					Network.CloseConnection(connections[i], true);
				}
			}
		}

		if(ShowFailedMasterServerConnect){
			GUI.Label(new Rect(sw*0.5f - tw*0.75f, sh*0.5f + th*2.5f, tw*2, th), "Failed to connect to master server.");
			if(GUI.Button(new Rect(sw*0.5f + tw*1.5f, sh*0.5f + th*2.5f, tw*0.5f, th), "OK")){
				ShowFailedMasterServerConnect = false;
			}
			//GUI.Label(new Rect(sw*0.5f - tw*0.75f, sh*0.5f + th*3.5f, tw*2, th), "Try restarting program.");
		}
	}

	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		ShowFailedMasterServerConnect = true;
	}
	
	private void InitServer(string RoomName){
		NetworkConnectionError e = Network.InitializeServer (20, Port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(TypeName,RoomName);
		Debug.Log("Multiplayer Server return: " + e);
		ServerIP = GetIP ();
	}
	
	void OnApplicationQuit(){
		Network.Disconnect(250);
	}
	
	public string GetIP()
	{
		IPHostEntry host;
		string localIP = "?";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
			}
		}
		return localIP;
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log("Player connected from " + player.ipAddress + 
		          ":" + player.port);
		CreatePlayer (player);
		UpdatePlayersObjects(player);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		bool cleaned = false; 
		Network.DestroyPlayerObjects(player);
		Network.RemoveRPCs(player);
		networkView.RPC("RemovePlayer", RPCMode.All, player);
		Debug.Log ("Player " + player.ipAddress + " was disconnected");
	}  
	
	private void CreatePlayer(NetworkPlayer player)
	{
		networkView.RPC("SetPlayer",RPCMode.All, player, Colors[colorIt].r, Colors[colorIt].g, Colors[colorIt].b);
		colorIt = (colorIt+1)%Colors.Count;
		Debug.Log("Player " + player.ipAddress + " was instantiated");
	}

	private void UpdatePlayersObjects(NetworkPlayer player){
		// TERRAIN
		networkView.RPC("ChangeTerrain", player, Network.player, CurrentDataSource, CurrentTerrainType, CurrentFilename, CurrentPreset, CurrentRegen, CurrentScale, CurrentColorScale);
		// FLAGS
		Debug.Log("Sending Flag Data of " + Flags.Count + " flags.");
		for(int i=0; i<Flags.Count; i++){
			FlagData d = Flags[i];
			Debug.Log("Flag " + i + " sent.");
			networkView.RPC("CreateFlag", player, player, d.GetID(), d.GetWorldPos(), d.GetDataPos(), d.GetAnnotation(), d.GetFlagColorString(), d.GetFlagImageIndex());
		}
		// GRIDS
		for(int i=0; i<Grids.Count; i++){
			GridData d = Grids[i];
			Debug.Log("Grid " + i + " sent.");
			networkView.RPC("CreateGrid", player, player, d.GetID(), d.GetOrientation(), d.GetName(), d.GetPositionIndex());
		}
		// AXES
		if(AxisOn){
			networkView.RPC("ToggleAxisOn", player, Network.player, AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
		}
		if(AxisLabelsOn){
			networkView.RPC("ToggleAxisLabelsOn", player, Network.player, AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
		}
		// DATA POINTS
		if(DataPointsOn){
			networkView.RPC("AddDataPoints", player, Network.player, NumDataPoints);
		}
	}
	
	#region RPC Calls:

	[RPC]
	void ChangeTerrain(NetworkPlayer player, string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		Debug.Log("Terrain changed on Server");
		if(player != Network.player){
			if(CurrentDataSource != DataSource || CurrentFilename != Filename){
				Debug.Log("Uh oh, Flags and grids are gone!");
				for(int i=0; i<Flags.Count; i++){
					networkView.RPC("DeleteFlag", RPCMode.Others, Flags[i].GetID());
				}
				for(int i=0; i<Grids.Count; i++){
					networkView.RPC("DeleteGrid", RPCMode.Others, Grids[i].GetID());
				}
				Flags = new List<FlagData>();
				Grids = new List<GridData>();
			}
			CurrentDataSource = DataSource;
			CurrentTerrainType = TerrainType;
			CurrentFilename = Filename;
			CurrentPreset = Preset;
			CurrentScale = scale;
			CurrentColorScale = colorScale;
			networkView.RPC("ChangeTerrain", RPCMode.Others, Network.player, DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
		}
	}

	[RPC]
	void CreateFlag(NetworkPlayer player, int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation, string Col, int Tex){
		FlagData tempData = new FlagData(ID, WorldPosition, DataPosition, Annotation, Col, Tex);
		Flags.Add(tempData);
		Debug.Log("New Flag created");
	}
	[RPC]
	void DeleteFlag(int ID){
		for(int i=0; i<Flags.Count; i++){
			if(Flags[i].GetID() == ID){
				Flags.RemoveAt(i);
				break;
			}
		}
	}
	
	[RPC]
	void CreateGrid(NetworkPlayer player, int ID, int Orientation, string Name, int PositionIndex){
		GridData tempData = new GridData(ID, Orientation, Name, PositionIndex);
		Grids.Add(tempData);
	}
	[RPC]
	void DeleteGrid(int ID){
		for(int i=0; i<Grids.Count; i++){
			if(Grids[i].GetID() == ID){
				Grids.RemoveAt(i);
				break;
			}
		}
	}
	
	[RPC]
	void UpdateGrid(NetworkPlayer player, int ID, int Index){
		for(int i=0; i<Grids.Count; i++){
			if(Grids[i].GetID() == ID){
				Grids[i].SetPositionIndex(Index);
				break;
			}
		}
	}
	
	[RPC]
	void ToggleAxisOn(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		AxisOn = true;
		AxisGridSizeX = AxisValueX;
		AxisGridSizeY = AxisValueY;
		AxisGridSizeZ = AxisValueZ;
	}
	[RPC]
	void ToggleAxisOff(){
		AxisOn = false;
	}
	[RPC]
	void ToggleAxisLabelsOn(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		AxisLabelsOn = true;
		AxisGridSizeX = AxisValueX;
		AxisGridSizeY = AxisValueY;
		AxisGridSizeZ = AxisValueZ;
	}
	[RPC]
	void ToggleAxisLabelsOff(){
		AxisLabelsOn = false;
	}

	[RPC]
	void ResetAxis(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		AxisGridSizeX = AxisValueX;
		AxisGridSizeY = AxisValueY;
		AxisGridSizeZ = AxisValueZ;
	}
	
	[RPC]
	void AddDataPoints(NetworkPlayer player, int NumDataPoints){
		DataPointsOn = true;
		this.NumDataPoints = NumDataPoints;
	}
	
	[RPC]
	void RemoveDataPoints(){
		DataPointsOn = false;
	}
	
	[RPC]
	void ResetDataPoints(NetworkPlayer player, int NumDataPoints){
		this.NumDataPoints = NumDataPoints;
	}
	
	[RPC]
	void Test(String q){
		Debug.Log ("Received: " + q);
	}
	
	#endregion
	
	#region Client RPC Calls (DISREGARD)
	
	[RPC]
	void SetPlayer(NetworkPlayer player, float playerColorR, float playerColorG, float playerColorB){}

	[RPC]
	void SpawnPlayer(NetworkPlayer player, float r, float g, float b, Vector3 position){}

	[RPC]
	void UpdatePlayer(NetworkPlayer player, Vector3 position){}

	[RPC]
	void RemovePlayer(NetworkPlayer player){}
	
	#endregion
}
