using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MapHandlerClient : NetworkBehaviour {
    private Map map;
    [SyncVar] float timer;
    private static float updateInterval = 0.3f;

    void Start() {
        map = GameObject.Find("MapHandler").GetComponent<MapHandlerServer>().map;
        
        if (isLocalPlayer) StartCoroutine(UpdateTimer(updateInterval));
    }

    IEnumerator UpdateTimer(float updateInterval) {
        while (true) {
            CmdSyncVarWithClients();
            RefreshTime();

            yield return new WaitForSeconds(updateInterval); // Put coroutine to sleep
        }
    }

    private void RefreshTime() {
        GameObject.Find("Timer").GetComponentInChildren<Text>().text = GetFormattedTime();
    }

    // http://answers.unity.com/answers/315200/view.html
    private string GetFormattedTime() {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // This code is run on client. Server sent it a new timer.
    [ClientRpc]
    void RpcSyncVarWithClients(float newTimer) {
        timer = newTimer; // Set local timer as new timer from server
    }

    // This code is run on server. Client told server to send it a new timer.
    [Command]
    void CmdSyncVarWithClients() {
        float newTimer = GameObject.Find("MapHandler").GetComponent<MapHandlerServer>().map.timer; // Get new timer from the MapHandlerServer class, which only server has access to
        RpcSyncVarWithClients(newTimer);
    }
}
