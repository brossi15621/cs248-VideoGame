using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManagerScript : MonoBehaviour {
	
	public Transform mainCharacter;
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
	public Vector3 respawnPoint;
	public Vector3 originalRespawnPoint;

	//Constants
	private const int maxSnakes = 10;
	private const float initialLightRange = 24f;
	private const float rangeDecrement = 4f;

	public Transform pauseCanvas;


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
		respawnPoint = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Pause")) {
			//player pressed escape
			Pause();
		}

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

	public void Pause() {
		if (pauseCanvas.gameObject.activeInHierarchy == false) {
			//pasue menu not active...set it to be active
			pauseCanvas.gameObject.SetActive (true);
			Time.timeScale = 0;
			mainCharacter.GetComponent<FirstPersonController> ().enabled = false;
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			AudioListener.pause = true;
		} else {
			//close pause menu
			pauseCanvas.gameObject.SetActive (false); 
			Time.timeScale = 1;
			mainCharacter.GetComponent<FirstPersonController> ().enabled = true;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			AudioListener.pause = false;
		}
	}

	public void QuitToMainMenu() {
		Time.timeScale = 1;
		AudioListener.pause = false;
		SceneManager.LoadScene(0); 
		Destroy (gameObject);
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
		gameObject.transform.position = respawnPoint;
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
			if (numSnakesChasing > 0) {
				numSnakesChasing--;
			}
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
