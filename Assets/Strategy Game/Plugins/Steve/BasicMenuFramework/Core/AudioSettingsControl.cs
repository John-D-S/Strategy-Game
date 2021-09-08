using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace BasicMenuFramework.Core
{
    /// <summary>
    /// This class handles Audio UI controls interactions with the Audio Mixer for the scene, saving set values using PlayerPrefs.
    /// </summary>
    public class AudioSettingsControl : MonoBehaviour
    {
        [Header("Audio Mixer"),Tooltip("The Audio Mixer the assigned volume sliders will correspond too.")]
        [SerializeField] private AudioMixer audioMixer;
        
        [Header("Exposed Parameter Names for Mixer Groups"),Tooltip("Name of the exposed Master Volume parameter in the Audio Mixer")]
        [SerializeField] private string masterGroupExposedParam;
        [Tooltip("Name of the exposed Music Volume parameter in the Audio Mixer")]
        [SerializeField] private string musicGroupExposedParam;
        [Tooltip("Name of the exposed SFX Volume parameter in the Audio Mixer")]
        [SerializeField] private string sfxGroupExposedParam;

        [Header("Volume Sliders"), Tooltip("The Master Volume slider in the scene")]
        [SerializeField] private Slider masterVolumeSlider;
        [Tooltip("The Music Volume slider in the scene")]
        [SerializeField] private Slider musicVolumeSlider;
        [Tooltip("The SFX Volume slider in the scene")]
        [SerializeField] private Slider sfxVolumeSlider;
    
        // Start is called before the first frame update
        void Start()
        {
            CheckVariables();
            
            LoadVolumePlayerPrefs();

            masterVolumeSlider.onValueChanged.AddListener(MasterSlider);
            musicVolumeSlider.onValueChanged.AddListener(MusicSlider);
            sfxVolumeSlider.onValueChanged.AddListener(SFXSlider);
        }

        /// <summary>
        /// Null Checks and throws Exceptions for all variables within the class.
        /// </summary>
        /// <exception cref="UnassignedReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void CheckVariables()
        {
            if(audioMixer == null)
                throw new UnassignedReferenceException($"No Audio Mixer has been assigned.");
            if(string.IsNullOrWhiteSpace(masterGroupExposedParam))
                throw new ArgumentException($"No parameter name has been assigned for the Audio Mixer Master Group");
            if(string.IsNullOrWhiteSpace(musicGroupExposedParam))
                throw new ArgumentException($"No parameter name has been assigned for the Audio Mixer Music Group");
            if(string.IsNullOrWhiteSpace(sfxGroupExposedParam))
                throw new ArgumentException($"No parameter name has been assigned for the Audio Mixer SFX Group");
            if(masterVolumeSlider == null)
                throw new UnassignedReferenceException($"No Slider has been assigned for the Master Volume Slider");
            if(musicVolumeSlider == null)
                throw new UnassignedReferenceException($"No Slider has been assigned for the Music Volume Slider");
            if(sfxVolumeSlider == null)
                throw new UnassignedReferenceException($"No Slider has been assigned for the SFX Volume Slider");
        }

    
    #region Volume Sliders
    
        /// <summary>
        /// Loads all the saved values for settings from playerprefs.
        /// </summary>
        public void LoadVolumePlayerPrefs()
        {
            if(PlayerPrefs.HasKey("MusicVolume"))
            {
                float volume = PlayerPrefs.GetFloat("MusicVolume");
                musicVolumeSlider.value = volume;
                MusicSlider(volume);
            }

            if(PlayerPrefs.HasKey("SFXVolume"))
            {
                float volume = PlayerPrefs.GetFloat("SFXVolume");
                sfxVolumeSlider.value = volume;
                SFXSlider(volume);
            }
        
            if(PlayerPrefs.HasKey("MasterVolume"))
            {
                float volume = PlayerPrefs.GetFloat("MasterVolume");
                masterVolumeSlider.value = volume;
                MasterSlider(volume);
            }
        }
    
     
    
        /// <summary>
        /// Function for the Master Volume slider. Saves value to PlayerPrefs.
        /// </summary>
        /// <param name="_volume">Float passed in from the slider</param>
        public void MasterSlider(float _volume)
        {
            PlayerPrefs.SetFloat("MasterVolume", _volume);
            _volume = VolumeRemap(_volume);
            audioMixer.SetFloat(masterGroupExposedParam, _volume);        
        }
    
        /// <summary>
        /// Function for the Master Volume slider. Saves value to PlayerPrefs.
        /// </summary>
        /// <param name="_volume">Float passed in from the slider</param>
        public void MusicSlider(float _volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", _volume);
            _volume = VolumeRemap(_volume);
            audioMixer.SetFloat(musicGroupExposedParam, _volume);        
        }

        /// <summary>
        /// Function for the SFX Volume slider. Saves value to PlayerPrefs.
        /// </summary>
        /// <param name="_volume">Float passed in from the slider</param>
        public void SFXSlider(float _volume)
        {
            PlayerPrefs.SetFloat("SFXVolume", _volume);
            _volume = VolumeRemap(_volume);
            audioMixer.SetFloat(sfxGroupExposedParam, _volume);
        }

        /// <summary>
        /// Remaps the passed in float from the slider(value 0-1) and outputs to audio scale
        /// </summary>
        /// <param name="value">Passed in slider value</param>
        /// <returns>Remapped float value to pass into the audio mixer</returns>
        public float VolumeRemap(float value)
        {
            return -40 + (value - 0) * (20 - -40) / (1 - 0);
        }
    #endregion
    
    }
}