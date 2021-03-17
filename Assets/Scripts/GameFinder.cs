using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameFinder : MonoBehaviour {
    private NetworkManager networkManager;
    private GameObject mainMenu;
    private GameObject chooseGameMenu;
    private int maxPlayers = 12;
    private List<string> maps = new List<string>() {"Map 1", "Map 2"};
    private MapHandlerServer mapHandler;

    void Start() {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        mainMenu = GameObject.Find("Main Menu").transform.GetChild(0).gameObject;
        chooseGameMenu = GameObject.Find("Select Game").transform.GetChild(0).gameObject;   
    }

    public void FindGame() {
        //FIXME 


        float lowestTime = GetTimer(maps[0]);
        string mapToJoin = "";

        //  Organize by time left, need to find each of the scenes and find out which one has the least time left
        foreach (string map in maps) {
            if (GetPlayers(map) < maxPlayers) {
                if (GetTimer(map) < lowestTime) {
                    lowestTime = GetTimer(map);
                    mapToJoin = map;
                }
            }
        }

        if (!string.IsNullOrEmpty(mapToJoin)) {
            HideMenus();

            SceneManager.LoadScene("Map " + mapToJoin);
            
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        } else {
            Debug.Log("NO MAPS ONLINE");
            
            ShowError("No suitable maps could be found. Please try again later.");
        }
    }

    public void JoinGame(int gameNum) {
        gameNum--; // Decrease by 1 because it starts at "map 1" but the first array element is 0
        string mapName = maps[gameNum];

        SceneManager.LoadScene(mapName);
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    private void HideMenus() {
        mainMenu.SetActive(false);
        chooseGameMenu.SetActive(false);
    }

    private float GetTimer(string map) {
        // get request to web server??


        // MapHandler mapHandler = getMapHandler(map);
        // return mapHandler ? mapHandler.map.timer : 5f;
        return 0f;
    }

    private int GetPlayers(string map) {
        // get request to server that hosts everything????


        // MapHandler mapHandler = getMapHandler(map);
        // return mapHandler ? mapHandler.map.players.Count : 50;
        return 0;
    }

    private void ShowError(string message) {
        GameObject.Find("Error").transform.GetChild(0).transform.GetChild(2).GetComponent<Text>().text = message;
        GameObject.Find("Error").transform.GetChild(0).gameObject.SetActive(true);
    }
}
