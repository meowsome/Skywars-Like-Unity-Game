using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemValidator {
    public int getMaxInventorySize() {
        return 6;
    }

    public bool isStackable(Item item) {
        string itemType = getItemType(item);
        return itemType != "weapon"; // Weapons are not stackable
    }

    private string getItemType(Item item) {
        switch (item.name.ToLower()) {
            case "watergun":
                return "weapon";
            case "potato":
                return "food";
            default:
                return "other";
        }
    }
}