using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour {
    private InventoryManagement inventoryManagement;

    void Start() {
        inventoryManagement = gameObject.GetComponent<InventoryManagement>();
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 5f)) {
                if (hit.transform.tag == "Player") {
                    // Actually hit the enemy
                    Movement movement = hit.transform.gameObject.GetComponent<Movement>();
                    if (movement) movement.Hit(transform.name, getHitDamage(inventoryManagement.activeItem));

                    // TODO add XP
                }
            }
        }

        // TODO check for gun shots / projectiles shot
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
