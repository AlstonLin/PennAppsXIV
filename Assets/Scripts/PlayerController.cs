using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class PlayerController : MonoBehaviour {
	private const float MOVE_SPEED = 3.0f;
	private const float FIRE_INTERVAL = 0.5f;

	private bool pressed = false;
	private float fireTimeRemaining = 0;
	private int id = -1;

	private GameObject go;
	private SocketIOComponent mySocket;

	private CharacterController controller;

	void Start () {
		controller = (CharacterController) GetComponent(typeof(CharacterController));
		go = GameObject.Find("SocketIO");
		mySocket = (SocketIOComponent) go.GetComponent(typeof(SocketIOComponent));
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

	private bool isPressed (){
		if (Input.GetMouseButtonDown(0)) {
			pressed = true;
			mySocket.Emit("melissa_mouse_down"); 
		} else if (Input.GetMouseButtonUp(0)) {
			pressed = false;
		}
		return GvrViewer.Instance.Triggered || pressed;
	}
}
