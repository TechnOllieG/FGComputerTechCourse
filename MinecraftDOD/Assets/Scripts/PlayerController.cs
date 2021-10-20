using TechnOllieG;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float accelerationSpeed;
	public float friction;
	public float jumpImpulse;
	public float gravity;
	
	public bool lockMouse = true;
	public float sensitivityScale = 1f;
	public float minTilt = -90f;
	public float maxTilt = 90f;

	private Vector3 _accumulatedMouseDelta;
	private Transform _cameraTransform;
	private Vector3 velocity;
	private Transform _tf;

	private void Start()
	{
		if (lockMouse)
			Cursor.lockState = CursorLockMode.Locked;
		
		_cameraTransform = Camera.main.transform;
		_tf = transform;
	}

	private void Update()
	{
		float forwardInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
		float rightInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
		bool jumpInput = Input.GetKey(KeyCode.Space);
		
		Vector3 acceleration = (forwardInput * _tf.forward + rightInput * _tf.right).normalized * accelerationSpeed;
		velocity += (acceleration - velocity * friction) * Time.deltaTime;
		velocity -=  _tf.up * (Time.deltaTime * gravity);

		// todo finish porting unreal to unity
		if(jumpInput && IsGrounded())
		{
			velocity += _tf.up * jumpImpulse;
		}

		Vector3 deltaToMove = velocity * Time.deltaTime;
		
		Physics.CapsuleCast()
	
		FHitResult Hit;

		for(int i = 0; i < MovementIterativeDepth; i++)
		{
			// todo sweep
			transform.position += deltaToMove;

			deltaToMove -= deltaToMove * Hit.Time;
		
			if(Hit.bBlockingHit)
			{
				Vector3 DeprenetationDelta = Vector3.Dot(Hit.Normal, velocity) * Hit.Normal;
				velocity -= DeprenetationDelta;
			
				if(deltaToMove.IsNearlyZero())
					break;
			
				deltaToMove -= Vector3.Dot(deltaToMove, Hit.Normal) * Hit.Normal;
			}
		}
	}
	
	

	private void LateUpdate()
	{
		Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		_accumulatedMouseDelta += mouseDelta * (Time.deltaTime * sensitivityScale);
		
		_accumulatedMouseDelta.x %= 360f;
		_accumulatedMouseDelta.y = Mathf.Clamp(_accumulatedMouseDelta.y, minTilt, maxTilt);
		_cameraTransform.localRotation = Quaternion.AngleAxis(-_accumulatedMouseDelta.y, Vector3.right);
		transform.localRotation = Quaternion.AngleAxis(_accumulatedMouseDelta.x, Vector3.up);
	}
}
