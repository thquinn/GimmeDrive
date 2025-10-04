using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Puzzles))]
public class PuzzlesEditor : Editor {
    private SerializedProperty puzzleStrings;
    private GUIStyle textAreaStyle;

    private void OnEnable() {
        puzzleStrings = serializedObject.FindProperty("puzzleStrings");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        // Lazily set up the style
        if (textAreaStyle == null) {
            textAreaStyle = new GUIStyle(EditorStyles.textArea) {
                fontSize = 14,
                wordWrap = true
            };

            try {
                textAreaStyle.font = Font.CreateDynamicFontFromOSFont("Consolas", 14);
            } catch {
                // fallback: just use whatever EditorStyles.textArea uses
                textAreaStyle.font = EditorStyles.textArea.font;
            }
        }

        EditorGUILayout.LabelField("Puzzle Strings", EditorStyles.boldLabel);

        // Add button
        if (GUILayout.Button("+", GUILayout.Width(30))) {
            puzzleStrings.InsertArrayElementAtIndex(puzzleStrings.arraySize);
            var newElement = puzzleStrings.GetArrayElementAtIndex(puzzleStrings.arraySize - 1);
            newElement.stringValue = string.Empty;
        }

        EditorGUILayout.Space();

        for (int i = 0; i < puzzleStrings.arraySize; i++) {
            EditorGUILayout.BeginHorizontal();

            // Index label
            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(25));

            // Text area
            var element = puzzleStrings.GetArrayElementAtIndex(i);
            element.stringValue = EditorGUILayout.TextArea(element.stringValue, textAreaStyle, GUILayout.MinHeight(100));

            // Minus button
            if (GUILayout.Button("-", GUILayout.Width(25))) {
                puzzleStrings.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
