namespace Rewired.Demos
{
	using UnityEngine;
	using Rewired;

	// This is merely an example, it's for an example purpose only
	// Your game WILL require a custom controller scripts, there's just no generic character control systems,
	// they at least depend on the animations

	[RequireComponent (typeof(CharacterController))]
	public class ThidPersonExampleController : MonoBehaviour
	{
		internal string _Animation = null;

		//public GameObject TouchPad;
		public float MovementSpeed = 6.0f;
		public Player player;
		public int playerId = 0;

		private Animator ani;
		private Transform _mainCameraTransform;
		private Transform _transform;
		private CharacterController _characterController;
		private Vector3 inputVector;
		Vector3 movementVector;
		private bool jump;


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


		public void Start ()
		{
			ani = gameObject.GetComponent<Animator> ();
		}

		public void Update ()
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
			if (ani) {  
				//AnimatorStateInfo stateInfo = ani.GetCurrentAnimatorStateInfo (0);  
				//if (stateInfo.IsName ("Body Animation Layer.Walk")) {
				{
					ani.SetFloat ("Blend", para);  
				}
			} 
				
			jump = player.GetButtonDown ("Jump");
		}

		private void ProcessInput ()
		{
			//idle
			GameObject.Find ("AnimationManagerUI").GetComponent<AnimationManagerUI> ().SetAnimation_Idle ();

			//move
			if (inputVector.sqrMagnitude > 0.001f) {
				movementVector = _mainCameraTransform.TransformDirection (inputVector);
				movementVector.y = 0f;
				movementVector.Normalize ();
				_transform.forward = movementVector;
				GameObject.Find ("AnimationManagerUI").GetComponent<AnimationManagerUI> ().SetAnimation_Walk ();
			}

			//movementVector += Physics.gravity;

			//walk
			if (inputVector.sqrMagnitude < 0.16f) {
				_characterController.Move (movementVector * Time.deltaTime * MovementSpeed * 0.3f);
			}
			//run
			else if (inputVector.sqrMagnitude > 0.16f) {
				_characterController.Move (movementVector * Time.deltaTime * MovementSpeed);
			}

			//jump
			if (jump) {
				GameObject.Find ("AnimationManagerUI").GetComponent<AnimationManagerUI> ().SetAnimation_Jump ();
			}
		}
	}
}
