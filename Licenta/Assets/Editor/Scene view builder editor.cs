using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneViewBuilder))]
public class Sceneviewbuildereditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SceneViewBuilder builder = (SceneViewBuilder)target;

        if (GUILayout.Button("Generate level")) {
            builder.GenerateNewLevel();
        }
    }
}

