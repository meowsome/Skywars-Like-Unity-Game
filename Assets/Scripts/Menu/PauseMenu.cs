using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class PauseMenu : MonoBehaviour, IPointerClickHandler {
    private Text text;
    private NetworkManager networkManager;

    void Start() {
        text = GetComponentInChildren<Text>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }
    
    public void OnPointerClick(PointerEventData eventData) {
        string clicked = eventData.pointerCurrentRaycast.gameObject.name;

        switch (clicked) {
            case "Leave":
                networkManager.StopClient();
                SceneManager.LoadScene("Menu");
                break;
            case "Options":
                Debug.Log("OPTIONS");
                break;
            case "Resume":
                PauseManager pauseManager = GameObject.Find("Pause").GetComponent<PauseManager>();
                pauseManager.togglePause();
                break;
        }
    }
}