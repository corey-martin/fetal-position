#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelMaker))]
public class LevelMakerEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

EditorGUILayout.HelpBox(@"LEGEND
-----------------------------
B = Void wall
X = Ground
O = Block", MessageType.Info);

		LevelMaker lvlMkr = (LevelMaker)target;

		if(GUILayout.Button("Load Scene")) {
			lvlMkr.LoadIn();
		}
	}
}

#endif