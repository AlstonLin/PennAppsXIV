using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class SpaceShip : MonoBehaviour, IGvrGazeResponder {
    public float MOVE_SPEED;
	private const int STARTING_AMMO = 30;

    public GameObject spaceShip, socketObj, gameOverText, youWinText;
    public Laser laser;
    public GameObject[] healthBars;
	public CharacterController controller;

    public TextMesh enemyHp, noOfKills;

	public SocketIOComponent socket;
    private Vector3 startingPosition;

	public int hp;
	public float fireInterval;
	private float ammoAmount = STARTING_AMMO;
    private float fireTimeRemaining = 0;
    private bool pressed = false;
	private bool isDead = false;

	public int kills = 0;

    void Start() {
		socket = socketObj.GetComponent (typeof(SocketIOComponent)) as SocketIOComponent;
        startingPosition = transform.localPosition;
        SetGazedAt(false);
		//setAmmoText ();
		gameOverText.SetActive (false);
		youWinText.SetActive (false);
    }

    void LateUpdate() {
        GvrViewer.Instance.UpdateState();
        if (GvrViewer.Instance.BackButtonPressed) {
            Application.Quit();
        }
    }

    public void SetGazedAt(bool gazedAt) {
        if(gazedAt) {
            GetComponent<Renderer>().material.color = Color.red;
        } else {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    public void Reset() {
        transform.localPosition = startingPosition;
    }

    public void ToggleVRMode() {
        GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
    }

    public void ToggleDistortionCorrection() {
        switch (GvrViewer.Instance.DistortionCorrection) {
            case GvrViewer.DistortionCorrectionMethod.Unity:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Native;
                break;
            case GvrViewer.DistortionCorrectionMethod.Native:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.None;
                break;
            case GvrViewer.DistortionCorrectionMethod.None:
            default:
                GvrViewer.Instance.DistortionCorrection = GvrViewer.DistortionCorrectionMethod.Unity;
                break;
        }
    }

    public void ToggleDirectRender() {
        GvrViewer.Controller.directRender = !GvrViewer.Controller.directRender;
    }

    public void Fire() {
        /*
		if (ammoAmount <= 0){
			return;
		}
        */
        fireTimeRemaining = fireInterval;

        //currently broken
        Laser newLaser = Instantiate(laser, transform.TransformPoint(Vector3.forward * 15), Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, 0)) as Laser;
		newLaser.shooterId  = NetworkController.playerID;

		ammoAmount--;
		//setAmmoText ();
		JSONObject json = new JSONObject ();

		json.AddField ("player_id", NetworkController.playerID);
		socket.Emit ("shot_fired", json);
    }

    void Update() {

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, 400)) {
            SpaceShipSkeleton ship = hit.transform.gameObject.GetComponent<SpaceShipSkeleton>();
            if (ship != null) {
                enemyHp.text = ship.hp.ToString();
            } 
        } else {
            enemyHp.text = "";
        }

		noOfKills.text = "Kills: " + kills; 

        moveForward ();
		//Debug.Log(string.Format("x:{0:g}, y:{1:g}, z:{2:g}", transform.position.x, transform.position.y, transform.position.z));

		if (isDead) {
			MOVE_SPEED *= 0.97F;
			gameOverText.transform.parent = null;
			return;
		}

		if (fireTimeRemaining > 0) {
			fireTimeRemaining -= Time.deltaTime;
		}
		// Checks for fire
		if (IsPressed () && fireTimeRemaining <= 0) {
			Fire ();
		}
    }

    private bool IsPressed() {
        if (Input.GetMouseButtonDown(0)) {
            pressed = true;
        }
        else if (Input.GetMouseButtonUp(0)) {
            pressed = false;
        }
        return GvrViewer.Instance.Triggered || pressed;
    }

    /*
	private void setAmmoText (){
		ammoText.text = "Ammo: " + ammoAmount;
	}
    */

	private void moveForward (){
		Vector3 forward = transform.forward;
		controller.Move (forward * MOVE_SPEED * Time.deltaTime);

        if (!isDead) {
            JSONObject json = new JSONObject();

            json.AddField("player_id", NetworkController.playerID);
            json.AddField("location_x", transform.position.x);
            json.AddField("location_y", transform.position.y);
            json.AddField("location_z", transform.position.z);

            json.AddField("rotation_x", transform.rotation.eulerAngles.x);
            json.AddField("rotation_y", transform.rotation.eulerAngles.y);
            json.AddField("rotation_z", transform.rotation.eulerAngles.z);
            socket.Emit("location_update", json);
        }
	}

    void OnCollisionEnter(Collision collisionInfo) {
        Debug.Log("spaceship: onCollisionEnter");

        Laser laser = collisionInfo.gameObject.GetComponent<Laser>();
        //returning nothing, not working
        Debug.Log("laser id: " + laser.shooterId);
		GetHit (laser.shooterId);
    }

	void GetHit(string shooterId) {
        hp--;
        Destroy(healthBars[hp]);
		JSONObject json = new JSONObject ();
		json.AddField ("player_id", NetworkController.playerID);
		json.AddField ("hp", (float)hp);
		socket.Emit ("player_health_update", json);
        if(hp < 1) {
			onDeath(shooterId);
        }
    }

    public void onWin() {
        youWinText.SetActive(true);
        socket.Emit("game_end");
        SceneManager.LoadScene(0);
    }

	public void onDeath(string shooterId) {
        Destroy(spaceShip, 3);
		JSONObject json = new JSONObject ();
		json.AddField ("player_id", NetworkController.playerID);
		json.AddField ("shooter_id", shooterId);
		socket.Emit ("player_death", json);

		gameOverText.SetActive (true);
		isDead = true;
    }

    #region IGvrGazeResponder implementation

        /// Called when the user is looking on a GameObject with this script,
        /// as long as it is set to an appropriate layer (see GvrGaze).
    public void OnGazeEnter() {
        SetGazedAt(true);
    }

    /// Called when the user stops looking on the GameObject, after OnGazeEnter
    /// was already called.
    public void OnGazeExit() {
        SetGazedAt(false);
    }

    /// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
    public void OnGazeTrigger() {    }

    #endregion
}