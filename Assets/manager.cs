using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour {
	public GameObject playerCamera;
	public GameObject overHeadCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("e")) {
			swapCamera();
		}
	}

	void swapCamera(){
		playerCamera.GetComponent<Camera> ().enabled = !playerCamera.GetComponent<Camera> ().enabled;
		overHeadCamera.GetComponent<Camera> ().enabled = !overHeadCamera.GetComponent<Camera> ().enabled;

	}
}
