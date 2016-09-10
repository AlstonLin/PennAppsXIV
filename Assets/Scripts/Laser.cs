using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

    public GameObject laser;
    public float speed;

    public float selfDestructMaxTime;

	// Use this for initialization
	void Start () {
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
}
