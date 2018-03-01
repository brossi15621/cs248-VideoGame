using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
	
	private Transform mainCharacter;
	public bool dead = false;
	public bool found = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (dead) {
			PlayerDeath ();
		}	
	}

	void PlayerDeath () {
		SceneManager.LoadScene ("DemoScene");
	}
}
