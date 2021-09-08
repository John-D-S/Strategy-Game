using BasicMenuFramework.Core;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.AnimatedValues;

using UnityEngine;

namespace BasicMenuFramework.Editor.Core
{
    [CustomEditor(typeof(MenuControl))]
    public class MenuControlEditor : UnityEditor.Editor
    {
        private MenuControl menuControl;

        private SerializedProperty quitButtonProperty;
        private SerializedProperty pauseButtonProperty;
        private SerializedProperty sceneLoadButtonsProperty;
        private SerializedProperty showPopUpProperty;
        private SerializedProperty popUpProperty;

        private AnimBool showPopUp = new AnimBool();

        private void OnEnable()
        {
            menuControl = target as MenuControl;

            quitButtonProperty = serializedObject.FindProperty("quitButton");
            pauseButtonProperty = serializedObject.FindProperty("pauseButton");
            sceneLoadButtonsProperty = serializedObject.FindProperty("sceneLoadButtons");
            showPopUpProperty = serializedObject.FindProperty("showPopUp");
            popUpProperty = serializedObject.FindProperty("popUp");

            showPopUp.value = showPopUpProperty.boolValue;
            showPopUp.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(quitButtonProperty);
                EditorGUILayout.PropertyField(pauseButtonProperty);
                    EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(showPopUpProperty);

                        showPopUp.target = showPopUpProperty.boolValue;
                        if(EditorGUILayout.BeginFadeGroup(showPopUp.faded))
                        {
                            EditorGUI.indentLevel++;
                            {
                                EditorGUILayout.PropertyField(popUpProperty);
                            }
                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndFadeGroup();
                    EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(sceneLoadButtonsProperty);
            }
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}