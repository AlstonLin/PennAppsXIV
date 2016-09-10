using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public GameObject playerPrefab;

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
	}
		
}
