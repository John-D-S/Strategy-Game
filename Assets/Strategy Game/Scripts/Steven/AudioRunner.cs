using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace StrategyGame
{
    /// <summary>
    /// This class can be used to play a set audio source.
    /// </summary>
    public class AudioRunner : MonoBehaviour
    {
        [SerializeField] private float pauseVolume = -50f;
        [SerializeField] private float originalVolume = 0;
        
        public AudioSource effect;
        //public AudioSource effect2;
        //public AudioSource effect3;

        public AudioMixer musicGroup;
        
        /// <summary>
        /// Use this to pause the other Audio in the Scene on the Music Group.
        /// </summary>
        public void PauseAudio()
        {
            musicGroup.GetFloat("MusicVolume", out originalVolume);
            musicGroup.SetFloat("MusicVolume", pauseVolume);
        }

    }
}