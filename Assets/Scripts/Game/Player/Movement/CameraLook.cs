using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : NetworkBehaviour {
    private float sensitivity = 700f;
    // private float sensitivity = 100f;
    private float rotationX = 0f;
    private float verticalOffset = 4.0f;
    private float horizontalOffset = 4.0f;
    public Transform player;
    private PauseManager pauseManager;

    void Start() {
        if (isLocalPlayer && !isServer) {
            Cursor.lockState = CursorLockMode.Locked;

            Camera.main.transform.position = transform.position - transform.forward * horizontalOffset + transform.up * verticalOffset;
            Camera.main.transform.LookAt(transform.position);
            Camera.main.transform.SetParent(transform);

            //Reactivate camera, needed to enable separate cameras for each player
            gameObject.GetComponentInChildren<Camera>().enabled = false;
            gameObject.GetComponentInChildren<Camera>().enabled = true;

            pauseManager = GameObject.Find("Pause").GetComponent<PauseManager>();
        }
    }

    void Update() {
        if (isLocalPlayer && !isServer && !pauseManager.isPaused()) {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            Camera.main.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f); // Y rotation
            transform.Rotate(Vector3.up * mouseX); // X rotation
        }
    }
}
