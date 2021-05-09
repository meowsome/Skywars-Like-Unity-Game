using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Watergun : GenericItem {
    protected override void Start() {
        base.Start();
        
        type = "projectile";
        stackable = false;
        hitDamage = 20;
        shotDamage = 25;
        removeAfterUse = false;
    }

    public override bool use() {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        Vector3 forwardWithVerticalRotation = new Vector3(playerTransform.forward.x, Camera.main.transform.eulerAngles.x, playerTransform.forward.z);

        Vector3 bulletSpawnPos = GameObject.FindWithTag("BulletSpawn").transform.position; // The location where the bullet will be spawned, constantly tracked by the client and sent to the server when it's time for it to be spawned
        attackHandler.Fire(bulletSpawnPos, forwardWithVerticalRotation, playerTransform.rotation); // Specify damage, speed, accuracy here
        return true;
    }
}
