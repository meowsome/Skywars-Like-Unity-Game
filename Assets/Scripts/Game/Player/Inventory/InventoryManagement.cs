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
        Item itemSearchResults = inventory.FirstOrDefault(item => item.name.ToLower() == name.ToLower());

        // Add to existing item stack
        if (itemSearchResults != null) {
            if (itemValidator.isStackable(itemSearchResults) && itemSearchResults.amount + amount <= stackSize) {
                itemSearchResults.amount += amount;
                hotbarItemUpdates.incrementItemInHotbar(itemSearchResults);
                return true;
            } else return false; // If item found and not stackable, no need to add
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

    // Only remove an item from inventory or decrease item from stack of items 
    public void Remove(string name, int amount) {
        Item itemSearchResults = inventory.FirstOrDefault(item => item.name == name);

        // Remove from item stack
        if (itemSearchResults != null) {
            if (itemValidator.isStackable(itemSearchResults)) {
                // Remove amount from stack
                itemSearchResults.amount -= amount;
                
                if (itemSearchResults.amount == 0) {
                    // If no more left in this stack, remove from inventory and hotbar
                    inventory.Remove(itemSearchResults);
                    hotbarItemUpdates.setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
                    hotbarItemUpdates.highlightCurrentHotbarSlot(transform, activeItemSlot, null); // Un-highlight hotbar slot
                    // Actually remove from hotbar
                    Destroy(transform.Find("Hotbar/Items Canvas/" + itemSearchResults.name).gameObject);
                    
                    setActiveItemSlot(activeItemSlot);
                } else {
                    // Decrement in hotbar
                    hotbarItemUpdates.incrementItemInHotbar(itemSearchResults);
                }
            } else {
                // Remove entire item, no stack
                inventory.Remove(itemSearchResults);
                hotbarItemUpdates.setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
                hotbarItemUpdates.highlightCurrentHotbarSlot(transform, activeItemSlot, null); // Un-highlight hotbar slot
                // Actually remove from hotbar
                Destroy(transform.Find("Hotbar/Items Canvas/" + itemSearchResults.name).gameObject);
                setActiveItemSlot(activeItemSlot);
            }
        }
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
