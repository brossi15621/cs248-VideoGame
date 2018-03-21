using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;


public class MirrorController : MonoBehaviour {
	private RawImage myMirror;
	private Transform mainCharacter;

	// Use this for initialization
	void Start () {
		myMirror = gameObject.GetComponent<RawImage> ();
		mainCharacter = GameObject.Find ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (mainCharacter.GetComponent<FirstPersonController>().isActiveAndEnabled) {
			//if first person controller enabled, we can toggle mirror
			if (Input.GetButtonDown ("Mirror")) {
				if (myMirror.enabled) {
					myMirror.enabled = false;
				} else {
					myMirror.enabled = true;
				}
			}
		} 
	}
}
