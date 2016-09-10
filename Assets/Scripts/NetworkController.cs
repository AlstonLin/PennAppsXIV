using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static int playerID = -1;
	private SocketIOComponent mySocket;
	public GameObject socketObj;

	void Start () {
		initializeSocketEvents ();
		mySocket.Emit ("new_player");
	}
	
	void Update () {
	
	}

	void initializeSocketEvents() {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		mySocket.On ("new_player", (SocketIOEvent e) => {
			Debug.Log(e.ToString());
			players.Add(e.data.GetField("id").ToString(), new GameObject ());
		});
	}
		
}
