using UnityEngine;
using SocketIO;

[RequireComponent(typeof(Collider))]
public class SpaceShip : MonoBehaviour, IGvrGazeResponder {
	private const float MOVE_SPEED = 1.0f;

	public GameObject laser, spaceShip, socketObj;
	public CharacterController controller;
	public int hp;
    public float fireInterval;

	private SocketIOComponent socket;
    private Vector3 startingPosition;

	private float id = -1;
    private float fireTimeRemaining = 0;
    private bool pressed = false;

    void Start() {
		socket = socketObj.GetComponent (typeof(SocketIOComponent)) as SocketIOComponent;
        startingPosition = transform.localPosition;
        SetGazedAt(false);
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
        fireTimeRemaining = fireInterval;
        GameObject newLaser = Instantiate(laser, transform.TransformPoint(Vector3.forward * 15), Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, 0)) as GameObject;
		socket.Emit ("shot_fired", new JSONObject());
    }

    void Update() {
		if (id != NetworkController.playerID) {
			return;
		}
		moveForward ();

		//Debug.Log(string.Format("x:{0:g}, y:{1:g}, z:{2:g}", transform.position.x, transform.position.y, transform.position.z));

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

	private void moveForward (){
		Vector3 forward = transform.forward;
		controller.Move (forward * MOVE_SPEED * Time.deltaTime);
		JSONObject json = new JSONObject ();
		json.AddField ("player_id", NetworkController.playerID);
		json.AddField ("location_x", transform.position.x);
		json.AddField ("location_y", transform.position.y);
		json.AddField ("location_z", transform.position.z);
		json.AddField ("rotation_x", transform.rotation.x);
		json.AddField ("rotation_y", transform.rotation.y);
		json.AddField ("rotation_z", transform.rotation.z);
		socket.Emit ("location_update", json);
	}

    void OnCollisionEnter(Collision collisionInfo) {
        Debug.Log("spaceship: onCollisionEnter");
        getHit();
    }

    void getHit() {
        hp--;
        if(hp < 1) {
            onDeath();
        }
    }

    void onDeath() {
        Destroy(spaceShip);
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