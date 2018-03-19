using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighthouseLight : MonoBehaviour {

	Animation rotating;

	// Use this for initialization
	void Start () {
		rotating = gameObject.GetComponent<Animation> ();
		rotating ["rotate_light"].speed = .1f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
