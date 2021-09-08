using BasicMenuFramework.Core;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace BasicMenuFramework.Editor.Core
{
    [CustomEditor(typeof(SettingsControl))]
    public class SettingsControlEditor : UnityEditor.Editor
    {
        private SettingsControl settingsControl;

        private SerializedProperty resolutionDropdownProperty;
        private SerializedProperty qualityDropdownProperty;
        private SerializedProperty fullscreenToggleProperty;

        private void OnEnable()
        {
            settingsControl = target as SettingsControl;

            resolutionDropdownProperty = serializedObject.FindProperty("resolutionDropdown");
            qualityDropdownProperty = serializedObject.FindProperty("qualityDropdown");
            fullscreenToggleProperty = serializedObject.FindProperty("fullscreenToggle");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(resolutionDropdownProperty);
                EditorGUILayout.PropertyField(qualityDropdownProperty);
                EditorGUILayout.PropertyField(fullscreenToggleProperty);
            }
            EditorGUILayout.EndVertical();
            
            
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}