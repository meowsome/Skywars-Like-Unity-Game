using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEditor;

public class AttackHandler : NetworkBehaviour {
    private InventoryManagement inventoryManagement;
    private Transform bulletSpawn;

    void Start() {
        inventoryManagement = gameObject.GetComponent<InventoryManagement>();
        bulletSpawn = GameObject.FindWithTag("BulletSpawn").transform; // The location where the bullet will be spawned, constantly tracked by the client and sent to the server when it's time for it to be spawned
    }

    void Update() {
        // On left click
        if (Input.GetMouseButtonDown(1)) {
            Vector3 forwardWithVerticalRotation = new Vector3(transform.forward.x, Camera.main.transform.eulerAngles.x, transform.forward.z);
            Fire(bulletSpawn.position, forwardWithVerticalRotation, transform.rotation);
        }

        if (Input.GetMouseButton(0)) {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 5f)) {
                if (hit.transform.tag == "Player") {
                    // Actually hit the enemy
                    Movement movement = hit.transform.gameObject.GetComponent<Movement>();
                    if (movement) movement.Hit(transform.name, getHitDamage(inventoryManagement.activeItem));
                }
            }
        }

        // TODO add XP
    }

    [Command]
    private void Fire(Vector3 pos, Vector3 forward, Quaternion playerRotation) {
        GameObject.Find("MapHandler").GetComponent<AttackHandlerServer>().FireServer(pos, forward, playerRotation);
    }

    private float getHitDamage(Item item) {
        if (item == null) return 15;
        
        switch(item.name) {
            case "watergun":
                return 20;
            default:
                return 15;
        }
    }

    private float getShotDamage(Item item) {
        if (item == null) return 15;

        switch(item.name) {
            case "watergun":
                return 25;
            default:
                return 15;
        }
    }
}
