using System;
using System.Numerics;
using TechnOllieG;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
	public float accelerationSpeed;
	public float friction;
	public float jumpImpulse;
	public float gravity;
	public int movementIterativeDepth = 3;
	
	public bool lockMouse = true;
	public float sensitivityScale = 1f;
	public float minTilt = -90f;
	public float maxTilt = 90f;

	private Vector3 _accumulatedMouseDelta;
	private Transform _cameraTransform;
	private Vector3 velocity;
	private Transform _tf;
	private CapsuleCollider _capsule;
	private Rigidbody _rb;

	private void Start()
	{
		if (lockMouse)
			Cursor.lockState = CursorLockMode.Locked;
		
		_cameraTransform = Camera.main.transform;
		_tf = transform;
		_capsule = GetComponent<CapsuleCollider>();
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		// float forwardInput = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);
		// float rightInput = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
		// bool jumpInput = Input.GetKey(KeyCode.Space);
		//
		// Vector3 acceleration = (forwardInput * _tf.forward + rightInput * _tf.right).normalized * accelerationSpeed;
		// velocity += (acceleration - velocity * friction) * Time.fixedDeltaTime;
		// velocity -=  _tf.up * (Time.fixedDeltaTime * gravity);
		//
		// if(jumpInput && IsGrounded())
		// {
		// 	velocity += _tf.up * jumpImpulse;
		// }
		//
		// _rb.velocity = velocity;
	}

	private void Update()
	{
		// Vector3 deltaToMove = velocity * Time.deltaTime;
		//
		// Vector3 pos = _tf.position;
		// Vector3 center = pos + _capsule.center;
		//
		// Vector3 p1 = center + Vector3.up * (_capsule.height * 0.5f) - _capsule.radius * Vector3.up;
		// Vector3 p2 = center - Vector3.up * (_capsule.height * 0.5f) + _capsule.radius * Vector3.up;
		//
		// for(int i = 0; i < movementIterativeDepth; i++)
		// {
		// 	float maxDistance = deltaToMove.magnitude;
		// 	bool blockingHit = Physics.CapsuleCast(p1, p2, _capsule.radius, deltaToMove.normalized, out RaycastHit hit, maxDistance);
		// 	float time = Mathf.InverseLerp(0f, maxDistance, hit.distance);
		//
		// 	Vector3 currentDelta = deltaToMove * (blockingHit ? time : 1f);
		// 	_tf.position += currentDelta;
		// 	deltaToMove -= currentDelta;
		// 	
		// 	if(blockingHit)
		// 	{
		// 		Vector3 depenetrationDelta = Vector3.Dot(hit.normal, velocity) * hit.normal;
		// 		velocity -= depenetrationDelta;
		// 	
		// 		if(deltaToMove.IsNearlyZero())
		// 			break;
		// 	
		// 		deltaToMove -= Vector3.Dot(deltaToMove, hit.normal) * hit.normal;
		// 	}
		// }
		//
		// Collider[] colliders = Physics.OverlapCapsule(p1, p2, _capsule.radius + 0.1f);
		//
		// if (colliders.Length > 1)
		// {
		// 	for (int i = 0; i < colliders.Length; i++)
		// 	{
		// 		if (ReferenceEquals(colliders[i], _capsule))
		// 			continue;
		//
		// 		Physics.ComputePenetration(_capsule, pos, _tf.rotation, colliders[i], colliders[i].transform.position,
		// 			colliders[i].transform.rotation, out Vector3 direction, out float distance);
		//
		// 		_tf.position += direction * distance;
		// 	}
		// }
	}

	private bool IsGrounded()
	{
		Vector3 pos = _tf.position;
		return Physics.Raycast(pos, Vector3.down, _capsule.height * 0.5f + 0.01f);
	}

	private void LateUpdate()
	{
		Vector3 mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		_accumulatedMouseDelta += mouseDelta * (Time.deltaTime * sensitivityScale);
		
		_accumulatedMouseDelta.x %= 360f;
		_accumulatedMouseDelta.y = Mathf.Clamp(_accumulatedMouseDelta.y, minTilt, maxTilt);
		_cameraTransform.localRotation = Quaternion.AngleAxis(_accumulatedMouseDelta.x, Vector3.up) * Quaternion.AngleAxis(-_accumulatedMouseDelta.y, Vector3.right);
		//transform.localRotation = ;
	}
}
