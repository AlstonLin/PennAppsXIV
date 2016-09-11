using UnityEngine;
using System.Collections;
using SocketIO;

public class Laser : MonoBehaviour {

    public GameObject laser;
    public float speed;

	public string shooterId;

    public float selfDestructMaxTime;

	public GameObject socketObj;
	private SocketIOComponent mySocket;

	// Use this for initialization
	void Start () {
		mySocket = (SocketIOComponent) socketObj.GetComponent (typeof(SocketIOComponent));
        Destroy(laser, selfDestructMaxTime);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(speed * Vector3.up * Time.deltaTime);	
	}

    void OnCollisionEnter(Collision collisionInfo) {
        Debug.Log("OnCollisionEnter: laser");
        Destroy(laser);
    }

	void setShooterId(string id) {
		this.shooterId = id;
	}
}
