using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour {

	private GameManagerScript manager;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(){
		manager.dead = true;
	}
}
