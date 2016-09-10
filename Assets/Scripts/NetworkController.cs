using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public GameObject playerPrefab;
	public GameObject ammoBoxPrefab;

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
			Vector3 initialLocation = getLocationField(e.data);
			Quaternion initialRotation = getRotationField(e.data);

			Debug.Log(initialLocation);
	
			GameObject newPlayer = Instantiate(playerPrefab, initialLocation, initialRotation) as GameObject;
			players.Add(id, newPlayer);
		});
		mySocket.On ("player_initialize", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			playerID = id;

			Vector3 location = getLocationField(e.data);
			Quaternion rotation = getRotationField(e.data);

			players[id].transform.position = location;
			players[id].transform.rotation = rotation;
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
		mySocket.On ("shot_fired", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			if (players.ContainsKey(id)) {
				((SpaceShipSkeleton)players[id]).Fire();
			}
		});
		mySocket.On ("player_death", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			((SpaceShipSkeleton)players[id]).onDeath();
		});
		mySocket.On ("player_respawn", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			if (!id.Equals(playerID)) {
				return;
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
