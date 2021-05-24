// Handle the actual damage that each of the individual items give

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEditor;

public class DamageHandler : NetworkBehaviour {
    private float redHitDelay = 0.4f;
    private Color redColor = Color.red;

    public void damagePlayer(string sender, string receiver, float amount) {
        if (isServer) {
            damagePlayerServer(sender, receiver, amount);
        }
    }

    private void damagePlayerServer(string sender, string receiver, float amount) {
        StartCoroutine(glowRed(receiver));

        // Track amount of damage dealt (server side)
        DamageTracker damageTracker = GameObject.FindWithTag("MapHandler").GetComponent<MapHandlerServer>().damageTracker;
        damageTracker.addDamage(sender, receiver, amount);

        // Print damage for testing
        foreach (KeyValuePair<string, Dictionary<string, float>> senderDict in damageTracker.projectileHits) {
            foreach (KeyValuePair<string, float> receiverDict in senderDict.Value) {
                Debug.Log(senderDict.Key + " damaged " + receiverDict.Key + " at value " + receiverDict.Value);
            }
        }

        // Call FROM server
        damagePlayerClients(sender, receiver, amount);
    }

    [ClientRpc]
    private void damagePlayerClients(string sender, string receiver, float amount) {
        StartCoroutine(glowRed(receiver));

        // Visually decrease health bar
        HealthBehavior healthBehavior = GameObject.Find("Health/Health Canvas/Health Bar").GetComponent<HealthBehavior>();

        if (healthBehavior.name == receiver) {
            healthBehavior.decreaseHealth(amount);
        }
    }

    // Make glow red for defined # of seconds
    private IEnumerator glowRed(string receiver) {
        GameObject receiverCharacterRepresentation = GameObject.Find(receiver + "/Cylinder"); // Get the game object
        Color oldColor = receiverCharacterRepresentation.GetComponent<MeshRenderer>().material.color; // Keep track of old color

        receiverCharacterRepresentation.GetComponent<MeshRenderer>().material.color = redColor; // Turn red
        yield return new WaitForSeconds(redHitDelay); // Wait, delay for defined # of seconds
        receiverCharacterRepresentation.GetComponent<MeshRenderer>().material.color = oldColor; // Fade from red to old color again
    }
}