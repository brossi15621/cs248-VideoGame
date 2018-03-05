using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalFPC : MonoBehaviour {

	public GameObject candle;
	private int candlesLeft;

	CharacterController myController;


	// Use this for initialization
	void Start () {
		myController = gameObject.GetComponent<CharacterController> ();
		candlesLeft = 5;
	}
	
	// Update is called once per frame
	void Update () {
		if (myController.isGrounded && Input.GetButtonDown("Candle")) {
			placeCandle ();
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
