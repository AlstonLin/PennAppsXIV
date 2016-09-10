using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public GameObject playerPrefab;
	public GameObject ammoBoxPrefab;

	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static int playerID = -1;
	private SocketIOComponent mySocket;

	void Start () {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		initializeSocketEvents ();
	}
	void initializeSocketEvents () {
		mySocket.On ("player_join", (SocketIOEvent e) => {
			Debug.Log(e.ToString());
			// TODO: Use the data from the socket event to set the initial location
			Vector3 initialLocation = new Vector3(0, 0, 0);
			// TODO: Create a transform from the location and rotation, add it as an argument to the Instantiate method
			GameObject newPlayer = Instantiate(playerPrefab) as GameObject;
			players.Add(e.data.GetField("id").ToString(), newPlayer);
		});
		mySocket.On ("id_assignment", (SocketIOEvent e) => {
			Debug.Log(e.ToString());
		});
		mySocket.On ("location_update", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			if (players.ContainsKey(id)){
				// TODO: Parse the data from the event (e) and set the position and rotation of this player to whatever was parsed
				// players[id].transform.position =
			}
		});
		mySocket.On ("ammo_spawn", (SocketIOEvent e) => {
			Debug.Log("RECEIVED COMMAND! " + e.ToString());
			int amount, x, y, z;
			int.TryParse (e.data.GetField("amount").ToString(), out amount);
			int.TryParse (e.data.GetField("location_x").ToString(), out x);
			int.TryParse (e.data.GetField("location_y").ToString(), out y);
			int.TryParse (e.data.GetField("location_z").ToString(), out z);
			Debug.Log("RECEIVED COMMAND2!");
			spawnAmmo(amount, x, y, z);
		});
	}

	void spawnAmmo (int amount, float x, float y, float z){
		Debug.Log ("AMMO SPAWN: (" + x + ", " + y + ", " + z + ")");
		Vector3 newPos = new Vector3(x, y, z);
		GameObject ammoBox = Instantiate(ammoBoxPrefab, newPos, Quaternion.identity) as GameObject;
	}
}
