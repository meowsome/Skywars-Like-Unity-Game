using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehavior : MonoBehaviour {
    private float maxHealth = 100.0f;
    private Slider slider;
    public float health;
    private GameObject playerGameObject;
    public string name;
    public string setPlayer {
        get {
            return this.playerGameObject.name;
        }
        set {
            this.playerGameObject = GameObject.Find(value);
            this.name = value;
        }
    }

    void Start() {
        health = maxHealth;
        slider = gameObject.GetComponent<Slider>();
        slider.value = health;
    }

    public void setHealth(float health) {
        this.health = health;
        slider.value = health;

        if (this.health < 0) {
            playerGameObject.GetComponent<StateHandler>().enableSpectatorState();
        }
    }

    public void decreaseHealth(float decreaseAmount) {
        setHealth(health - decreaseAmount);
    }
}
