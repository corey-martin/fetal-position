#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaker : MonoBehaviour {

	public GameObject groundPrefab;
	public GameObject blockPrefab;
	public GameObject voidWallPrefab;

    [TextArea(12,12)]
    public string layout;

	public void LoadIn() {
		foreach (Transform child in transform) {
			foreach (Transform gchild in child) {
				UnityEditor.EditorApplication.delayCall+=()=> {
            		DestroyImmediate(gchild.gameObject, true);
        		};
			}
		}

		string[] characters = new string[layout.Length];
		for (int i = 0; i < layout.Length; i++) {
			characters[i] = layout[i].ToString();
		}

		int x = 0;
		int y = 0;
		foreach (string c in characters) {
			if (c == "B") {
				InstantiateOne(voidWallPrefab, "VoidWalls", x, y);
			} else if (c == "X") {
				InstantiateOne(groundPrefab, "Grounds", x, y);
			} else if (c == "O") {
				InstantiateOne(blockPrefab, "Blocks", x, y);
			}

			if (c == "\n") {
				y++;
				x = 0;
			} else {
				x++;
			}
		}

		//
	}

	void InstantiateOne(GameObject prefab, string parent, int _x, int _y) {
		GameObject go = Instantiate (prefab, _x, -_y);
		go.transform.parent = GameObject.Find(parent).transform;
	}

	public GameObject Instantiate(GameObject gObject, int x, int y) {
		return GameObject.Instantiate (gObject, new Vector3(x, y), Quaternion.identity) as GameObject;
	}
}

#endif