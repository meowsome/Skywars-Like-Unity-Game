using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehavior : MonoBehaviour {
    private int spinSpeed = 30;

    void Update() {
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);

        Vector3 pos = transform.position;
        pos.y = Mathf.Sin(Time.time) + GetComponent<Collider>().bounds.size.y;
        transform.position = pos;
    }
}
