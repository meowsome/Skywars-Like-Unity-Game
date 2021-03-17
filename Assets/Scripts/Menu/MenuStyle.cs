using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class MenuStyle : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
    private Text text;
    private Color defaultColor = Color.white;
    private Color hoverColor = Color.yellow;
    private Color clickColor = Color.red;
    private Color prevColor;

    void Start() {
        text = GetComponentInChildren<Text>();
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        // Add hover zoom functionality
        text.color = hoverColor;
        prevColor = hoverColor;
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        text.color = defaultColor;
        prevColor = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData) {
        text.color = clickColor;
    }

    public void OnPointerUp(PointerEventData eventData) {
        text.color = prevColor;
    }

    public void OnPointerClick(PointerEventData eventData) {
        // Reset default color for resetting color back to white when moving between menus
        text.color = defaultColor;
        prevColor = defaultColor;
    }
}