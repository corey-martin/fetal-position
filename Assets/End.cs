using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class End : MonoBehaviour {

	//AudioSource audioSource;
	public Button menuButton;

	void Awake() {
	//	audioSource = GetComponent<AudioSource>();
	}

	void Update() {
		if (EventSystem.current.currentSelectedGameObject == null) {
			menuButton.Select();
			menuButton.OnSelect(null);
		}
	}

	public void LoadMenu() {
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
