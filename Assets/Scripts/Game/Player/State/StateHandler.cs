using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateHandler : MonoBehaviour {
    public int state; // 0 = alive, 1 = spectator

    void Start() {
        state = 0;
    }

    public void enableSpectatorState() {
        state = 1; // Turn state to spectator state
        transform.localScale = new Vector3(0, 0, 0); // Hide from view
        gameObject.layer = LayerMask.NameToLayer("Spectators"); // Prevent collisions with map

        // Reflect for server and all clients

        // Remove inventory and items
    }

    public void enableAliveState() {
        state = 0; // Turn state to alive state
        transform.localScale = new Vector3(1, 1, 1); // Show in view
        gameObject.layer = LayerMask.NameToLayer("Default"); // Re-enable collisions with map
    }

    public bool isPlayerAlive() {
        return state == 0;
    }
}