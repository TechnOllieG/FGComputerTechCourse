using UnityEngine;

namespace TechnOllieG
{
	public class CameraController : MonoBehaviour
	{
		public float standardFov = 90f;
		public float sprintFov = 100f;
		public float fovLerpMultiplier = 1f;
		public float cameraUpSpeed = 10f;
		public float accelerationSpeed = 1f;
		public float sprintingAccelerationSpeed;
		public bool lockMouse = true;
		public float sensitivityScale = 1f;
		public float minTilt = -90f;
		public float maxTilt = 90f;
		public float friction = 0.2f;

		private Transform _tf;
		private Vector3 _accumulatedMouseDelta;
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
			Vector3 right = new Vector3(_tf.right.x, 0f, _tf.right.z).normalized;
			Vector3 acceleration = (forwardInput * forward + rightInput * right).normalized * (_sprinting ? sprintingAccelerationSpeed : accelerationSpeed);

			_velocity = _velocity += (acceleration - _velocity * friction) * Time.deltaTime;
			
			_tf.position += (_velocity + (upInput * cameraUpSpeed) * Vector3.up) * Time.deltaTime;
		}

		private void LateUpdate()
		{
			Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			_accumulatedMouseDelta += mouseDelta * (Time.deltaTime * sensitivityScale);
		
			_accumulatedMouseDelta.x %= 360f;
			_accumulatedMouseDelta.y = Mathf.Clamp(_accumulatedMouseDelta.y, minTilt, maxTilt);
			_tf.localRotation = Quaternion.AngleAxis(_accumulatedMouseDelta.x, Vector3.up) * Quaternion.AngleAxis(-_accumulatedMouseDelta.y, Vector3.right);
		}
	}
}