using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;

public class StartMenu : MonoBehaviour, IPointerClickHandler {
    private GameFinder gameFinder;
    private GameObject mainMenu;
    private GameObject chooseGameMenu;

    void Start() {
        gameFinder = GameObject.Find("GameFinder").GetComponent<GameFinder>();
        mainMenu = GameObject.Find("Main Menu").transform.GetChild(0).gameObject;
        chooseGameMenu = GameObject.Find("Select Game").transform.GetChild(0).gameObject;
    }

    public void OnPointerClick(PointerEventData eventData) {
        string clicked = eventData.pointerCurrentRaycast.gameObject.name;

        switch (clicked) {
            case "Start":
                mainMenu.SetActive(false);
                chooseGameMenu.SetActive(true);
                break;
            case "Quit":
                Application.Quit();
                break;
            case "Back":
                mainMenu.SetActive(true);
                chooseGameMenu.SetActive(false);
                break;
            case "Automatic":
                gameFinder.FindGame();
                break;
            case "Map 1":
                gameFinder.JoinGame(1);
                break;
            case "Map 2":
                gameFinder.JoinGame(2);
                break;
        }
    }

    public void HideError() {
        GameObject.Find("Error Canvas").SetActive(false);
    }
}