using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAIController : MonoBehaviour {

	    private Transform mainCharacter;
		private SphereCollider myCollider;
	    private CharacterController myCharacterController;
		private Animator myAnimator;
		private GameManagerScript manager;
	    private bool patrol = true;
		private float gravity = 0f;
	    public GameObject[] waypoints;
		int currentWaypoint;
	    public float accuracyWaypoint = 5.0f;
	    public float patrolSpeed = 3.0f;
	    public float alertSpeed = 10.0f;
	    public float patrolRotationSpeed = 0.01f;
	    public float alertRotationSpeed = 0.1f;
	    public float findAngle = 60f;
	    public float findDistance = 40f;
	    public float chaseDistance = 50f;
		public float killDistance = 4f;
		


	    // Use this for initialization
	    void Start () {
		        myCharacterController = GetComponent<CharacterController> ();
				myCollider = GetComponent<SphereCollider> ();
				myAnimator = GetComponent<Animator> ();
				manager = GameObject.Find ("Player").GetComponent<GameManagerScript> ();
		        currentWaypoint = Random.Range (0, waypoints.Length);
		        GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0];
		        mainCharacter = mainCamera.transform;
		    }

	    void Update() {
			if (Input.GetButton ("Run")) {
				myCollider.radius = 10f;
			} else {
				myCollider.radius = 6f;
			}
		}
	    
	    // Update is called once per frame
	    void FixedUpdate () {
		        Vector3 direction = mainCharacter.position - this.transform.position;
		        float angle = Vector3.Angle (direction, this.transform.forward);
				gravity -= 9.81f * Time.deltaTime;
				direction.y = gravity;


		        if (patrol && waypoints.Length > 0) {
			            //patrol
						myAnimator.SetBool("isWalking", true);
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
		        // Noise Detection
		        if (!patrol && distance < chaseDistance) {
						myAnimator.SetBool("isWalking", false);
						patrol = false;
			            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
			            myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed);
		        } else if(distance < 4.0f){
						myAnimator.SetBool("isWalking", false);
			            patrol = false;
		        } else if ( lineOfSight(angle)) {
			            //AI alerted, pursue main character
						myAnimator.SetBool("isWalking", false);
			            patrol = false;
			            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
			            myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed);
		        } else {
			        patrol = true;
					myAnimator.SetBool("isWalking", true);
			    }

				if (myCharacterController.isGrounded)
					gravity = 0f;
				
				if (distance <= killDistance) {
					manager.dead = true;
				}
			
		    }

		private bool lineOfSight(float angle){
		RaycastHit hit;
		//Check Distance
		if (Vector3.Distance (mainCharacter.position, this.transform.position) < findDistance
			//Check angle
			&& (!patrol || angle < findAngle)) {
			//In finding area now check for barriers
			if (!Physics.Linecast (transform.position, mainCharacter.position, out hit) || hit.transform.tag == "Player") {
				//If there is a direct line between player and giant. Meaning terrain is not in way.
				return true;
			}
		}
		return false;
		}

	    void OnTriggerStay(Collider other) {
		        if (patrol) {
			            if (other.tag == "Player" || other.tag == "MainCamera") {
				                if (Input.GetButton ("Horizontal") || Input.GetButton ("Vertical") || Input.GetButton ("Jump")) {
										myAnimator.SetBool("isWalking", false);
					                    patrol = false;
					                    Vector3 direction = mainCharacter.position - this.transform.position;
					                    this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed);
					                    myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed);
					                }
				            }
			        }
		    }

}


