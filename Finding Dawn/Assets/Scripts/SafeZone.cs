﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Snake") {
			other.gameObject.GetComponent<SnakeAIController> ().destroySnake ();
		} else if (other.tag == "Giant") {
			other.gameObject.GetComponent<GiantAIController> ().inSafeZone ();
		} else if (other.tag == "Humanoid") {

		}
	}
}
