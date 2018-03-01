using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
	
	private Transform mainCharacter;
	public bool dead = false;
	public int numSnakesChasing = 0;
	public GameObject[] snakesMade;
	public int currSnakeIndex = 0;

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

	/**
	 * Only destroys snake and sets to null if a snake has been made in that array.
	 * Also decrements the count on snakes chasing you by 1. 
	 */ 
	public void destroySnake(GameObject snake, int index){
		if (snakesMade [index] != null) {
			Destroy (snakesMade [index]);
			snakesMade [index] = null;
			numSnakesChasing--;
		}
	}

	/**
	 * Checks if we are at the end of the array of snakes added
	 * if so restart at the begining of the array.
	 * Then destroy any snake in the world that is at that index of the array
	 * Make a new snake and release it.
	 */ 
	public void addSnake (GameObject snake){
		if (currSnakeIndex > maxSnakes - 1) {
			currSnakeIndex = 0;
		}

		//Gets snake script and makes it chase player and keeps track of its index
		SnakeAIController snakeScript = snake.GetComponent<SnakeAIController> ();
		snakeScript.setPatrol (false);
		snakeScript.setIndex (currSnakeIndex);

		destroySnake (snake, currSnakeIndex);
		snakesMade [currSnakeIndex] = snake;
		currSnakeIndex++;
		numSnakesChasing++;
	}

}
