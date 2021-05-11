using UnityEngine;

// Two types of cooldowns:
// Interact: Picking up items, left/right clicking
// Keypress: Pressing key on keyboard

public class Cooldowns {
    private float cooldownKeypress = 0;
    private float cooldownKeypressAmount = 0.25f;
    public bool cooldownKeypressActive {get; set;} = false;
    private float cooldownInteract = 0;
    private float cooldownInteractAmount = 0.075f;
    public bool cooldownInteractActive {get; set;} = false;

    // Constructor with optional amount, used for item reload times
    public Cooldowns(float amount = 0f) {
        // If amount is not null, overwrite default values
        if (amount == 0f) return;
        cooldownKeypressAmount = amount;
        cooldownInteractAmount = amount;
    }

    public void updateCooldowns() {
        // Decrease cooldown
        if (cooldownKeypressActive && cooldownKeypress > 0) {
            cooldownKeypress -= Time.deltaTime;
        }
        if (cooldownInteractActive && cooldownInteract > 0) {
            cooldownInteract -= Time.deltaTime;
        }

        // If below zero, stpo cooldown.
        if (cooldownKeypress < 0) {
            cooldownKeypress = 0;
            cooldownKeypressActive = false;
        }
        if (cooldownInteract < 0) {
            cooldownInteract = 0;
            cooldownInteractActive = false;
        }
    }
    
    public void startKeypressCooldown() {
        cooldownKeypress = cooldownKeypressAmount;
        cooldownKeypressActive = true;
    }

    public void startInteractCooldown() {
        cooldownInteract = cooldownInteractAmount;
        cooldownInteractActive = true;
    }
}