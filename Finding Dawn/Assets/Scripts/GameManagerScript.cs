using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
	
	private Transform mainCharacter;
	public bool dead = false;
	public int numSnakesFound = 0;
	public GameObject[] snakesMade;
	public int snakesMadeCount = 0;

	//Constants
	private const int maxSnakes = 10;

	// Use this for initialization
	void Start () {
		snakesMade = new GameObject[maxSnakes];
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

	public void addSnake (GameObject snake){
		if (snakesMadeCount > maxSnakes - 1) {
			snakesMadeCount = 0;
		}
		Destroy (snakesMade [snakesMadeCount]);
		snakesMade [snakesMadeCount] = snake;
		snakesMadeCount++;
	}
}
