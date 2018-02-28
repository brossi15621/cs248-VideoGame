using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAIController : MonoBehaviour {

	private Transform mainCharacter;
	//private Renderer myRenderer;
	private SphereCollider myCollider;
	private CharacterController myCharacterController;
	private bool patrol = true;
	private GameManagerScript manager;
	private float deadSensitivity = .19f; //The sensitity where below this number the controller doesn't recognize it as moving left stick
	private float killDistance = 3f; 
	public GameObject[] waypoints;
	int currentWaypoint;
	public float accuracyWaypoint = 5.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed = 10.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;
	public float chaseDistance = 50f;
	public Animation anim;

    private float gravity = 0f;


	// Use this for initialization
	void Start () {
		//myRenderer = GetComponent<Renderer> ();
		myCharacterController = GetComponent<CharacterController> ();
		myCollider = GetComponent<SphereCollider> ();
		anim = GetComponent<Animation> ();
		manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();

		//This is buggy because it is just getting the parent object and not making
		//an array of its children. personally I think it might be better to just publically enter these.
		//waypoints = GameObject.FindGameObjectsWithTag ("WaypointsHumanoidAI"); 
		currentWaypoint = Random.Range (0, waypoints.Length);
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		mainCharacter = mainCamera.transform;
	}

	// Public getter for other snakes to communicate
	public bool isPatrolling() {
		return patrol;
	}

	void Update() {
		if (Input.GetButton ("Run")) {
			// For new snake prefab with snake model, this should be ~4.0f
			myCollider.radius = 16f;
		} else {
			// For new snake prefab with snake model, this should be ~2.0f
			myCollider.radius = 8f;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 direction = mainCharacter.position - this.transform.position;
        gravity -= 9.81f * Time.deltaTime;
        direction.y = gravity;


		if (patrol && waypoints.Length > 0) {
			//patrol
			if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
				//select random waypoint to patrol towards
				currentWaypoint = Random.Range(0, waypoints.Length);
			}

			//rotate towards current waypoint
			direction = waypoints[currentWaypoint].transform.position - this.transform.position;
			direction.y = gravity;

			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
			myCharacterController.Move(this.transform.forward * Time.deltaTime * patrolSpeed);
		}
		float distance = Vector3.Distance (mainCharacter.position, this.transform.position);

		if (!patrol && distance < chaseDistance) {
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
			myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed);
		} else if (distance < 4.0f) {
			patrol = false;
			//myRenderer.material.color = Color.red;
		} else {
			//not alert
			//myRenderer.material.color = Color.blue;
			patrol = true;
		}
		if (distance <= killDistance) {
			manager.dead = true;
		}
        if (myCharacterController.isGrounded)
            gravity = 0f;
	}

	void OnTriggerStay(Collider other) {
		if (patrol) {
			//Checks if colliding with player/camera
			if (other.tag == "Player" || other.tag == "MainCamera") { 
				//Checks if player is moving or jumping
				if (Input.GetAxis ("Horizontal") > deadSensitivity || Input.GetAxis ("Vertical") > deadSensitivity || Input.GetButton ("Jump")) {
					//If so, alerts the snake.
					patrol = false;
					Vector3 direction = mainCharacter.position - this.transform.position;
					this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
					//myRenderer.material.color = Color.red;
					myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed);
				}
			}
		}
	}

}
