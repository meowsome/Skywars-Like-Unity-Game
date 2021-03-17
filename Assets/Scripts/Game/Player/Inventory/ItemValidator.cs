using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemValidator {
    public int getMaxInventorySize() {
        return 6;
    }

    public bool isStackable(Item item) {
        string itemType = getItemType(item);
        return itemType != "Weapon"; // Weapons are not stackable
    }

    private string getItemType(Item item) {
        switch (item.name) {
            case "Watergun":
                return "Weapon";
            case "Potato":
                return "Food";
            default:
                return "Other";
        }
    }
}