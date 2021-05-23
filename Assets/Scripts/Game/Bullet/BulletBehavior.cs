using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletBehavior : NetworkBehaviour {
    private float destroyDistance = 500f;
    public float damage { get; set; }
    public string sender { get; set; }
    private DamageHandler damageHandler;
    private bool hitSomething = false;

    void Start() {
        damageHandler = GameObject.Find("DamageHandler").GetComponent<DamageHandler>();
    }

    void Update() {
        checkIfTooFar();
    }

    // Handle collision, destroy bullet
    private void OnTriggerEnter(Collider collider) {
        // Only allow collision if hasn't collided with anything else yet
        if (!hitSomething) {
            hitSomething = true; // Track that collision happened to prevent counting multiple points in a row

            Debug.Log("hit " + collider.name);
            
            NetworkServer.Destroy(gameObject);
            
            // If bullet hit player, damage player
            if (collider.gameObject.tag == "Player") {
                damageHandler.damagePlayer(sender, collider.gameObject.name, damage);
            }
        }
    }

    // If bullet travels further than defined distance from the center of the map, destroy
    private void checkIfTooFar() {
        if (Vector3.Distance(transform.position, new Vector3(0, 0, 0)) > destroyDistance) NetworkServer.Destroy(gameObject);
    }
}
