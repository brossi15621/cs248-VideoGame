using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EndGameMusicScript : MonoBehaviour {
	public AudioMixerSnapshot endGame;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player" || other.tag == "MainCamera") {
			//player entered sphere collider to trigger end-game music
			endGame.TransitionTo (4);
		}
	}
}
