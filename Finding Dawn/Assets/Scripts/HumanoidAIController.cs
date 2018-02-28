﻿using System.Collections; using System.Collections.Generic; using UnityEngine;  public class HumanoidAIController : MonoBehaviour {  	    private Transform mainCharacter; 	    private Renderer myRenderer; 		private SphereCollider myCollider; 	    private CharacterController myCharacterController; 	    private bool patrol = true; 	    public GameObject[] waypoints; 	    int currentWaypoint; 	    float gravity = -10f; 	    public float accuracyWaypoint = 5.0f; 	    public float patrolSpeed = 3.0f; 	    public float alertSpeed = 10.0f; 	    public float patrolRotationSpeed = 0.01f; 	    public float alertRotationSpeed = 0.1f; 	    public float findAngle = 60f; 	    public float findDistance = 40f; 	    public float chaseDistance = 50f; 		public float killDistance = 4f; 		   	    // Use this for initialization 	    void Start () { 		        myRenderer = GetComponent<Renderer> (); 		        myCharacterController = GetComponent<CharacterController> (); 				myCollider = GetComponent<SphereCollider> ();  		        currentWaypoint = Random.Range (0, waypoints.Length); 		        GameObject mainCamera = GameObject.FindGameObjectsWithTag ("MainCamera")[0]; 		        mainCharacter = mainCamera.transform; 		    }  	    void Update() { 			if (Input.GetButton ("Run")) { 				myCollider.radius = .1f; 			} else { 				myCollider.radius = .05f; 			} 		} 	     	    // Update is called once per frame 	    void FixedUpdate () { 		        Vector3 direction = mainCharacter.position - this.transform.position; 		        float angle = Vector3.Angle (direction, this.transform.forward); 		        direction.y = 0f; 		        //Maybe adding some gravity things 		//        if (myCharacterController.isGrounded) { 		//            direction.y = 0; 		//        } else { 		//            direction.y += gravity * Time.deltaTime; 		//        }   		        if (patrol && waypoints.Length > 0) { 			            //patrol 			            if (Vector3.Distance (waypoints [currentWaypoint].transform.position, transform.position) < accuracyWaypoint) { 				                //select random waypoint to patrol towards 				                currentWaypoint = Random.Range(0, waypoints.Length); 				            }  			            //rotate towards current waypoint 			            direction = waypoints[currentWaypoint].transform.position - this.transform.position; 			            direction.y = 0f;  			            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), patrolRotationSpeed); 			            myCharacterController.Move(this.transform.forward * Time.deltaTime * patrolSpeed); 			        }  		        float distance = Vector3.Distance (mainCharacter.position, this.transform.position); 		        // Noise Detection 		        if (!patrol && distance < chaseDistance) { 			            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed); 			            myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed); 		        } else if(distance < 4.0f){ 			            patrol = false; 			            myRenderer.material.color = Color.red; 		        } else if ( lineOfSight(angle)) { 			            //AI alerted, pursue main character 			            patrol = false; 			            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed); 			            myRenderer.material.color = Color.red; 			            myCharacterController.Move(this.transform.forward * Time.deltaTime * alertSpeed); 		        } else { 			        myRenderer.material.color = Color.blue; 			        patrol = true; 			    }  				if (distance <= killDistance) { 					 				} 			 		    }  		private bool lineOfSight(float angle){ 			RaycastHit hit; 			//Check Distance 			if (Vector3.Distance (mainCharacter.position, this.transform.position) < findDistance 				//Check angle 				&& (!patrol || angle < findAngle) 				//Check for barriers 				&& ( !patrol || (Physics.Linecast (transform.position, mainCharacter.position, out hit) && hit.transform.tag != "Terrain"))) { 				return true; 			} 			return false; 		}  	    void OnTriggerStay(Collider other) { 		        if (patrol) { 			            if (other.tag == "Player" || other.tag == "MainCamera") { 				                if (Input.GetButton ("Horizontal") || Input.GetButton ("Vertical") || Input.GetButton ("Jump")) { 					                    patrol = false; 					                    Vector3 direction = mainCharacter.position - this.transform.position; 					                    this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed); 					                    myRenderer.material.color = Color.red; 					                    myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed); 					                } 				            } 			//            // Now check to see if colliding with other snake 			//            // **** NOTE: I DON'T THINK THIS WORKS UNLESS SNAKES HAVE A RIGID BODY **** 			//            GameObject collidingObject = other.gameObject; 			//            SnakeAIController collidingSnake = collidingObject.GetComponent (typeof(SnakeAIController)) as SnakeAIController; 			//            if (collidingSnake != null) { 			//                //colliding with another Snake AI. Check if that snake is alert 			//                if (collidingSnake.isPatrolling ()) { 			//                    // do nothing 			//                } else { 			//                    // colliding with chasing snake, set this snake to chase 			//                    patrol = false; 			//                    Vector3 direction = mainCharacter.position - this.transform.position; 			//                    this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (direction), alertRotationSpeed); 			//                    myRenderer.material.color = Color.red; 			//                    myCharacterController.Move (this.transform.forward * Time.deltaTime * alertSpeed); 			//                } 			//            } 			        } 		    }  }  
