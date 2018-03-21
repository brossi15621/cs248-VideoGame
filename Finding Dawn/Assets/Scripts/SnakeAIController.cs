using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SnakeAIController : MonoBehaviour {

	private Transform mainCharacter;
	//private Renderer myRenderer;
	private SphereCollider myCollider;
	private CharacterController myCharacterController;
	private GameManagerScript manager;
	private float killDistance = 3f; 
	private float gravity = 0f;
	private GameObject[] waypoints;
	int currentWaypoint;
	public GameObject waypointParent;
	public float accuracyWaypoint = 5.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed =10.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;
	public float chaseDistance = 50f;
	public float findRadius = 6f;
	public Animation anim;
	private bool patrol = true;
	private int snakeIndex = -1;
	private float touchDistance = 6f;
	private bool isDead = false;
	private Light snakeLight;
	public bool returnToLife = true;
	public AudioSource hissSource;




	// Use this for initialization
	void Start () {
		//myRenderer = GetComponent<Renderer> ();
		myCharacterController = GetComponent<CharacterController> ();
		myCollider = GetComponent<SphereCollider> ();
		anim = GetComponent<Animation> ();
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
		snakeLight = gameObject.GetComponentInChildren<Light> ();
		snakeLight.color = Color.green;

		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		mainCharacter = mainCamera.transform;

		//Getting waypoint
		if (waypointParent != null) {
			waypoints = new GameObject[waypointParent.transform.childCount];
			for (int i = 0; i < waypoints.Length; i++) {
				waypoints [i] = waypointParent.transform.GetChild (i).gameObject;
			}
			currentWaypoint = Random.Range (0, waypoints.Length);
		}
	}

	void Update() {
		if (Input.GetButton ("Run")) {
			myCollider.radius = findRadius * 1.5f;
		} else {
			myCollider.radius = findRadius;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!isDead) {
			//Get movement direction towards main character including gravity
			Vector3 direction = mainCharacter.position - this.transform.position;
			gravity -= 9.81f * Time.deltaTime;
			//direction.y = 0;

			if (patrol && waypoints [0] != null) {
				moveToWaypoint (direction);
			}

			float distance = Vector3.Distance (mainCharacter.position, this.transform.position);
			if (!patrol && distance < chaseDistance) {
				direction.y -= 1.78f;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
				Vector3 moveDirection = transform.forward;
				moveDirection.y = gravity;
				myCharacterController.Move (moveDirection * Time.deltaTime * alertSpeed);
			} else {
				//not alert
				if (!patrol) {
					//Just been lost
					destroySnake ();
				}
				patrol = true;
			}
			if (distance <= killDistance) {
				manager.dead = true;
			}
			if (myCharacterController.isGrounded)
				gravity = 0f;
		}
	}

	void OnTriggerStay(Collider other) {
		//Patrolling non-instantiated snake
		if (patrol && waypoints != null) {
			lookForPlayer (other);
		}

		//Checks if enemies are close together to push them apart
		if (other.gameObject != gameObject && isEnemy(other.tag) && Vector3.Distance (other.gameObject.transform.position, this.transform.position) < touchDistance) {
			moveAway (other);
		}
		
	}

	/**
	 * Returns true if the tag passed in is an
	 * enemy tag. Returns false otherwise.
	 */ 
	private bool isEnemy(string tag){
		if (tag == "Snake" || tag == "Giant" || tag == "Humanoid") {
			return true;
		}
		return false;
	}

	/**
	 * Checks if the player is what the snake ran into
	 * Then checks if the player is moving at all
	 * If so, it alerts the snake
	 */
	private void lookForPlayer(Collider other){
		//Checks if colliding with player/camera
		if (other.tag == "Player" || other.tag == "MainCamera") { 
			//Checks if player is moving or jumping
			if (Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0f || Input.GetButton ("Jump")) {
				//If so, alerts the snake.
				hissSource.Play (); 
				manager.numSnakesChasing++;
				patrol = false;
				Vector3 direction = mainCharacter.position - this.transform.position;
				//direction.y = 0;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
				Vector3 moveDirection = transform.forward;
				moveDirection *= alertSpeed;
				moveDirection.y = gravity;
				myCharacterController.Move (moveDirection * Time.deltaTime);
			} 
		}
	}

	/**
	 * Moves snake away from object passed in
	 */ 
	private void moveAway(Collider other){
		GameObject otherObject = other.gameObject;
		Vector3 direction = otherObject.transform.position - this.transform.position;
		direction.y = 0f;
		myCharacterController.Move (-direction * Time.deltaTime);
	}
		

	/**
	 * Selects a random waypoint if close to destination waypoint
	 * Rotates to new waypoint
	 * Applies gravity
	 * Moves snake
	 **/
	private void moveToWaypoint(Vector3 direction){
		//patrol
		if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
			//select random waypoint to patrol towards
			currentWaypoint = Random.Range(0, waypoints.Length);
		}

		//rotate towards current waypoint
		direction = waypoints[currentWaypoint].transform.position - this.transform.position;
		direction.y = 0;
		this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
		Vector3 moveDirection = transform.forward;
		moveDirection *= patrolSpeed;
		moveDirection.y = gravity;
		myCharacterController.Move(moveDirection * Time.deltaTime);
	}

	/**
	 * If this snake is a patrolling snake it is sent
	 * back to its first way point patrolling.
	 * If it is an instantiated snake it tells the manager to destroy it.
	 */ 
	public void destroySnake(){
		if (!isDead) { //Need this in case player puts down 2 lanterns
			if (waypoints != null) {
				isDead = true;
				anim.Play ("death");
				StartCoroutine (resetTimer (3f));
			} else {
				isDead = true;
				anim.Play ("death");
				StartCoroutine (destroyTimer (3f));
			}
		}
	}

	public IEnumerator destroyTimer (float time) {
		yield return new WaitForSeconds (time);
		manager.destroyInstantiatedSnake (gameObject, snakeIndex);
	}

	public IEnumerator resetTimer (float time) {
		if (!patrol && manager.numSnakesChasing > 0) {
			manager.numSnakesChasing--;
		}
		yield return new WaitForSeconds (time);
		if (returnToLife) {
			gameObject.transform.position = waypoints [0].transform.position;
			patrol = true;
			isDead = false;
			anim.Play ("move");
		} else {
			this.enabled = false;
		}

	}

	public void setIndex(int index){
		snakeIndex = index;
	}

	public int getIndex(){
		return snakeIndex;
	}

	public bool getPatrolling(){
		return patrol;
	}

	public void setPatrol(bool newPatrol){
		patrol = newPatrol;
	}

}
