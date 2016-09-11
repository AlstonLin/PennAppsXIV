using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpaceShipSkeleton : MonoBehaviour, IGvrGazeResponder {
	private const float MOVE_SPEED = 1.0f;

	public GameObject laser, spaceShip, arrowPrefab;

	public int hp;

	private GameObject hud, arrow;
    private Vector3 startingPosition;

	private string id = "";

    void Start() {
		hud = GameObject.Find ("/Main Camera/HUD");
		arrow = Instantiate (arrowPrefab, hud.transform.position, Quaternion.identity) as GameObject;
		arrow.transform.parent = hud.transform;
        startingPosition = transform.localPosition;
        SetGazedAt(false);
    }

	void Update(){
		arrow.transform.localPosition = new Vector3 (0, 15, 20);
		// Calculates direction for the arrow
		GameObject player = GameObject.Find("/Main Camera/Space Ship (Friendly)");
		Vector3 playerPos = player.transform.position;
		Vector3 dirToEnemy = playerPos - transform.position;
		float size = (float) (1 / Math.Log(dirToEnemy.magnitude));
		arrow.transform.localScale = new Vector3 (size, size, size);
		arrow.transform.LookAt (transform.position);
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

	public void Fire(string shooterId) {
		Debug.Log ("skeleton ship fire called");
        Laser newLaser = Instantiate(laser, transform.TransformPoint(Vector3.forward * 15), Quaternion.Euler(transform.eulerAngles.x + 90, transform.eulerAngles.y, 0)) as Laser;
		newLaser.shooterId = shooterId;
	}

    public void onDeath() {
        Debug.Log("onDeath start");
        Destroy(spaceShip);
        Debug.Log("onDeath end");
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


	public void disposeArrow() {
		Destroy (arrow);
	}

    /// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
    public void OnGazeTrigger() {    }

    #endregion
}