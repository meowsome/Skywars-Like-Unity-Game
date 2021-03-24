using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private float maxHealth = 100.0f;
    private Slider slider;
    public float health;

    void Start() {
        health = maxHealth;
        slider = gameObject.GetComponent<Slider>();
        slider.value = health;
    }

    public void setHealth(float health) {
        this.health = health;
        slider.value = health;
    }

    public void decreaseHealth(float decreaseAmount) {
        setHealth(health - decreaseAmount);
    }
}
