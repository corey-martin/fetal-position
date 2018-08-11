﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	[HideInInspector] public Moveable[] moveables;

	public static bool freeze = false;
	public AudioSource pullSound;

	public static int sceneIndex;

	void Start() {
		moveables = FindObjectsOfType<Moveable>();

		sceneIndex = SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.SetInt("saveIndex", sceneIndex);
	}

	void Update() {
		if (!Moveable.isMoving && !freeze) {
			if (Input.GetButtonDown("Undo")) {
				foreach (Moveable m in moveables) {
					m.DoUndo();
				}
			}

			if (Input.GetButtonDown("Restart")) {
				foreach (Moveable m in moveables) {
					m.DoRestart();
				}
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
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}
	}
}
