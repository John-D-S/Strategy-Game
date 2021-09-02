using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame.Camera
{
	/// <summary>
	/// This class handles the Cinemachine FreeLook Camera that orbits the player.
	/// </summary>
	public class CameraControl : MonoBehaviour
	{
		[Header("Camera Variables")]
		public CinemachineFreeLook freeLookCam;
		[SerializeField] private float xSensitivity = 2;
		[SerializeField] private float ySensitivity = 0.05f;
    
		private bool look;

		// Update is called once per frame
		void Update()
		{
			// If Right mouse button is held down, look is true.
			look = Input.GetMouseButton(1);
			// If look is true, player can control the camera with the mouse.
			if(look)
			{
				freeLookCam.m_XAxis.Value += Input.GetAxisRaw("Mouse X") * xSensitivity;
				freeLookCam.m_YAxis.Value += Input.GetAxisRaw("Mouse Y") * ySensitivity;
			}
		}
		
		// todo - How to implement touch controls for mobile?? swiping the screen would work. 

	}
}