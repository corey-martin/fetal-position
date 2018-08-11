using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public GameObject continueButton;

	void Awake() {
		continueButton.SetActive(PlayerPrefs.HasKey("saveIndex"));
	}

	public void StartButton() {
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}

	public void ContinueButton() {
		SceneManager.LoadScene(PlayerPrefs.GetInt("saveIndex"), LoadSceneMode.Single);
	}

	public void QuitButton() {
		Application.Quit();
	}
}
