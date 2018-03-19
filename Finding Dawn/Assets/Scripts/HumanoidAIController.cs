using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HumanoidAIController : MonoBehaviour
{

	private Transform mainCharacter;
	private SphereCollider myCollider;
	private CharacterController myCharacterController;
	private Animator myAnimator;
	private GameManagerScript manager;
	private bool patrol = true;
	private float gravity = 0f;
	private bool movingBack = false;
	private bool isJumpingLantern = false;
	private bool dead = false;
	int currentWaypoint;
	private GameObject[] waypoints;
	public GameObject waypointParent;
	public float accuracyWaypoint = 5.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed = 11.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;
	public float findAngle = 60f;
	public float findDistance = 40f;
	public float chaseDistance = 70f;
	public float killDistance = 4f;
	public AudioSource audioSource;
	public AudioClip[] audioClips;

		


	// Use this for initialization
	void Start ()
	{
		myCharacterController = GetComponent<CharacterController> ();
		myCollider = GetComponent<SphereCollider> ();
		myAnimator = GetComponent<Animator> ();
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera") [0];
		mainCharacter = mainCamera.transform;

		//Getting waypoint
		waypoints = new GameObject[waypointParent.transform.childCount];
		for (int i = 0; i < waypoints.Length; i++) {
			waypoints [i] = waypointParent.transform.GetChild (i).gameObject;
		}
		currentWaypoint = Random.Range (0, waypoints.Length);
	}

	void Update ()
	{
		if (Input.GetButton ("Run")) {
			myCollider.radius = 8f;
		} else {
			myCollider.radius = 5f;
		}
	}
	    
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (!movingBack && !dead) {
			Vector3 direction = mainCharacter.position - this.transform.position;
			float angle = Vector3.Angle (direction, this.transform.forward);
			gravity -= 9.81f * Time.deltaTime;
			direction.y = 0f;


			if (patrol && waypoints.Length > 1) {
				//patrol
				myAnimator.SetBool ("isWalking", true);
				myAnimator.SetBool ("isIdle", false);
				if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
					//select random waypoint to patrol towards
					currentWaypoint = Random.Range (0, waypoints.Length);
				}

				//rotate towards current waypoint
				direction = waypoints [currentWaypoint].transform.position - this.transform.position;
				direction.y = 0f;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
				Vector3 moveDirection = transform.forward;
				moveDirection *= patrolSpeed;
				moveDirection.y = gravity;
				myCharacterController.Move (moveDirection * Time.deltaTime);
			} else if (patrol && waypoints.Length == 1) {
				//Idle State
				myAnimator.SetBool ("isIdle", true);
				myAnimator.SetBool ("isWalking", false);
			}

			float distance = Vector3.Distance (mainCharacter.position, this.transform.position);
			// Noise Detection
			if (!patrol && distance < chaseDistance) {
				myAnimator.SetBool ("isWalking", false);
				myAnimator.SetBool ("isIdle", false);
				patrol = false;
				direction.y = 0f;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
				Vector3 moveDirection = transform.forward;
				moveDirection *= alertSpeed;
				moveDirection.y = gravity;
				myCharacterController.Move (moveDirection * Time.deltaTime);
			} else if (lineOfSight (angle)) {
				//AI alerted, pursue main character
				myAnimator.SetBool ("isWalking", false);
				myAnimator.SetBool ("isIdle", false);

				if (patrol) {
					//move from state of patrolling to a state of pursuit
					manager.numHumanoidsChasing++;
					int randClip = Random.Range (1, audioClips.Length);
					audioSource.clip = audioClips[randClip];
					audioSource.Play();
				}

				patrol = false;
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
				Vector3 moveDirection = transform.forward;
				moveDirection *= alertSpeed;
				moveDirection.y = gravity;
				myCharacterController.Move (moveDirection * Time.deltaTime);
			} else {
				
				if (!patrol) {
					//move from state of pursuit to state of patrol
					manager.numHumanoidsChasing--;
				}

				patrol = true;
				if (waypoints.Length > 1) {
					myAnimator.SetBool ("isWalking", true);
					myAnimator.SetBool ("isIdle", false);
				} else {
					myAnimator.SetBool ("isIdle", true);
					myAnimator.SetBool ("isWalking", false);
				}
			}

			if (myCharacterController.isGrounded)
				gravity = 0f;
				
			if (distance <= killDistance) {
				manager.dead = true;
			}
		} else if(movingBack && !dead){
			myCharacterController.Move (transform.forward * Time.deltaTime * -3f);
		} 
	}

	private bool lineOfSight (float angle)
	{
		RaycastHit hit;
		float distance = Vector3.Distance (mainCharacter.position, this.transform.position);
		//Check Distance

		if (((patrol && distance < findDistance) || (!patrol && distance < chaseDistance))
			//Check angle
		    && (!patrol || angle < findAngle)) {
			//In finding area now check for barriers
			if (!patrol || (!Physics.Linecast (transform.position, mainCharacter.position, out hit) || hit.transform.tag == "Player")) {
				//If there is a direct line between player and giant. Meaning terrain is not in way.
				//However there is a bug here where if there is a snake between you two it will not see you.
				//Linecast hits the snake collider.
				return true;
			} 
		}
		return false;
	}

	void OnTriggerStay (Collider other)
	{
		if (patrol) {
			if (other.tag == "Player" || other.tag == "MainCamera") {
				if (Input.GetAxis ("Horizontal") != 0f || Input.GetAxis ("Vertical") != 0 || Input.GetButton ("Jump")) {
					//move from state of patrol to state of pursuit
					myAnimator.SetBool ("isWalking", false);
					manager.numHumanoidsChasing++;
					int randClip = Random.Range (1, audioClips.Length);
					audioSource.clip = audioClips[randClip];
					audioSource.Play();
					patrol = false;
					Vector3 direction = mainCharacter.position - this.transform.position;
					this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
					Vector3 moveDirection = transform.forward;
					moveDirection *= alertSpeed;
					moveDirection.y = gravity;
					myCharacterController.Move (moveDirection * Time.deltaTime);
				}
			}
		}
	}

	public void enterSafeZone (bool isLantern)
	{
		if (isLantern && !isJumpingLantern) {
			isJumpingLantern = true;
			movingBack = true;
			myAnimator.SetTrigger ("inLantern");
			StartCoroutine (stumble ());
		} else if(!isLantern){
			manager.numHumanoidsChasing--;
			audioSource.clip = audioClips[0];
			audioSource.Play();
			dead = true;
			myAnimator.SetBool ("inSafeZone", true);
		}
	}

	IEnumerator stumble(){
		yield return new WaitForSeconds (1.5f);
		movingBack = false;
		yield return new WaitForSeconds (.5f);
		isJumpingLantern = false;
	}

}


