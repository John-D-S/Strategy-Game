using BasicMenuFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor;

using UnityEngine;

namespace BasicMenuFramework.Editor.Core
{
    [CustomEditor(typeof(AudioSettingsControl))]
    public class AudioSettingsControlEditor : UnityEditor.Editor
    {
        private AudioSettingsControl audioSettingsControl;

        private SerializedProperty audioMixerProperty;
        private SerializedProperty masterProperty;
        private SerializedProperty musicProperty;
        private SerializedProperty sfxProperty;
        private SerializedProperty masterVolumeSliderProperty;
        private SerializedProperty musicVolumeSliderProperty;
        private SerializedProperty sfxVolumeSliderProperty;

        private void OnEnable()
        {
            audioSettingsControl = target as AudioSettingsControl;
            
            audioMixerProperty = serializedObject.FindProperty("audioMixer");
            masterProperty = serializedObject.FindProperty("masterGroupExposedParam");
            musicProperty = serializedObject.FindProperty("musicGroupExposedParam");
            sfxProperty = serializedObject.FindProperty("sfxGroupExposedParam");
            masterVolumeSliderProperty = serializedObject.FindProperty("masterVolumeSlider");
            musicVolumeSliderProperty = serializedObject.FindProperty("musicVolumeSlider");
            sfxVolumeSliderProperty = serializedObject.FindProperty("sfxVolumeSlider");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                
                EditorGUILayout.PropertyField(audioMixerProperty);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(masterProperty);
                EditorGUILayout.PropertyField(musicProperty);
                EditorGUILayout.PropertyField(sfxProperty);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(masterVolumeSliderProperty);
                EditorGUILayout.PropertyField(musicVolumeSliderProperty);
                EditorGUILayout.PropertyField(sfxVolumeSliderProperty);
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}