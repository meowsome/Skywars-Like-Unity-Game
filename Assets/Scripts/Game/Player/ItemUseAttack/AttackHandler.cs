using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEditor;

public class AttackHandler : NetworkBehaviour {
    private InventoryManagement inventoryManagement;
    private ItemUseHandler itemUseHandler;
    private Cooldowns cooldowns = new Cooldowns();
    private Cooldowns itemReload;

    void Start() {
        inventoryManagement = gameObject.GetComponent<InventoryManagement>();
        itemUseHandler = GameObject.FindWithTag("Held Item").GetComponent<ItemUseHandler>();
    }

    void Update() {
        // If overall cooldown isn't active AND item reload doesn't exis OR item reload isn't active
        if (!cooldowns.cooldownInteractActive && (itemReload == null || !itemReload.cooldownInteractActive)) {
            // On right click
            if (Input.GetMouseButtonDown(1)) {
                useItem(true);
            }

            // On left click
            if (Input.GetMouseButton(0)) {
                useItem(false);
            }
        }
        
        cooldowns.updateCooldowns();
        if (itemReload != null) itemReload.updateCooldowns();

        // TODO add XP
    }

    private void useItem(bool rightClick) {
        cooldowns.startInteractCooldown();
        
        Transform heldItemDisplay = getHeldItemDisplay();
        
        Vector3 bulletSpawnPos = Vector3.zero;
        if (heldItemDisplay != null) bulletSpawnPos = heldItemDisplay.position;
        
        if (inventoryManagement.activeItem != null && heldItemDisplay != null) {
            string itemNameWithCapitilization = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inventoryManagement.activeItem.name.ToLower());

            GenericItem item = heldItemDisplay.GetComponent(itemNameWithCapitilization) as GenericItem;

            if (item != null) {
                // Activate the reload time for this item
                itemReload = new Cooldowns(item.reload);
                itemReload.startInteractCooldown();
                itemUseHandler.useItem(item, rightClick);
            }
        }
    }

    private Transform getHeldItemDisplay() {
        foreach (Transform child in transform.Find("Held Item")) {
            if (child.CompareTag("Held Item Display")) return child;
        }

        return null;
    }

    [Command]
    public void Fire(Vector3 bulletSpawnPos, Vector3 forward, Quaternion playerRotation, float accuracy, float damage) {
        GameObject.FindWithTag("MapHandler").GetComponent<AttackHandlerServer>().FireServer(bulletSpawnPos, forward, playerRotation, accuracy, damage, gameObject.name);
    }
}
