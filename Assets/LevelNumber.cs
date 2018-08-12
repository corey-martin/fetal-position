using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelNumber : MonoBehaviour {

	void Start () {
		Text levelNumText = GetComponent<Text>();
		levelNumText.text = "LEVEL " + SceneManager.GetActiveScene().buildIndex;
	}
}
