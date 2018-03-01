using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSpawner : MonoBehaviour {
	private GameManagerScript manager;

	// Use this for initialization
	void Start () {
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player" && manager.found) {
			//release a snake
			print("Releasing snake");
		}
	}
}
