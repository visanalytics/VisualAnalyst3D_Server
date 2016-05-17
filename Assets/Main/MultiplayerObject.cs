using UnityEngine;
using System.Collections;
public class MultiplayerObject : MonoBehaviour
{
	public NetworkPlayer ID;
	Light PlayerLight;
	Color PlayerColor;

	// Use this for initialization
	void Start () {
		PlayerLight = this.light;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetPosition(Vector3 pos){
		this.transform.position = pos;
	}
	public Vector3 GetPosition(){return this.transform.position;}

	public void SetColor(Color col){
		PlayerColor = col;
		this.renderer.material.color = PlayerColor;//Color.Lerp(PlayerColor,Color.white,0.5f);
		this.light.color = Color.Lerp(PlayerColor,Color.white,0.15f);
		if(networkView.isMine){
			networkView.RPC("SetColor", RPCMode.All, col.r, col.g, col.b);
		}
	}

	public void SetColor(float r, float g, float b){
		PlayerColor = new Color(r,g,b,1.0f);
		this.renderer.material.color = PlayerColor;//Color.Lerp(PlayerColor,Color.white,0.5f);
		this.light.color = Color.Lerp(PlayerColor,Color.white,0.15f);
		if(networkView.isMine){
			networkView.RPC("SetColorRPC", RPCMode.All, r, g, b);
		}
	}
	public Color GetColor(){return this.PlayerColor;}
	public void BroadcastColor(){
		networkView.RPC("SetColorRPC", RPCMode.All, PlayerColor.r, PlayerColor.g, PlayerColor.b);
	}

	[RPC]
	void SetColorRPC(float r, float g, float b){
		PlayerColor = new Color(r,g,b,1.0f);
		this.renderer.material.color = PlayerColor;//Color.Lerp(PlayerColor,Color.white,0.5f);
		this.light.color = Color.Lerp(PlayerColor,Color.white,0.15f);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Destroy(gameObject);
	}
}