using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class InventoryManagement : MonoBehaviour {
    public List<Item> inventory = new List<Item>();
    private int stackSize = 100;
    private float width = 0;
    private ItemValidator itemValidator;
    private HotbarItemUpdates hotbarItemUpdates;
    public int activeItemSlot; // 0 is far right, "max" is far left
    public Item activeItem { get; set; }

    void Start() {
        width = Screen.width;
        itemValidator = new ItemValidator();
        hotbarItemUpdates = new HotbarItemUpdates();
        activeItemSlot = 0;
    }

    public bool Add(string name, int amount) {
        Item itemSearchResults = inventory.FirstOrDefault(item => item.name == name);

        // Add to existing item stack
        if (itemSearchResults != null && itemValidator.isStackable(itemSearchResults) && itemSearchResults.amount + amount <= stackSize) {
            itemSearchResults.amount += amount;
            hotbarItemUpdates.incrementItemInHotbar(itemSearchResults);
            return true;
        } else if (inventory.Count < itemValidator.getMaxInventorySize()) {
            // Create new item stack
            Item item = new Item {
                name = name,
                amount = amount
            };

            inventory.Add(item);
            hotbarItemUpdates.addItemInHotbar(item, transform);
            return true;
        } else return false;
    }

    public void Remove(string name, int amount, Vector3 pos) {
        Item itemSearchResults = inventory.FirstOrDefault(item => item.name == name);

        // Add to existing item stack
        if (itemSearchResults != null) {
            if (itemValidator.isStackable(itemSearchResults)) {
                // Remove amount from stack
                itemSearchResults.amount -= amount;
                
                if (itemSearchResults.amount == 0) {
                    // If no more left in this stack, remove from inventory and hotbar
                    inventory.Remove(itemSearchResults);
                    hotbarItemUpdates.setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
                    // Actually remove from hotbar
                    Destroy(transform.Find("Hotbar/Items Canvas/" + itemSearchResults.name));
                } else {
                    // Decrement in hotbar
                    hotbarItemUpdates.incrementItemInHotbar(itemSearchResults);
                }
            } else {
                // Remove entire item, no stack
                inventory.Remove(itemSearchResults);
                hotbarItemUpdates.setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
                // Actually remove from hotbar
                Destroy(transform.Find("Hotbar/Items Canvas/" + itemSearchResults.name));
            }

            // Spawn new amount of item at feet
            for (int i = 0; i < amount; i++) {
                CreateDroppedItem(itemSearchResults.name, pos);
            }
        }
    }

    public void CreateDroppedItem(string name, Vector3 pos) {
        Instantiate(Resources.Load(name), pos, Quaternion.identity);
    }

    public int InventoryCount() {
        return inventory.Count;
    }

    public Item GetItemInSlot(int slot) {
        return inventory.Count > slot ? inventory[slot] : null;
    }

    public void setActiveItemSlot(int slot) {
        // Set both activeItemSlot and activeItem when updated
        activeItemSlot = slot;
        activeItem = GetItemInSlot(slot);
    }
}
