using UnityEngine;
using System.Collections;
using SocketIO;

public class Laser : MonoBehaviour {

    public GameObject laser;
    public float speed;

	public string shooterId;

    public float selfDestructMaxTime;

	void Start () {
        Destroy(laser, selfDestructMaxTime);
    }
	
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
