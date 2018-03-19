using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseLight : MonoBehaviour {

	Animation rotating;

	// Use this for initialization
	void Start () {
		rotating = gameObject.GetComponent<Animation> ();
		StartCoroutine ("QuickStart");
	}
	
	IEnumerator QuickStart(){
		yield return new WaitForSeconds (2f);
		rotating ["rotate_light"].speed = .1f;
	}
}
