using UnityEngine;

namespace TechnOllieG
{
	public class CameraController : MonoBehaviour
	{
		[Header("FOV")]
		public float standardFov = 60f;
		public float sprintFov = 70f;
		public float fovLerpMultiplier = 10f;
		
		[Header("Camera Movement")]
		public bool useCameraMovement = true;
		public float cameraUpSpeed = 10f;
		public float accelerationSpeed = 40f;
		public float sprintingAccelerationSpeed = 60f;
		public float friction = 2f;
		
		[Header("Camera rotation")]
		public bool lockMouse = true;
		public float sensitivityScale = 100f;
		public float minTilt = -90f;
		public float maxTilt = 90f;
		
		[HideInInspector] public Vector3 accumulatedMouseDelta;

		private Transform _tf;
		private Vector3 _velocity;
		private bool _sprinting = false;
		private Camera _cam;
		private float _targetFov;
		
		private void Awake()
		{
			_tf = transform;
			_cam = GetComponent<Camera>();
		}

		private void Start()
		{
			if (lockMouse)
				Cursor.lockState = CursorLockMode.Locked;

			_cam.fieldOfView = standardFov;
			_targetFov = standardFov;
		}

		private void Update()
		{
			if (!useCameraMovement)
				return;
			
			float forwardInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
			float rightInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
			float upInput = (Input.GetKey(KeyCode.Space) ? 1 : 0) - (Input.GetKey(KeyCode.LeftShift) ? 1 : 0);

			bool sprintInput = Input.GetKey(KeyCode.LeftControl);

			if (forwardInput > 0.001f || forwardInput < -0.001f || rightInput > 0.001f || rightInput < -0.001f)
			{
				if (sprintInput)
				{
					_sprinting = true;
					_targetFov = sprintFov;
				}
			}
			else
			{
				_sprinting = false;
				_targetFov = standardFov;
			}

			_cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetFov, Time.deltaTime * fovLerpMultiplier);

			Vector3 forward = new Vector3(_tf.forward.x, 0f, _tf.forward.z).normalized;
			if (forward.IsNearlyZero())
			{
				forward = (_tf.forward.y < 0 ? _tf.up : -_tf.up);
				forward.y = 0;
				forward.Normalize();
			}
			
			Vector3 right = new Vector3(_tf.right.x, 0f, _tf.right.z).normalized;
			Vector3 acceleration = (forwardInput * forward + rightInput * right).normalized * (_sprinting ? sprintingAccelerationSpeed : accelerationSpeed);

			_velocity = _velocity += (acceleration - _velocity * friction) * Time.deltaTime;
			
			_tf.position += (_velocity + (upInput * cameraUpSpeed) * Vector3.up) * Time.deltaTime;
		}

		private void LateUpdate()
		{
			Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			accumulatedMouseDelta += mouseDelta * (Time.deltaTime * sensitivityScale);
		
			accumulatedMouseDelta.x %= 360f;
			accumulatedMouseDelta.y = Mathf.Clamp(accumulatedMouseDelta.y, minTilt, maxTilt);
			_tf.localRotation = Quaternion.AngleAxis(accumulatedMouseDelta.x, Vector3.up) * Quaternion.AngleAxis(-accumulatedMouseDelta.y, Vector3.right);
		}
	}
}