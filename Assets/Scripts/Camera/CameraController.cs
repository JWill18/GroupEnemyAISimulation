using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

	/// <summary>
	/// The angle that the camera will be at.
	/// </summary>
	public Quaternion CameraRotation;

	private Camera mainCamera = null;

	private void Awake()
	{
		// If this object has the camera script already on it.
		if (gameObject.GetComponent<Camera>() != null)
		{
			// Find the main camera
			if (gameObject.GetComponent<Camera>().tag == "MainCamera")
			{
				mainCamera = gameObject.GetComponent<Camera>();
			}
			else
			{
				var cameraList = FindObjectsOfType<Camera>();

				foreach (var camera in cameraList)
				{
					if (camera.tag == "MainCamera")
					{
						mainCamera = camera;
					}
				}
			}

			// If main camera is not found then this is the main camera. Else remove self
			if (mainCamera == null)
			{
				mainCamera = gameObject.GetComponent<Camera>();
				mainCamera.tag = "MainCamera";
			}
			else
			{
				if (mainCamera.gameObject.GetComponent<CameraController>() == null)
				{
					var tempCamera = mainCamera.gameObject.AddComponent<CameraController>();
					tempCamera.CameraRotation = CameraRotation;
					print("There is already a main camera in the level. There can only be one main camera at a time.");
					Destroy(this);
				}
			}

			GameObject cameraContainer;
			CameraController cameraMovement;

			// Find or create empty parent object.
			if (transform.parent == null)
			{
				cameraContainer = new GameObject("CameraContainer");
				transform.parent = cameraContainer.transform;
			}
			else
			{
				cameraContainer = transform.parent.gameObject;
			}

			// Find or Add camera movement script to the parent object.
			if (cameraContainer.GetComponent<CameraController>() == null)
			{
				cameraMovement = cameraContainer.AddComponent<CameraController>();
			}
			else
			{
				cameraMovement = cameraContainer.GetComponent<CameraController>();
			}

			cameraMovement.CameraRotation = CameraRotation;
			DestroyImmediate(this);
		}
		else
		{
			if (gameObject.GetComponentInChildren<Camera>() == null)
			{
				var cameraList = FindObjectsOfType<Camera>();

				foreach (var camera in cameraList)
				{
					if (camera.tag == "MainCamera")
					{
						mainCamera = camera;
					}
				}

				if (mainCamera == null)
				{
					mainCamera = new Camera
					{
						tag = "MainCamera"
					};
				}

				mainCamera.transform.parent = gameObject.transform;
				mainCamera.transform.position = gameObject.transform.position;
			}
			else
			{
				var childCamera = gameObject.GetComponentInChildren<Camera>();

				if (childCamera.tag != "MainCamera")
				{
					DestroyImmediate(childCamera);

					var cameraList = FindObjectsOfType<Camera>();

					foreach (var camera in cameraList)
					{
						if (camera.tag == "MainCamera")
						{
							mainCamera = camera;
						}
					}

					if (mainCamera == null)
					{
						mainCamera = new Camera
						{
							tag = "MainCamera"
						};
					}


					mainCamera.transform.parent = gameObject.transform;
					mainCamera.transform.position = gameObject.transform.position;
				}
			}
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		if(mainCamera != null && mainCamera.transform.rotation != CameraRotation)
		{
			mainCamera.transform.rotation = CameraRotation;
		}
	}
}
