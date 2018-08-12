using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	[HideInInspector] public Moveable[] moveables;

	public Player player;

	public static bool freeze = false;
	public AudioSource pullSound;
	public AudioSource undoSound;

	public static int sceneIndex;

	void Start() {
		if (player == null) {
			player = transform.root.GetComponent<Player>();
		}
		moveables = FindObjectsOfType<Moveable>();

		sceneIndex = SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.SetInt("saveIndex", sceneIndex);
	}

	void Update() {
		if (!Moveable.isMoving && !freeze) {
			if (Input.GetButtonDown("Undo")) {
				undoSound.pitch = Random.Range(.9f, 1f);
				undoSound.Play();
				foreach (Moveable m in moveables) {
					m.DoUndo();
				}
				player.UpdateFacing();
			}

			if (Input.GetButtonDown("Restart")) {
				undoSound.pitch = Random.Range(.5f, .5f);
				undoSound.Play();
				foreach (Moveable m in moveables) {
					m.DoRestart();
				}
				player.UpdateFacing();
			}
		}

		if (Input.GetKeyDown("escape")) {
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}

	public void AddToUndoStack() {
		foreach (Moveable m in moveables) {
			m.AddToUndoStack();
		}
	}

	public static IEnumerator NextLevel() {
		yield return new WaitForSeconds(2);
		freeze = false;
		if (sceneIndex < (SceneManager.sceneCountInBuildSettings - 1)) {
			SceneManager.LoadScene(sceneIndex + 1, LoadSceneMode.Single);
		} else {
			PlayerPrefs.DeleteKey("saveIndex");
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}
}
