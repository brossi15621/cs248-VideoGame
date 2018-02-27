using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorController : MonoBehaviour {
	private RawImage myMirror;

	// Use this for initialization
	void Start () {
		myMirror = gameObject.GetComponent<RawImage> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Mirror")) {
			if (myMirror.enabled) {
				
				myMirror.enabled = false;
			} else {
				myMirror.enabled = true;
			}
		}
	}
}
