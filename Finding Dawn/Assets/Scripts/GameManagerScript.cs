using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManagerScript : MonoBehaviour {
	
	private Transform mainCharacter;
	private Light CharacterLight;
	public bool dead = false;
	public int numSnakesChasing = 0;
	private int numSnakesChasingLast = 0;
	public int numGiantsChasing = 0;
	private int numGiantsChasingLast = 0;
	public int numHumanoidsChasing = 0;
	private int numHumanoidsChasingLast = 0;
	private bool beingChased = false;
	public GameObject[] snakesMade;
	public int currSnakeIndex = 0;
	public GameObject CharacterLightObject;
	public static GameManagerScript instance = null;

	public AudioMixerSnapshot outOfCombat;
	public AudioMixerSnapshot inCombat;
	public AudioClip[] stings;
	public AudioSource stingSource;
	public float bpm = 128;

	private float m_TransitionIn;
	private float m_TransitionOut;
	private float m_QuarterNote;

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

		m_QuarterNote = 60 / bpm;
		m_TransitionIn = m_QuarterNote * 2;
		m_TransitionOut = m_QuarterNote * 32;
	}
	
	// Update is called once per frame
	void Update () {
		bool transitionToChase = false;
		bool transitionFromChase = false;
		if (!beingChased) {
			if (numSnakesChasing > 0 && numSnakesChasingLast == 0) {
				transitionToChase = true;
			} else if (numGiantsChasing > 0 && numGiantsChasingLast == 0) {
				transitionToChase = true;
			} else if (numHumanoidsChasing > 0 && numHumanoidsChasingLast == 0) {
				transitionToChase = true;
			}
		} else {
			if ((numSnakesChasing == 0 && numSnakesChasingLast > 0) && numGiantsChasing == 0 && numHumanoidsChasing == 0) {
				transitionFromChase = true;
			} else if ((numGiantsChasing == 0 && numGiantsChasingLast > 0) && numSnakesChasing == 0 && numHumanoidsChasing == 0) {
				transitionFromChase = true;
			} else if ((numHumanoidsChasing == 0 && numHumanoidsChasingLast > 0) && numSnakesChasing == 0 && numGiantsChasing == 0) {
				transitionFromChase = true;
			}
		}

		if (transitionToChase) {
			//went from quiet state to chasing state, change music
			inCombat.TransitionTo (m_TransitionIn);
			stingSource.Play ();
			beingChased = true;
		}

		if (transitionFromChase) {
			//went from chasing state to quiet state, change music
			outOfCombat.TransitionTo(m_TransitionOut);
			beingChased = false;
		}

		numSnakesChasingLast = numSnakesChasing;
		numGiantsChasingLast = numGiantsChasing;
		numHumanoidsChasingLast = numHumanoidsChasing;

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

		outOfCombat.TransitionTo(m_TransitionOut);
		beingChased = false;
		numSnakesChasing = 0;
		numSnakesChasingLast = 0;
		numGiantsChasing = 0;
		numGiantsChasingLast = 0;
		numHumanoidsChasing = 0;
		numHumanoidsChasingLast = 0;
		currSnakeIndex = 0;

		gameObject.GetComponent<AdditionalFPC> ().resetCandles ();
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
		gameObject.transform.position = GameObject.Find ("RespawnPoint").transform.position;
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
