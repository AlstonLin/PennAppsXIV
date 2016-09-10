using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static int playerID = -1;
	private SocketIOComponent sock;
	public GameObject socketObj;

	void Start () {
		sock = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		sock.On ("player_join", ev => {
		});
	}
	
	void Update () {
	
	}
}
