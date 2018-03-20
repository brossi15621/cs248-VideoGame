using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class AdditionalFPC : MonoBehaviour {

	public GameObject candle;
	private int candlesLeft;
	public Transform mainCharacter;

	CharacterController myController;


	// Use this for initialization
	void Start () {
		myController = gameObject.GetComponent<CharacterController> ();
		candlesLeft = 5;
	}
	
	// Update is called once per frame
	void Update () {

		//Candle things
		if (mainCharacter.GetComponent<FirstPersonController>().isActiveAndEnabled) {
			//if first person controller is enabled, we can place candles
			if (myController.isGrounded && Input.GetButtonDown ("Candle")) {
				placeCandle ();
			}
		}
	}

	/**
	 * Places a candle in the world a few feet in front of you.
	 * There is still a bug in this in that players 
	 * can place candles in trees and things
	 */ 
	private void placeCandle(){
		if (candlesLeft > 0) {
			Vector3 placement = GameObject.Find ("CandlePlacement").transform.position;
			Instantiate (candle, placement, Quaternion.identity);
			candlesLeft--;
		}
	}

	/**
	 * When player dies the candles get reset
	 */ 
	public void resetCandles(){
		candlesLeft = 5;
	}
}
