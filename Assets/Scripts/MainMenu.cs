using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour, IGvrGazeResponder {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartGame() {
        Debug.Log("start game");
        SceneManager.LoadScene(1);
    }

	#region IGvrGazeResponder implementation


	public void OnGazeEnter() {
	}


	public void OnGazeExit() {
	}

	public void OnGazeTrigger() {
		StartGame ();
	}

	#endregion

}
