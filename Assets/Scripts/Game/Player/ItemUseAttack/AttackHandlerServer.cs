using System;
using System.IO;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AttackHandlerServer : NetworkBehaviour {
    private GameObject bulletPrefab;
    private float bulletSpeed = 5000;
    private System.Random random = new System.Random(); // Need to declare random variable here so that it uses the same seed and doesn't repeat the same results

    void Start() {
        bulletPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/bullet.prefab", typeof(GameObject));
    }

    public void FireServer(Vector3 pos, Vector3 forward, Quaternion playerRotation, float accuracy, float damage) {
        // Update "foward" so y rotation is valid
        forward = getBulletRotation(forward, accuracy);
        
        // Also create on server...
        Vector3 spawnPos = pos + forward * 2;

        // Same rotation as player but rotated 90 degrees to be perpendicular (convert from quaternion, to vector3, then to quaternion again)
        Quaternion rotation = Quaternion.Euler(new Vector3(playerRotation.eulerAngles.x, 90f + playerRotation.eulerAngles.y, playerRotation.eulerAngles.z));

        // Create the game object with specified place, rotation, & parent
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rotation, GameObject.Find("Projectiles").transform);

        bullet.GetComponent<Rigidbody>().AddForce(forward * bulletSpeed); // Apply force to bullet

        NetworkServer.Spawn(bullet); // Create on server
        FireClient(bullet, forward, spawnPos); // Create on all clients
    }

    [ClientRpc]
    private void FireClient(GameObject obj, Vector3 forward, Vector3 spawnPos) {
        obj.transform.SetParent(GameObject.Find("Projectiles").transform); // Set the parent of the bullet to the projectiles
        obj.transform.position = spawnPos;
        obj.GetComponent<Rigidbody>().AddForce(forward * bulletSpeed); // Apply force to bullet
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay) {
        yield return new WaitForSeconds(delay);

         NetworkServer.Destroy(bullet);
    }

    private Vector3 getBulletRotation(Vector3 forward, float accuracy) {
        // If 270-360, looking upwards where 270 is up and 360 is out
        // If 0-90, looking downwards where 90 is down and 0 is out
        // Actual nums: -1 = down, 0 = straight, 1 = up
        float y = 0;
        if (forward.y >= 270) y = yCoordFormula(forward.y, 360, 360 - 270);
        else if (forward.y <= 90) y = yCoordFormula(forward.y, 90, 90 - 0) - 1;

        float randomAccuracyX = randomAccuracySkew(accuracy);
        float randomAccuracyY = randomAccuracySkew(accuracy);
        float randomAccuracyZ = randomAccuracySkew(accuracy);

        return new Vector3(forward.x + randomAccuracyX, y + 0.08f + randomAccuracyY, forward.z + randomAccuracyZ); // Add to angle the bullet slightly higher, add to all directions to make it randomized accuracy
    }

    private float yCoordFormula(float y, float max, float difference) {
        return (max - y) / difference;
    }

    // Get random float between -accuracy and +accuracy to add to rotation to make it higher chance of failure
    private float randomAccuracySkew(float accuracy) {
        float min = -accuracy; // Set min as negative of max
        return (float) random.NextDouble() * (accuracy - min) + min;
    }
}
