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
        accuracy = 0.05f;
        damage = 5f;
        reload = 1f; // 1 sec
    }

    public override bool use() {
        Transform playerTransform = transform.parent.transform.parent.transform;
        Vector3 forwardWithVerticalRotation = new Vector3(playerTransform.forward.x, Camera.main.transform.eulerAngles.x, playerTransform.forward.z);

        Vector3 bulletSpawnPos = transform.parent.Find("BulletSpawn").transform.position;
        foreach (Transform child in transform) if (child.CompareTag("BulletSpawn")) bulletSpawnPos = child.position; // The location where the bullet will be spawned, constantly tracked by the client and sent to the server when it's time for it to be spawned (https://answers.unity.com/questions/47989/is-it-possible-to-findwithtag-only-within-children.html)
        attackHandler.Fire(bulletSpawnPos, forwardWithVerticalRotation, playerTransform.rotation, accuracy, damage); // Specify damage, speed, accuracy here
        return true;
    }
}
