// https://forum.unity.com/threads/attempting-to-make-a-fall-damage-system.775394/#post-5162093

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class FallDamage : NetworkBehaviour {
    private CharacterController controller;
    private HealthBar healthBar;
    bool fallen = false;
    float damage = 0;
    float pos = 0;

    void Start() {
        if (isLocalPlayer) {
            this.controller = GetComponent<CharacterController>();
            this.healthBar = GameObject.Find("Health/Health Canvas/Health Bar").GetComponent<HealthBar>();
            pos = controller.transform.position.y;
        }
    }

    void Update() {
        if (isLocalPlayer) {
            if (controller.velocity.y < 0) {
                if (!fallen) pos = controller.transform.position.y; // Update position that the player started from
                damage += Time.deltaTime; // Count time as "fall damage"
                fallen = true;
            } else if (fallen) {
                // Whenver player is not falling anymore...

                // If player has been falling for 0.5 seconds and has fallen more than 5 units
                if (damage > 0.5 && pos - controller.transform.position.y > 5) {
                    healthBar.decreaseHealth((float) Math.Pow(1 + damage, 6));
                }

                fallen = false;
                damage = 0;
            }
        }
    }
}