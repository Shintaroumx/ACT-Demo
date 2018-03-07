using Rewired;
using UnityEngine;

namespace Examples.Scenes.TouchpadCamera
{
	public class RotateCamera : MonoBehaviour
	{
		public float RotationSpeed = 15f;
		public Transform cam_transform;
		public int playerId = 0;

		private Vector3 relCameraPos;
		private Player player;

		[System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
		private bool initialized;

		private void Initialize ()
		{
			// Get the Rewired Player object for this player.
			player = ReInput.players.GetPlayer (playerId);

			initialized = true;
		}


		public void Update ()
		{
			if (!ReInput.isReady)
				return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
			if (!initialized)
				Initialize (); // Reinitialize after a recompile in the editor

			//var horizontalMovement = player.GetAxis ("Horizontal2");
			//var verticalMovement = player.GetAxis ("Vertical2");

			//cam_transform.Rotate (Vector3.up, horizontalMovement * Time.deltaTime * RotationSpeed);
			//cam_transform.Rotate (Vector3.left, verticalMovement * Time.deltaTime * RotationSpeed);


			float cam_h = player.GetAxis ("Horizontal2");                                                            
			float cam_v = player.GetAxis ("Vertical2");  

			cam_transform.position = relCameraPos + transform.position;  
			  
			cam_transform.RotateAround (transform.position, Vector3.up, cam_h * RotationSpeed);  
			  
			float angleX = cam_transform.rotation.eulerAngles.x;  
			float nextAngleX = cam_v * RotationSpeed + angleX;  
			if (nextAngleX >= 360f) {  
				nextAngleX -= 360f;  
			}  
			if ((nextAngleX < 80f) || (nextAngleX <= 360f && nextAngleX >= 280f))
				cam_transform.RotateAround (transform.position, cam_transform.right, cam_v * RotationSpeed);  

			relCameraPos = cam_transform.position - transform.position;  

		}


	}
}