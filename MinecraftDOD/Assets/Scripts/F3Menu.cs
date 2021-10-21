using System;
using UnityEngine;
using UnityEngine.UI;

public class F3Menu : MonoBehaviour
{
	public GameObject f3Menu;
	public Text blockCoords;
	public Text chunkCoords;
	
	private bool _menuIsVisible = false;
	private Transform _cameraTf;
	
	private void Awake()
	{
		f3Menu.SetActive(_menuIsVisible);
	}

	private void Start()
	{
		_cameraTf = Camera.main.transform;
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F3))
		{
			_menuIsVisible = !_menuIsVisible;
			f3Menu.SetActive(_menuIsVisible);
		}

		if (_menuIsVisible)
		{
			Vector3 position = _cameraTf.position;
			Vector2Int chunkCoordinate = ConvertPositionToChunkCoordinate(position);
			
			blockCoords.text = $"Block: {Mathf.RoundToInt(position.x)} {Mathf.RoundToInt(position.y)} {Mathf.RoundToInt(position.z)}";
			chunkCoords.text = $"Chunk: {(int) position.x % 16} {(int) position.z % 16} in {chunkCoordinate.x} {chunkCoordinate.y}";
		}
	}
	
	Vector2Int ConvertPositionToChunkCoordinate(Vector3 position)
	{
		return new Vector2Int((int) Mathf.Floor(position.x / 16), (int) Mathf.Floor(position.z / 16));
	}
}
