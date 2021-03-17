using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class HotbarItemUpdates {
    private float offset = 32.5f;
    private float numItemsOld = 0;
    private float width = 0;
    private string hotbarPath = "Hotbar/";
    private float hotbarHeight = 125.0f;

    public void setHotbarItemPositions(Transform itemsCanvas) {
        float offsetWidth = offset;

        foreach (Transform item in itemsCanvas) {
            item.position = new Vector3(Screen.width - offsetWidth, offset, 0);

            offsetWidth += offset * 2;
        }
    }

    public void highlightCurrentHotbarSlot(Transform transform, int slot, Item item) {
        Transform outlineCanvas = transform.Find("Hotbar/Outline Canvas");
        Debug.Log("name: " + item.name);
        string itemName = item.name;

        if (itemName != null) {
            // If moving to an item and the canvas is hidden, show it
            outlineCanvas.gameObject.SetActive(true);

            // Actually move to the item
            transform.Find("Hotbar/Outline Canvas/Outline").position = new Vector3(Screen.width - (offset * 2 * slot) - offset, offset);
        } else {
            // If invalid item and canvas is not hidden, hide the outline
            outlineCanvas.gameObject.SetActive(false);
        }
    }
    
    public void setHotbarPosition(Transform transform, int activeItemSlot, int numberOfItems, Item item) {
        if (numberOfItems > 0 && (numItemsOld != numberOfItems || width != Screen.width)) {
            transform.Find(hotbarPath + "Background Canvas/Background").GetComponent<RectTransform>().sizeDelta = new Vector2(offset * 2 * numberOfItems, hotbarHeight); // Update the width and height, x2 because needs to cover entire item width
            
            transform.Find(hotbarPath + "Background Canvas/Background").transform.position = new Vector3(Screen.width - transform.Find(hotbarPath + "Background Canvas/Background").GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0); // Update the position to bottom right

            numItemsOld = numberOfItems;
            width = Screen.width;

            setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
            highlightCurrentHotbarSlot(transform, activeItemSlot, item);
        }
    }

    public void addItemInHotbar(Item item, Transform transform) {
        GameObject itemContainer = new GameObject();
        itemContainer.transform.SetParent(GameObject.Find("Hotbar/Items Canvas").transform);
        itemContainer.name = item.name;

        GameObject itemGameObject = new GameObject();
        itemGameObject.transform.SetParent(itemContainer.transform);
        itemGameObject.name = item.name;
        Image itemSpriteComponent = itemGameObject.AddComponent<Image>();

        itemSpriteComponent.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Images/" + item.name.ToLower() + ".png", typeof(Sprite));
        itemGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / 20, Screen.width / 20);
        
        GameObject numberGameObject = new GameObject();
        numberGameObject.transform.SetParent(itemContainer.transform);
        numberGameObject.transform.position = new Vector3(offset, 0, 0);
        numberGameObject.AddComponent<RectTransform>();
        numberGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        Text numberTextComponent = numberGameObject.AddComponent<Text>();
        numberTextComponent.text = item.amount.ToString();
        numberTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        numberTextComponent.fontSize = 20;
        numberTextComponent.color = Color.black;
        numberTextComponent.name = "Amount";

        setHotbarItemPositions(transform.Find("Hotbar/Items Canvas"));
    }

    public void incrementItemInHotbar(Item item) {
        GameObject.Find("Hotbar/Items Canvas/" + item.name + "/Amount").GetComponent<Text>().text = item.amount.ToString();
    }
}