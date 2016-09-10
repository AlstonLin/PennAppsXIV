using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public GameObject playerPrefab;

	private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
	public static string playerID = "";
	private SocketIOComponent mySocket;

	void Start () {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		initializeSocketEvents ();
	}

	void initializeSocketEvents () {
		mySocket.On ("player_join", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			playerID = id;

			Vector3 initialLocation = getLocationField(e.data);
			Quaternion initialRotation = getRotationField(e.data);

			Debug.Log(initialLocation);
	
			GameObject newPlayer = Instantiate(playerPrefab, initialLocation, initialRotation) as GameObject;
			players.Add(e.data.GetField("id").ToString(), newPlayer);
		});
		mySocket.On ("player_initialize", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			Debug.Log(id);

			playerID = id;
		});
		mySocket.On ("location_update", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			if (players.ContainsKey(id)){
				Vector3 location = getLocationField(e.data);
				Quaternion rotation = getRotationField(e.data);

				players[id].transform.position = location;
				players[id].transform.rotation = rotation;
			}
		});
		mySocket.On ("player_health_update", (SocketIOEvent e) => {
		
		});
		mySocket.On ("player_respawn", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			if (!id.Equals(playerID)) {
				return;
			}

		});
	}

	private float getFloatField(string key, JSONObject data) {
		return float.Parse (data.GetField (key).ToString ());
	}

	private Vector3 getLocationField(JSONObject data) {
		float locationX = getFloatField("location_x", data);
		float locationY = getFloatField("location_y", data);
		float locationZ = getFloatField("location_z", data);
		return new Vector3(locationX, locationY, locationZ);
	}

	private Quaternion getRotationField(JSONObject data) {
		float rotationX = getFloatField("rotation_x", data);
		float rotationY = getFloatField("rotation_y", data);
		float rotationZ = getFloatField("rotation_z", data);
		return new Quaternion(rotationX, rotationY, rotationZ, 0);
	}
}
