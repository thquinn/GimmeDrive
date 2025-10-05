using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Puzzles))]
public class PuzzlesEditor : Editor {
    private SerializedProperty puzzleStrings;
    private ReorderableList list;
    private GUIStyle textAreaStyle;

    private void OnEnable() {
        puzzleStrings = serializedObject.FindProperty("puzzleStrings");

        list = new ReorderableList(serializedObject, puzzleStrings, true, true, true, true);
        list.drawHeaderCallback = rect => {
            EditorGUI.LabelField(rect, "Puzzle Entries");
        };

        list.elementHeightCallback = index => {
            var element = puzzleStrings.GetArrayElementAtIndex(index);
            var content = element.FindPropertyRelative("content");
            // base height + estimated content height
            return Mathf.Max(110, EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing + 120);
        };

        list.drawElementCallback = (rect, index, active, focused) => {
            if (textAreaStyle == null) {
                textAreaStyle = new GUIStyle(EditorStyles.textArea) {
                    fontSize = 14,
                    wordWrap = true
                };
                try {
                    textAreaStyle.font = Font.CreateDynamicFontFromOSFont("Consolas", 14);
                } catch {
                    textAreaStyle.font = EditorStyles.textArea.font;
                }
            }

            var element = puzzleStrings.GetArrayElementAtIndex(index);
            var nameProp = element.FindPropertyRelative("name");
            var contentProp = element.FindPropertyRelative("content");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            var nameRect = new Rect(rect.x, rect.y + 2, rect.width, lineHeight);
            var contentRect = new Rect(rect.x, nameRect.yMax + spacing, rect.width, rect.height - lineHeight - spacing - 6);

            EditorGUI.PropertyField(nameRect, nameProp, new GUIContent($"Name {index}"));
            EditorGUI.LabelField(new Rect(rect.x, contentRect.y - 2, 80, lineHeight), "Puzzle Text");
            contentProp.stringValue = EditorGUI.TextArea(contentRect, contentProp.stringValue, textAreaStyle);
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
