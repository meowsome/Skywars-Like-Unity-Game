using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using UnityEditor;

public class MapHandlerServer : NetworkBehaviour {
    public Map map { get; set; }
    private static float updateInterval = 0.3f;
    private bool createdStartingItems = false;

    void Start() {
        if (isServer) {
            if (map == null) {
                map = new Map(SceneManager.GetActiveScene().name);
                StartLobby();
            }
            
            StartCoroutine(UpdateTimer(updateInterval));
        }
    }

    void Update() {
        // If a player has joined and starting items have not been created yet, create them
        if (isServer && map.players.Count > 0 && !createdStartingItems) {
            createStartingItems();
        }
    }

    private void createStartingItems() {
        createdStartingItems = true;
        CreateItemServer(new Vector3(5, 0, 0), 1, "watergun");
        CreateItemServer(new Vector3(10, 0, 0), 1, "watergun");
        CreateItemServer(new Vector3(15, 0, 0), 1, "watergun");
        CreateItemServer(new Vector3(20, 0, 0), 1, "watergun");
    }

    IEnumerator UpdateTimer(float updateInterval) {
        bool timerRunning = true;
        while (timerRunning) {
            if (isServer) {
                if (map.status == 0) {
                    // Lobby logic
                    if (map.timer <= 0) {
                        if (map.players.Count == 12) {
                            // Move from lobby to actual game
                            StartGame();
                        } else {
                            // Failed to reach number of players, restart lobby timer
                            map.timer = 180;
                        }
                    } else {
                        if (map.timer > 30 && map.players.Count >= 10) {
                            // 10 players reached, reduce timer to 30s
                            map.timer = 30;
                        }

                        RefreshTime();
                    }
                } else if (map.status == 1) {
                    // Game logic
                    RefreshTime();

                    if (map.players.Count == 0) {
                        // If everyone left, reset game
                        ResetGame();
                    }

                    if (map.timer <= 0 || map.playersAlive.Count <= 1) {
                        // If timer ran out OR 1 player is left
                        GameOver();
                    }


                } else if (map.status == 1) {
                    RefreshTime();
                    
                    if (map.timer <= 0 || map.playersAlive.Count <= 1) {
                        StartLobby();
                    }
                }
            }

            yield return new WaitForSeconds(updateInterval); // Put coroutine to sleep
        }
    }

    private void StartLobby() {
        map.timer = 180; // 3 min
        map.status = 0;
    }

    private void StartGame() {
        map.timer = 600; // 10 mins
        map.status = 1;
        map.playersAlive = new List<string>(map.players);
    }

    private void RefreshTime() {
        map.timer -= updateInterval;
        GameObject.Find("Timer").GetComponentInChildren<Text>().text = GetFormattedTime();
    }

    private void GameOver() {
        map.status = 2;

        List<string> winningPlayers = map.playersAlive; // Whoever is remaining is the winner

        map.timer = 60; // 1 min
        map.playersAlive = new List<string>(map.players);

        //Stop users from moving
        //Teleport all users to finish area

        if (winningPlayers.Count == 0) {
            // nobody won
        } else if (winningPlayers.Count == 1) {
            // 1 winner
        } else {
            // tie
        }
        
        // todo Show users on pedestal
        // todo Wait 30 seconds

        ResetGame();
    }

    private void ResetGame() {
        //Teleport back to lobby
        map.status = 0;
        map.timer = 300;
    }

    // http://answers.unity.com/answers/315200/view.html
    private string GetFormattedTime() {
        int minutes = Mathf.FloorToInt(map.timer / 60F);
        int seconds = Mathf.FloorToInt(map.timer - minutes * 60);

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void CreateItemServer(Vector3 pos, int amount, string itemName) {
        int i = 0;

        for(i = 0; i < amount; i++) {
            // Get item prefab and get texture for it
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/DroppedItems/" + itemName + ".prefab", typeof(GameObject));
            
            GameObject item = Instantiate(prefab, pos, Quaternion.identity);

            item.transform.SetParent(GameObject.Find("Dropped Items/Canvas").transform);
            item.name = itemName;

            NetworkServer.Spawn(item);

            CreateItemClient(item, pos, itemName);
        }
    }    

    [ClientRpc]
    void CreateItemClient(GameObject item, Vector3 pos, string itemName) {
        // Only need to update the current existing item
        item.transform.SetParent(GameObject.Find("Dropped Items/Canvas").transform);
        item.name = itemName;
        item.transform.position = pos;
    }
}
