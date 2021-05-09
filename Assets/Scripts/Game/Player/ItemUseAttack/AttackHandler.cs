using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEditor;

public class AttackHandler : NetworkBehaviour {
    private InventoryManagement inventoryManagement;
    private ItemUseHandler itemUseHandler;

    void Start() {
        inventoryManagement = gameObject.GetComponent<InventoryManagement>();
        itemUseHandler = GameObject.FindWithTag("Held Item").GetComponent<ItemUseHandler>();
    }

    void Update() {
        // On right click
        if (Input.GetMouseButtonDown(1)) {
            useItem(true);
        }

        // On left click
        if (Input.GetMouseButton(0)) {
            useItem(false);
        }

        // TODO add XP
    }

    private void useItem(bool rightClick) {
        GameObject heldItemDisplay = GameObject.FindWithTag("Held Item Display");

        if (inventoryManagement.activeItem != null && heldItemDisplay != null) {
            string itemNameWithCapitilization = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(inventoryManagement.activeItem.name.ToLower());

            itemUseHandler.useItem(heldItemDisplay.GetComponent(itemNameWithCapitilization) as GenericItem, true);
        }
    }

    [Command]
    public void Fire(Vector3 bulletSpawnPos, Vector3 forward, Quaternion playerRotation) {
        GameObject.Find("MapHandler").GetComponent<AttackHandlerServer>().FireServer(bulletSpawnPos, forward, playerRotation);
    }
}
