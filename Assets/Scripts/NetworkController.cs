using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using System;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public SpaceShipSkeleton playerPrefab;
	public GameObject ammoBoxPrefab;

	private Dictionary<string, SpaceShipSkeleton> players = new Dictionary<string, SpaceShipSkeleton>();
	public static string playerID = "";
	private SocketIOComponent mySocket;

	void Start () {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		initializeSocketEvents ();
	}
	void initializeSocketEvents () {
		mySocket.On ("player_initialize", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").str;
			playerID = id;

			Vector3 location = getLocationField(e.data);
			Quaternion rotation = getRotationField(e.data);

			players[id].transform.position = location;
			players[id].transform.rotation = rotation;
		});
		mySocket.On ("location_update", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").ToString();
			if (players.ContainsKey(id)){
				Vector3 location = getLocationField(e.data);
				Quaternion rotation = getRotationField(e.data);
				players[id].transform.position = location;
				players[id].transform.rotation = rotation;
			} else {
				spawnPlayer(id, e);
			}
		});
		mySocket.On ("player_health_update", (SocketIOEvent e) => {
			string id = e.data.GetField("id").ToString();
			int hp = Int32.Parse(e.data.GetField("hp").ToString());
			players[id].hp = hp;
		});
		mySocket.On ("shot_fired", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").ToString();
			if (players.ContainsKey(id)) {
				players[id].Fire();
			}
		});
		mySocket.On ("player_death", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").ToString();
			players[id].onDeath();

		});
		mySocket.On ("player_respawn", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").ToString();
			if (!id.Equals(playerID)) {
				return;
			}
		});
		mySocket.On ("player_leave", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").ToString();
			if (players.ContainsKey(id)){
				Destroy(players[id].spaceShip);
				players.Remove(id);
			}
		});
        /*
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
        */
	}

	void spawnPlayer (string id, SocketIOEvent e){
		Vector3 initialLocation = getLocationField(e.data);
		Quaternion initialRotation = getRotationField(e.data);
		SpaceShipSkeleton newPlayer = Instantiate(playerPrefab, initialLocation, initialRotation) as SpaceShipSkeleton;
		players.Add(id, newPlayer);
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
