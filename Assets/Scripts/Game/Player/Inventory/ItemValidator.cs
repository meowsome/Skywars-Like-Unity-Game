using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemValidator {
    public int getMaxInventorySize() {
        return 6;
    }

    public bool isStackable(Item item) {
        string itemType = getItemType(item);
        return itemType != "melee" && itemType != "projectile" && itemType != "armor";
    }

    private string getItemType(Item item) {
        switch (item.name.ToLower()) {
            case "watergun":
            case "musket":
            case "rifle":
            case "pistol":
            case "machine gun":
            case "submachine gun":
            case "assault rifle":
            case "sniper rifle":
            case "revolver":
            case "snowball":
            case "egg":
            case "flamethrower":
            case "bow":
                return "projectile";
            case "knife":
            case "sparklers":
            case "dagger":
            case "ka-bar":
            case "bolo knife":
                return "melee";
            case "collar":
            case "bulletproof vest":
            case "helmet":
            case "boots":
                return "armor";
            case "scope":
                return "upgrades";
            case "compass":
                return "locator";
            case "turtle shell":
                return "explosive";
            case "invisibility potion":
            case "power potion":
            case "speed potion":
            case "health potion":
            case "regeneration potion":
                return "potion";
            case "slime":
            case "banana":
            case "land mine":
            case "ghoul":
            case "candy":
            case "ice cube":
            case "lava bucket":
                return "trap";
            case "bullet":
            case "dart":
            case "arrow":
                return "ammunition";
            case "burger":
            case "ice pop":
            case "bandage":
                return "heal";
            default:
                return "other";
        }
    }
}