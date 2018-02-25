using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorController : MonoBehaviour {
	private MeshRenderer myRenderer;

	// Use this for initialization
	void Start () {
		myRenderer = gameObject.GetComponent<MeshRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Mirror")) {
			if (myRenderer.enabled) {
				myRenderer.enabled = false;
			} else {
				myRenderer.enabled = true;
			}
		}
	}
}
