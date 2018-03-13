using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
	
	private Transform mainCharacter;
	private Light CharacterLight;
	public bool dead = false;
	public int numSnakesChasing = 0;
	public GameObject[] snakesMade;
	public int currSnakeIndex = 0;
	public GameObject CharacterLightObject;
	private Vector3 startPoint = new Vector3 (115.0f, 5.0f, 60.0f);
	public static GameManagerScript instance = null;

	//Constants
	private const int maxSnakes = 10;
	private const float initialLightRange = 24f;
	private const float rangeDecrement = 4f;


	// Use this for initialization
	void Start () {
		if (instance == null ) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		snakesMade = new GameObject[maxSnakes]; 
		CharacterLight = CharacterLightObject.GetComponent<Light> ();
		Application.targetFrameRate = 200;
	}
	
	// Update is called once per frame
	void Update () {
		if (dead) {
			PlayerDeath ();
		}	
	}

	void PlayerDeath () {
		CharacterLight.range -= rangeDecrement;
		if (CharacterLight.range == 0) {
			CharacterLight.range = initialLightRange;
		}
		snakesMade = new GameObject[maxSnakes];
		numSnakesChasing = 0;
		currSnakeIndex = 0;
		gameObject.GetComponent<AdditionalFPC> ().resetCandles ();
		SceneManager.LoadScene ("DemoScene");
		gameObject.transform.position = startPoint;
		dead = false;
	}

	/**
	 * Only destroys snake and sets to null if a snake has been made in that array.
	 * Also decrements the count on snakes chasing you by 1. 
	 */ 
	public void destroyInstantiatedSnake(GameObject snake, int index){
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

		destroyInstantiatedSnake (snake, currSnakeIndex);
		snakesMade [currSnakeIndex] = snake;
		currSnakeIndex++;
		numSnakesChasing++;
	}

}
