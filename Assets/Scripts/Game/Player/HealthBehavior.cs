using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehavior : MonoBehaviour {
    private float maxHealth = 100.0f;
    private Slider slider;
    public float health;
    public string name;

    void Start() {
        health = maxHealth;
        slider = gameObject.GetComponent<Slider>();
        slider.value = health;
    }

    public void setHealth(float health) {
        this.health = health;
        slider.value = health;

        if (this.health < 0) {
            GameObject.Find(name).GetComponent<StateHandler>().enableSpectatorState();
        }
    }

    public void decreaseHealth(float decreaseAmount) {
        setHealth(health - decreaseAmount);
    }
}
