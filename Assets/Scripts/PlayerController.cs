using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class PlayerController : MonoBehaviour {
	private const float MOVE_SPEED = 1.0f;
	private const float FIRE_INTERVAL = 0.5f;

	private bool pressed = false;
	private float fireTimeRemaining = 0;
	private int id = -1;

	private GameObject go;
	private SocketIOComponent sock;

	private CharacterController controller;

	void Start () {
		controller = (CharacterController) GetComponent(typeof(CharacterController));
		go = GameObject.Find("SocketIO");
		sock = (SocketIOComponent) go.GetComponent(typeof(SocketIOComponent));
	}

	void Update () {
		if (id != NetworkController.playerID) {
			return;
		}
		moveForward ();
	}

	private void moveForward (){
		Vector3 forward = transform.forward;
		controller.Move (forward * MOVE_SPEED * Time.deltaTime);
	}
}
