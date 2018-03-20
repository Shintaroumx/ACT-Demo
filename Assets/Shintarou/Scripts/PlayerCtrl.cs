namespace Rewired.Demos
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	using Rewired;

	[RequireComponent (typeof(CharacterController))]
	public class PlayerCtrl : MonoBehaviour
	{
		public Player player;
		public int playerId;
		public float MovementSpeed = 6.0f;
		public WeaponTrail Katana;

		private Transform _mainCameraTransform;
		private Transform _transform;
		private CharacterController _characterController;
		private Vector3 inputVector;
		private Vector3 movementVector;
		private Animator animator;

		private bool moveTrigger = true;
		private bool jumpTrigger = false;
		private bool jump;
		private bool circleAtk;
		private bool circleAtkLong;
		private bool triangleAtk;
		private bool dash;


		protected AnimationController animationController;


		[System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
		private bool initialized;

		private void Initialize ()
		{
			// Get the Rewired Player object for this player.
			player = ReInput.players.GetPlayer (playerId);

			initialized = true;
		}

		private void OnEnable ()
		{
			_mainCameraTransform = Camera.main.GetComponent<Transform> ();
			_characterController = GetComponent<CharacterController> ();
			_transform = GetComponent<Transform> ();
		}



		void Awake ()
		{
			animationController = GetComponent<AnimationController> ();
		}


		// Use this for initialization
		void Start ()
		{
			animator = gameObject.GetComponent<Animator> ();
			animationController.AddTrail (Katana);
			Katana.ClearTrail ();

		}
	
		// Update is called once per frame
		void Update ()
		{
			if (!ReInput.isReady)
				return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
			if (!initialized)
				Initialize (); // Reinitialize after a recompile in the editor

			GetInput ();
			ProcessInput ();
		}


		private void GetInput ()
		{
			inputVector = new Vector3 (player.GetAxis ("Horizontal"), player.GetAxis ("Vertical"));
			movementVector = Vector3.zero;

			//movetree
			float para = inputVector.magnitude;
			if (animator) {
				animator.SetFloat ("Blend", para);  
			} 
				
			jump = player.GetButtonDown ("Jump");//cross
			circleAtk = player.GetButtonTimedPressUp ("CircleAttack", 0f, 0.4f);//circle
			circleAtkLong = player.GetButtonTimedPressDown ("CircleAttackLong", 0.4f);//long circle
			triangleAtk = player.GetButtonDown ("TriangleAttack");
			dash = player.GetButtonDown ("Dash");

		}

		private void ProcessInput ()
		{
			//idle
			GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,1");

			//move
			if (moveTrigger) {
				if (inputVector.sqrMagnitude > 0.001f) {
					movementVector = _mainCameraTransform.TransformDirection (inputVector);
					movementVector.y = 0f;
					movementVector.Normalize ();
					_transform.forward = movementVector;
					GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,2");
				}	
				//walk
				if (inputVector.sqrMagnitude < 0.16f) {
					_characterController.Move (movementVector * Time.deltaTime * MovementSpeed * 0.2f);
					Dash (0.2f);
				}
			//run
			else if (inputVector.sqrMagnitude > 0.305f) {
					_characterController.Move (movementVector * Time.deltaTime * MovementSpeed);
					Dash (1.0f);
				}
			//lerp within walk and run
			else if (inputVector.sqrMagnitude < 0.305f && inputVector.sqrMagnitude > 0.16f) {
					Vector3.Normalize (inputVector);
					_characterController.Move (movementVector * Time.deltaTime * MovementSpeed * Mathf.Lerp (0.2f, 1.0f, inputVector.sqrMagnitude));
				}
			}

			//jump
			if (jump) {
				jumpTrigger = true;
				GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,3");
				StartCoroutine (DelaytoInvoke (() => {
					jumpTrigger = false;
				}, 1.4f));
			}

			//circle attack
			if (circleAtk) {
				GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,5");
				if (!jumpTrigger) {
					Katana.SetTime (0.7f, 0, 1);
				}
				//Katana.StartTrail (0.15f, 0.1f);
			}

			//circle attack long
			if (circleAtkLong) {
				GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,4");
				moveTrigger = false;
				Katana.SetTime (1.0f, 0, 1);
				StartCoroutine (DelaytoInvoke (() => {
					moveTrigger = true;
				}, 1.4f));
			}

			//triangle attack
			if (triangleAtk) {
				GameObject.Find ("AnimatorController").GetComponent<AnimatorController> ().SetInt ("animation,6");
				Katana.SetTime (1.0f, 0, 1);
			}

		}


		//use coroutine to delay invoking some functions
		IEnumerator DelaytoInvoke (Action action, float delaySeconds)
		{
			yield return new WaitForSeconds (delaySeconds);
			action ();
		}


		//R1 dash
		private void Dash (float normalSpeed)
		{
			if (dash) {
				_characterController.Move (movementVector * Time.deltaTime * MovementSpeed * 5.0f);
				gameObject.GetComponent<GhostShadow> ().enabled = true;
				StartCoroutine (DelaytoInvoke (() => {
					_characterController.Move (movementVector * Time.deltaTime * MovementSpeed * normalSpeed);
				}, 1.0f));
				StartCoroutine (DelaytoInvoke (() => {
					gameObject.GetComponent<GhostShadow> ().enabled = false;
				}, 1.0f));
			}
		}
	}
}