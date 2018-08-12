using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour {

	public Button startButton;
	public Button continueButton;
	public Button backButton;
	public Button noButton;
	public GameObject menuMain;
	public GameObject menuCredits;
	public GameObject menuAreYouSure;

	void Awake() {
		//PlayerPrefs.DeleteKey("saveIndex");
		continueButton.gameObject.SetActive(PlayerPrefs.HasKey("saveIndex"));
		MainMenuButtonSelect();
	}

	void Update() {
		if (EventSystem.current.currentSelectedGameObject == null) {
			if (menuMain.activeSelf) {
				MainMenuButtonSelect();
			} else {
				BackButton();
			}
		}
	}

	void MainMenuButtonSelect() {
		if (continueButton.gameObject.activeSelf) {
			continueButton.Select();
			continueButton.OnSelect(null);
		} else {
			startButton.Select();
			startButton.OnSelect(null);
		}
	}

	public void StartButton() {
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	public void ContinueButton() {
		SceneManager.LoadScene(PlayerPrefs.GetInt("saveIndex"), LoadSceneMode.Single);
	}

	public void CreditsButton() {
		menuMain.SetActive(false);
		menuCredits.SetActive(true);
		backButton.Select();
		backButton.OnSelect(null);
	}

	public void BackButton() {
		menuCredits.SetActive(false);
		menuAreYouSure.SetActive(false);
		menuMain.SetActive(true);
		MainMenuButtonSelect();
	}

	public void AreYouSureButton() {
		if (continueButton.gameObject.activeSelf) {
			menuMain.SetActive(false);
			menuAreYouSure.SetActive(true);
			noButton.Select();
			noButton.OnSelect(null);
		} else {
			StartButton();
		}
	}

	public void QuitButton() {
		Application.Quit();
	}
}
