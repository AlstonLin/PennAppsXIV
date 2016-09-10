using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static int playerID = -1;
	private SocketIOComponent mySocket;
	public GameObject socketObj;
	public GameObject player;

	void Start () {
		socketObj = GameObject.Find("SocketIO");
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		initializeSocketEvents ();
	}
	
	void Update () {
	}

	void initializeSocketEvents () {
		mySocket.On ("new_player", (SocketIOEvent e) => {
			Debug.Log(e.ToString());
			GameObject newPlayer = Instantiate(player);
			players.Add(e.data.GetField("id").ToString(), newPlayer);

		});
		mySocket.On ("id_assignment", (SocketIOEvent e) => {
			Debug.Log(e.ToString());
		});
		mySocket.Emit ("new_player_joined");
	}
		
}
