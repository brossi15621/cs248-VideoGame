using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAIController : MonoBehaviour {

	private Transform mainCharacter;
	private Renderer myRenderer;
	private CharacterController myCharacterController;
	private bool patrol = true;
	public GameObject[] waypoints;
	int currentWaypoint;
	float gravity = -10f;
	public float accuracyWaypoint = 5.0f;
	public float patrolSpeed = 3.0f;
	public float alertSpeed = 10.0f;
	public float patrolRotationSpeed = 0.01f;
	public float alertRotationSpeed = 0.1f;
	public float findAngle = 60f;
	public float findDistance = 40f;


	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<Renderer> ();
		myCharacterController = GetComponent<CharacterController> ();

		//This is buggy because it is just getting the parent object and not making
		//an array of its children. personally I think it might be better to just publically enter these.
		//waypoints = GameObject.FindGameObjectsWithTag ("WaypointsHumanoidAI"); 
		currentWaypoint = Random.Range (0, waypoints.Length);
		GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		mainCharacter = mainCamera.transform;
	}

	void update() {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 direction = mainCharacter.position - this.transform.position;
		float angle = Vector3.Angle (direction, this.transform.forward);
		direction.y = 0f;
		//Maybe adding some gravity things
//		if (myCharacterController.isGrounded) {
//			direction.y = 0;
//		} else {
//			direction.y += gravity * Time.deltaTime;
//		}


		if (patrol && waypoints.Length > 0) {
			//patrol
			if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) {
				//select random waypoint to patrol towards
				currentWaypoint = Random.Range(0, waypoints.Length);
			}

			//rotate towards current waypoint
			direction = waypoints[currentWaypoint].transform.position - this.transform.position;
			direction.y = 0f;

//			if (myCharacterController.isGrounded) {
//				direction.y = 0;
//			} else {
//				direction.y += gravity * Time.deltaTime;
//			}
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed);
			myCharacterController.Move(this.transform.forward * Time.deltaTime * patrolSpeed);
		}

		if ( lineOfSight(angle)) {
			//AI alerted, pursue main character
			patrol = false;
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
			myRenderer.material.color = Color.red;
			myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed);
		} else {
			//not alert
			myRenderer.material.color = Color.blue;
			patrol = true;
		}
	}

	private bool lineOfSight(float angle){
		RaycastHit hit;
		//Check Distance
		if (Vector3.Distance (mainCharacter.position, this.transform.position) < findDistance
		//Check angle
			&& (!patrol || angle < findAngle)
		//Check for barriers
			&& ( !patrol || (Physics.Linecast (transform.position, mainCharacter.position, out hit) && (hit.transform.tag == "Player" || hit.transform.tag == "MainCamera")))) {
			return true;
		}
		return false;
	}

}
