using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame
{
    /// <summary>
    /// This class can be used to play a set audio source.
    /// </summary>
    public class AudioRunner : MonoBehaviour
    {
        public AudioSource effect;
        //public AudioSource audio2;
        //public AudioSource audio3;

        /// <summary>
        /// Plays the passed AudioSource
        /// </summary>
        /// <param name="_audio">The AudioSource that will play.</param>
        public void PlayAudio(AudioSource _audio) => _audio.Play();

    }
}