using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemUseHandler : MonoBehaviour {
    private Movement movement;

    void Start() {
        movement = transform.parent.GetComponent<Movement>();
    }

    public void useItem(GenericItem item, bool isRightClick) {
        if (item == null) return; // If item no longer exists, then cancel using the item

        if (isRightClick) {
            // Does current item have right click action?
            if (hasRightClickCapability(item)) {
                carryOutItemUse(item);
            }
        } else {
            if (hasLeftClickCapability(item)) {
                carryOutItemUse(item);
            }
        }
    }

    private bool hasRightClickCapability(GenericItem item) {
        return item.type == "projectile";
    }

    private bool hasLeftClickCapability(GenericItem item) {
        return item.type == "melee";
    }

    // Handle using item
    private void carryOutItemUse(GenericItem item) {
        bool success = item.use();
        if (success) {
            if (item.removeAfterUse) {
                // Remove item from inventory. Stacks already handled
                movement.removeHeldItemFromInventory();
            }
        }
    }

    // private void meleeAttack(GenericItem item) {
    //     if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, 5f)) {
    //         if (hit.transform.tag == "Player") {
    //             // Actually hit the enemy
    //             Movement movement = hit.transform.gameObject.GetComponent<Movement>();
    //             if (movement) movement.Hit(transform.name, item.hitDamage);
    //         }
    //     }
    // }
}
