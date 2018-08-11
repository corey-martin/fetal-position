using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour {

	Game game;
	float moveSpeed = 8f;
	public static bool isMoving = false;
	Vector3 startPos;
	Vector3 endPos;

	Vector3 initialPos;
	List<Vector3> undoPositions = new List<Vector3>();
	List<Vector3> undoRotations = new List<Vector3>();
	List<bool> undoCrouchs = new List<bool>();
	public static bool isCrouching = false;
	public Transform modelParent;

	public static List<Moveable> objsToMove = new List<Moveable>();

	bool isFalling = false;

	void Start() {
		game = GameObject.Find("Main Camera").GetComponent<Game>();
		initialPos = transform.position;
		AddToUndoStack();
	}

	void Update() {
	//	Debug.Log("objsToMove.Count: " + objsToMove.Count);
	}

	public void AddToUndoStack() {
		undoPositions.Add(transform.position);
		if (modelParent != null) {
			undoRotations.Add(modelParent.eulerAngles);
		}
		if (gameObject.tag == "Player") {
			undoCrouchs.Add(isCrouching);
		}
	}

	public void DoUndo() {
		if (undoPositions.Count > 1) {
			undoPositions.RemoveAt(undoPositions.Count - 1);
			transform.position = undoPositions[undoPositions.Count - 1];
			if (modelParent != null) {
				undoRotations.RemoveAt(undoRotations.Count - 1);
				modelParent.eulerAngles = undoRotations[undoRotations.Count - 1];
			}
			if (gameObject.tag == "Player") {
				undoCrouchs.RemoveAt(undoCrouchs.Count - 1);
				isCrouching = undoCrouchs[undoCrouchs.Count - 1];
			}
		}
	}

	public void DoRestart() {
		transform.position = initialPos;
		AddToUndoStack();
	}

	public bool CanMove(Vector3 dir) {
		bool canMove = true;
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "Tile" && child.gameObject.activeSelf && canMove) {
				Vector3 posToCheck = child.position + dir;
				Collider[] hitColliders = Physics.OverlapSphere(posToCheck, 0.25f);
				foreach (Collider col in hitColliders) {
					if (col.tag == "Wall") {
						return false;
					} else if (col.tag == "Tile" && col.transform.parent.gameObject != this.gameObject) {
						if (col.transform.parent.GetComponent<Moveable>().CanMove(dir)) {
							// no problems yet
						} else {
							return false;
						}
					}
				}
			}
		}
		AddIt(dir);
		return true;
	}

	public void AddIt(Vector3 dir) {
		startPos = transform.position;
		endPos = startPos + dir;
		if (!objsToMove.Contains(this)) {
			objsToMove.Add(this);
		}
	}

	public void Carry(Vector3 dir) {
		// Carry stuff
		List<Moveable> newMoves = new List<Moveable>();

		foreach (Moveable m in objsToMove) {
			if (m.tag != "Player") {
				foreach (Transform child in m.transform) {
					Vector3 posToCheck = child.position + Vector3.back;
					Collider[] hitColliders = Physics.OverlapSphere(posToCheck, 0.25f);
					foreach (Collider col in hitColliders) {
						if (col.tag == "Tile" && col.transform.parent.gameObject != this.gameObject) {
							Moveable passenger = col.transform.parent.GetComponent<Moveable>();
							if (!objsToMove.Contains(passenger) && passenger.CanMove(dir)) {
								newMoves.Add(passenger);
							}
						}
					}
				}
			}
		}
		foreach (Moveable passenger in newMoves) {
			passenger.AddIt(dir);
		}
	}

	public IEnumerator MoveIt(Vector3 dir) {
		isMoving = true;

		Vector3 playerRotation = Vector3.zero;
		if (gameObject.tag == "Player") {
			playerRotation = modelParent.eulerAngles;
			modelParent.eulerAngles = playerRotation + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
		}

		if (objsToMove.Count > 1) {
			game.pullSound.pitch = Random.Range(.8f, 1f);
			game.pullSound.Play();
		}

		float t = 0;

		while (t < 1f) {
			t += Time.deltaTime * moveSpeed;
			foreach (Moveable m in objsToMove) {
				m.transform.position = Vector3.Lerp(m.startPos, m.endPos, t);
			}
			yield return null;
		}

		// fall stuff
		bool falling = true;
		foreach (Moveable m in game.moveables) {
			StartCoroutine(m.Fall());
		}
		while (falling) {
			yield return null;
			falling = false;
			foreach (Moveable m in game.moveables) {
				if (m.isFalling) {
					falling = true;
				}
			}
		}
		// end fall stuff

		if (gameObject.tag == "Player") {
			modelParent.eulerAngles = playerRotation;
		}

		transform.position = new Vector3 (Mathf.Round(endPos.x), Mathf.Round(endPos.y), Mathf.Round(endPos.z));

		game.AddToUndoStack();

		objsToMove.Clear();

		isMoving = false;
	}

	public IEnumerator Fall() {

		bool shouldFall = true;
		isFalling = true;

		while (shouldFall) {
			foreach (Transform child in transform) {
				if (child.gameObject.tag == "Tile") {
					if (child.position.z == 0) {
						shouldFall = false;
					}
					Vector3 posToCheck = child.position + Vector3.forward;
					Collider[] hitColliders = Physics.OverlapSphere(posToCheck, 0.25f);
					foreach (Collider col in hitColliders) {
						if (col.tag == "Tile" || col.tag == "Wall" || col.tag == "Player") {
							if (col.gameObject != this.gameObject) {
								shouldFall = false;
							}
						}
					}
				}
			}

			if (shouldFall) {
				startPos = transform.position;
				endPos = startPos + Vector3.forward;
				float t = 0;

				while (t < 1f) {
					t += Time.deltaTime * moveSpeed;
					transform.position = Vector3.Lerp(startPos, endPos, t);
					yield return null;
				}
			}
		}
		isFalling = false;
	}
}
