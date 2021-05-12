using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletBehavior : NetworkBehaviour {
    private float destroyDistance = 500f;
    public string sender { get; set; }

    void Update() {
        checkIfTooFar();
    }

    // Handle collision, destroy bullet
    private void OnTriggerEnter(Collider collider) {
        Debug.Log("hit " + collider.name);
        NetworkServer.Destroy(gameObject);
    }

    // If bullet travels further than defined distance from the center of the map, destroy
    private void checkIfTooFar() {
        if (Vector3.Distance(transform.position, new Vector3(0, 0, 0)) > destroyDistance) NetworkServer.Destroy(gameObject);
    }
}
