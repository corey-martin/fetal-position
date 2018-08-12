using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Moveable {

	bool isPulling = false;

	Vector3 facing = Vector3.down;
	Vector3 direction = Vector3.right;

	public GameObject standingTile;

	public GameObject modelStand;
	public GameObject modelStandPull;
	public GameObject modelCrouch;
	public GameObject modelCrouchPull;

	public GameObject winText;

	AudioSource audioSource;
	public AudioClip moveSound;
	public AudioClip crouchSound;

	void Awake() {
		winText.SetActive(false);
		audioSource = GetComponent<AudioSource>();
	}
	
	void Update () {

		if (!isMoving && !Game.freeze) {

			// WIN?
			bool surrounded = true;

			List<Vector3> positionsToCheck = new List<Vector3>();
			positionsToCheck.Add(transform.position + Vector3.back);
			positionsToCheck.Add(transform.position + Vector3.up);
			positionsToCheck.Add(transform.position + Vector3.down);
			positionsToCheck.Add(transform.position + Vector3.left);
			positionsToCheck.Add(transform.position + Vector3.right);

			int i = 0;

			while (surrounded && i < positionsToCheck.Count) {
				surrounded = false;
				Collider[] colls = Physics.OverlapSphere(positionsToCheck[i], 0.25f);
				foreach (Collider col in colls) {
					if (col.tag == "Tile" || col.tag == "Wall") {
						surrounded = true;
					}
				}
				i++;
			}
			if (surrounded) {
				Game.freeze = true;
				facing = Vector3.down;
				modelParent.eulerAngles = new Vector3 (0,0,0); 
				isCrouching = true;
				isPulling = false;
				winText.SetActive(true);
				StartCoroutine(Game.NextLevel());
			} else {
				// MOVEMENT

				// crouching
				if (Input.GetButtonDown("Crouch")) {
					Crouch();
				}

				isPulling = Input.GetButton("Pull");
				float hor = Input.GetAxisRaw("Horizontal");
				float ver = Input.GetAxisRaw("Vertical");

				if (hor == 1) {
					direction = Vector3.right;
				} else if (hor == -1) { 
					direction = Vector3.left;
				} else if (ver == -1) {
					direction = Vector3.down;
				} else if (ver == 1) {
					direction = Vector3.up;
				} else {
					direction = Vector3.zero;
				}

				if (direction != Vector3.zero) {
					if (isPulling && facing == -direction) {
						Vector3 posToCheck = transform.position - direction;
						if (!isCrouching) {
							posToCheck += Vector3.back;
						}
						Collider[] hitColliders = Physics.OverlapSphere(posToCheck, 0.25f);
						foreach (Collider col in hitColliders) {
							if (col.tag == "Tile") {
								Moveable m = col.transform.parent.GetComponent<Moveable>();
								if (!objsToMove.Contains(m) && m.CanMove(direction)) {
									m.AddIt(direction);
								}
							}
						}
					}
					if (CanMove(direction)) {
						//Carry(direction);
						audioSource.pitch = Random.Range(.8f, 1f);
						audioSource.clip = moveSound;
						audioSource.Play();

						if (!isPulling) {
							if (hor == 1) {
								facing = Vector3.right;
								modelParent.eulerAngles = new Vector3 (0,0,90); 
							} else if (hor == -1) {
								facing = Vector3.left;
								modelParent.eulerAngles = new Vector3 (0,0,-90); 
							} else if (ver == -1) {
								facing = Vector3.down;
								modelParent.eulerAngles = new Vector3 (0,0,0); 
							} else if (ver == 1) {
								facing = Vector3.up;
								modelParent.eulerAngles = new Vector3 (0,0,180); 
							}
						}

						StartCoroutine(MoveIt(direction));
					} else if (objsToMove.Count > 0 && !isCrouching) {
						if (objsToMove[0].transform.position.z < -0.5f) {
							StartCoroutine(MoveIt(direction));
							Crouch();
						} else {
							objsToMove.Clear();
						}
					} else {
						objsToMove.Clear();
					}
				}
			}
		}

		// GRAPHICS

		modelStand.SetActive(false);
		modelStandPull.SetActive(false);
		modelCrouch.SetActive(false);
		modelCrouchPull.SetActive(false);

		if (isCrouching) {
			standingTile.SetActive(false);
			if (isPulling) {
				modelCrouchPull.SetActive(true);
			} else {
				modelCrouch.SetActive(true);
			}
		} else {
			standingTile.SetActive(true);
			if (isPulling) {
				modelStandPull.SetActive(true);
			} else {
				modelStand.SetActive(true);
			}
		}
	}

	void Crouch() {
		bool canStand = true;

		if (isCrouching) {
			Vector3 posToCheck = transform.position + Vector3.back;
			Collider[] hitColliders = Physics.OverlapSphere(posToCheck, 0.25f);
			foreach (Collider col in hitColliders) {
				if (col.tag == "Tile") {
					canStand = false;
				}
			}
		}

		if (canStand) {
			isCrouching = !isCrouching;
			audioSource.pitch = Random.Range(1f, 1.4f);
			audioSource.clip = crouchSound;
			audioSource.Play();
		}
	}
}
