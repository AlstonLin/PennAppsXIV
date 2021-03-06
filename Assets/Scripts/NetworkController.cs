﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using System;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour {
	public GameObject socketObj;
	public SpaceShipSkeleton playerPrefab;
	public GameObject[] asteroidPrefabs;
	public GameObject ammoBoxPrefab;

    public SpaceShip clientSpaceShip;
    public GameObject clientPrefab;

	private Dictionary<string, SpaceShipSkeleton> players = new Dictionary<string, SpaceShipSkeleton>();
	public static string playerID = "";
	private SocketIOComponent mySocket;

	void Start () {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
		initializeSocketEvents ();
	}
	void initializeSocketEvents () {
		mySocket.On ("player_initialize", (SocketIOEvent e) => {
            Debug.Log("initializeSocketEvents called");
			string id = e.data.GetField("player_id").str;
			playerID = id;

			Vector3 location = getLocationField(e.data);
			Quaternion rotation = getRotationField(e.data);

			clientPrefab.transform.position = location;
			clientPrefab.transform.rotation = rotation;

		});

		mySocket.On ("set_asteroids", (SocketIOEvent e) => {
			Debug.Log("ASTEROIDS! " + e.data.ToString());
			List<JSONObject> asteroidLocations = e.data.GetField("data").list;
			foreach (JSONObject asteroidLocation in asteroidLocations){
				GameObject asteroidPrefab = asteroidPrefabs[getIntField("type", asteroidLocation)];
				Vector3 location = new Vector3(getFloatField("location_x", asteroidLocation),
					getFloatField("location_y", asteroidLocation), getFloatField("location_z", asteroidLocation));
				Instantiate(asteroidPrefab, location, Quaternion.identity);
			}
		});	
		mySocket.On ("location_update", (SocketIOEvent e) => {
            Debug.Log("Location_update called");
			string id = e.data.GetField("player_id").str;
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
			string id = e.data.GetField("player_id").str;
            int hp = Mathf.RoundToInt(e.data.GetField("hp").f);
            Debug.Log("HP parsed: " + hp);
            if (players.ContainsKey(id)) {
    			players[id].hp = hp;
            }
		});

		mySocket.On ("shot_fired", (SocketIOEvent e) => {

			string id = e.data.GetField("player_id").str;
			Debug.Log("SHOT FIRED BY ID: " + id);
			if (players.ContainsKey(id)) {
				players[id].Fire(id);
			}
		});

		mySocket.On ("player_death", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").str;
			string shooterID = e.data.GetField("shooter_id").str;
            Destroy(players[id].spaceShip);
            players.Remove(id);
            if(players.Count == 0) {
                //won't work if you stay alive as a bystander for the whole time
                Debug.Log("no more players, you win?");
                clientSpaceShip.onWin();
            }
				
            Debug.Log("Shooter id: " + shooterID);
            if(playerID.Equals(shooterID)) {
                clientSpaceShip.kills++;
            }

		});

		mySocket.On ("game_won", (SocketIOEvent e) => {
			SceneManager.LoadScene(0);
		});

        mySocket.On("game_end", (SocketIOEvent e) => {
            SceneManager.LoadScene(0); 
        });

		mySocket.On ("player_respawn", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").str;
			if (!id.Equals(playerID)) {
				return;
			}
		});

		mySocket.On ("player_leave", (SocketIOEvent e) => {
			string id = e.data.GetField("player_id").str;
			Debug.Log("Player Disconnected! ID is " + id);
			if (players.ContainsKey(id)){
				Debug.Log("Player Destroyed!");
				Destroy(players[id].spaceShip);
				players[id].disposeArrow();
				players.Remove(id);
			}
		});
        /*
		mySocket.On ("ammo_spawn", (SocketIOEvent e) => {
			Debug.Log("RECEIVED COMMAND! " + e.str);
			int amount, x, y, z;
			int.TryParse (e.data.GetField("amount").str, out amount);
			int.TryParse (e.data.GetField("location_x").str, out x);
			int.TryParse (e.data.GetField("location_y").str, out y);
			int.TryParse (e.data.GetField("location_z").str, out z);
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
		return float.Parse (data.GetField (key).ToString());
	}

	private int getIntField(string key, JSONObject data) {
		return int.Parse (data.GetField (key).ToString());
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
        return Quaternion.Euler(rotationX, rotationY, rotationZ);
	}
}
