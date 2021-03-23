using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemBehavior : NetworkBehaviour {
    private int spinSpeed = 30;
    private Vector3 move;
    private float verticalVelocity;
    private float gravity = 50.0f;
    private float destroyDistance = 500f;

    void Start() {
        move = new Vector3(0, 0, 0);

        // Prevent collisions with players
        Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<Collider>(), GetComponent<Collider>());
    }

    void Update() {
        handleRotate();
        checkIfTooFar();
    }

    private void handleRotate() {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);

        Vector3 pos = transform.position;
        if (isGrounded()) pos.y = Mathf.Sin(Time.time) + GetComponent<Collider>().bounds.size.y;
        transform.position = pos;
    }

    private bool isGrounded() {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, 1f);
    }

    // If dropped item is further than defined distance from the center of the map, destroy the item altogether
    private void checkIfTooFar() {
        if (Vector3.Distance(transform.position, new Vector3(0, 0, 0)) > destroyDistance) NetworkServer.Destroy(gameObject);
    }
}
