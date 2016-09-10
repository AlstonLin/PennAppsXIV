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
		// Updates the fire timer
		if (fireTimeRemaining > 0) {
			fireTimeRemaining -= Time.deltaTime;
		}
		// Checks for fire
		if (isPressed () && fireTimeRemaining <= 0) {
			fire ();
		}
	}

	private void fire (){
		fireTimeRemaining = FIRE_INTERVAL;
		Debug.Log ("FIRE!");
	}

	private void moveForward (){
		Vector3 forward = transform.forward;
		controller.Move (forward * MOVE_SPEED * Time.deltaTime);
	}

	private bool isPressed (){
		if (Input.GetMouseButtonDown(0)) {
			pressed = true;
			sock.Emit("melissa_mouse_down"); 
		} else if (Input.GetMouseButtonUp(0)) {
			pressed = false;
		}
		return GvrViewer.Instance.Triggered || pressed;
	}
}
