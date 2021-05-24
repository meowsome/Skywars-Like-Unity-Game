using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using UnityEditor;

public class Movement : NetworkBehaviour {
    private CharacterController controller;
    private Vector3 move;
    private float speedMultiplier = 8.0f;
    private float sprintMultiplier = 1.5f;
    // private float crouchOffset = 1.0f;
    private float crouchSpeedMultiplier = 0.5f;
    private float crouchHeightMultiplier = 0.75f;
    private bool crouching = false;
    private float verticalVelocity;
    private float gravity = 50.0f;
    private float jumpForce = 16.0f;
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.V;
    private KeyCode crouchKey = KeyCode.C;
    private PauseManager pauseManager;
    private KeyCode pauseKey = KeyCode.Escape;
    private HotbarItemUpdates hotbarItemUpdates;
    private bool started = false;
    private InventoryManagement inventoryManagement;
    private HealthBehavior healthBehavior;
    private KeyCode dropKey = KeyCode.Q;
    private Cooldowns cooldowns = new Cooldowns();
    private MapHandlerClient mapHandler;
    private StateHandler stateHandler;
    private bool itemsHidden = false;
    private float destroyDistance = 500f;

    void Start() {     
        if (isLocalPlayer && !isServer) {
            controller = GetComponent<CharacterController>();
            move = new Vector3(0, 0, 0);

            // Set the name of the player's game object
            string playerName = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString();
            ChangePlayerGameObjectNameServer(gameObject.name, playerName);

            GameObject.Find("Health/Health Canvas/Health Bar").GetComponent<HealthBehavior>().name = playerName; // Set the name of the new player to the health behavior, for this client ONLY (not server, not other clients)

            inventoryManagement = gameObject.GetComponent<InventoryManagement>();
            
            hotbarItemUpdates = new HotbarItemUpdates();


            GameObject.Find("Hotbar").transform.parent = this.transform; // Set as child of player
            setHUDItems();

            pauseManager = GameObject.Find("Pause").GetComponent<PauseManager>();
            healthBehavior = GameObject.Find("Health/Health Canvas/Health Bar").GetComponent<HealthBehavior>();

            mapHandler = transform.GetComponent<MapHandlerClient>();
            stateHandler = transform.GetComponent<StateHandler>();
        } else if (isServer && !started) {
            Debug.Log("Server started");
            started = true;
        }
    }

    private void Update() {
        if (isLocalPlayer && !isServer) {
            handlePause();

            if (!pauseManager.isPaused()) {
                handleMovement();

                if (stateHandler.isPlayerAlive()) {
                    // If the HUD items were previously hidden by spectator mode, re-enable them
                    if (itemsHidden) showHUDItems();

                    setHUDItems();
                    handleInventoryInput();
                    cooldowns.updateCooldowns();
                    itemsHidden = false;
                } else {
                    if (!itemsHidden) {
                        hideHUDItems();
                        itemsHidden = true;
                    }
                }
            }

            checkIfTooFar();
        } else hideOtherHotbar();
    }

    private void handleMovement() {
        if (isAtEdgeAndCrouching() && stateHandler.isPlayerAlive()) return;

        move = getHorizontalMovement(); // Set horizontal WASD movement

        if (controller.isGrounded || !stateHandler.isPlayerAlive()) {
            verticalVelocity = getVerticalMovement(); // Set vertical movement for next iteration. 
            move.y = verticalVelocity; // Set vertical movement to what it was before
            
            handleCrouch();
        } else if (stateHandler.isPlayerAlive()) {
            verticalVelocity -= getGravityMovement(); // Set vertical movement to simulate gravity and store for next iteration
            move.y = verticalVelocity; // Set vertical movement to what it was before
        }

        controller.Move(move * Time.deltaTime); // Actually move the character
    }

    private void hideOtherHotbar() {
        // transform.Find(hotbarPath).gameObject.SetActive(false);
    }

    private void handlePause() {
        if (Input.GetKeyDown(pauseKey)) pauseManager.togglePause();
    }

    private bool isAtEdgeAndCrouching() {
        Vector3 rayDirection = transform.forward;
        rayDirection.y -= 5f;

        // Debug.DrawRay(transform.position, rayDirection, Color.green,5f);

        // Check if crouch key IS pushed, there IS a platform below the user, and there IS NOT a platform at an angle looking down directly in front of the user
        return Input.GetKey(crouchKey) && Physics.Raycast(new Ray(transform.position, Vector3.down), out RaycastHit hitGroundUnder, 1.8f) && !Physics.Raycast(new Ray(transform.position, rayDirection), out RaycastHit hitGroundFront, 3f);
    }

    private Vector3 getHorizontalMovement() {
        return (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * getSpeed();
    }

    private float getVerticalMovement() {
        // If space bar is pressed, send force upwards. Otherwise, even though the player is on the ground, set vertical movement to simulate gravity. If set to 0, controller.isGrounded does not work
        if (Input.GetKey(jumpKey)) {
            return jumpForce;
        } else {
            if (stateHandler.isPlayerAlive()) {
                return -gravity * Time.deltaTime;
            } else if (Input.GetKey(crouchKey)) {
                return -jumpForce;
            } else {
                return 0;
            }
        }
    }

    private float getGravityMovement() {
        return gravity * Time.deltaTime;
    }

    // Account for sprint and crouch
    private float getSpeed() {
        if (Input.GetKey(sprintKey)) return speedMultiplier * sprintMultiplier;
        else if (Input.GetKey(crouchKey)) return speedMultiplier * crouchSpeedMultiplier;
        else return speedMultiplier;
    }

    private void handleCrouch() {
        if (Input.GetKey(crouchKey)) {
            if (!crouching) {
                crouching = true;

                // Reduce the y value of the scale by crouchHeightMultiplier
                Vector3 scale = transform.localScale;
                scale.y = transform.localScale.y * crouchHeightMultiplier;
                transform.localScale = scale;
            }
        } else if (!Input.GetKey(crouchKey) && crouching) {
            // Check to see if user can get back up
            if (Physics.Raycast(new Ray(transform.position, Vector3.up), out RaycastHit hitUp, 1.8f)) return;

            crouching = false;

            // Return the scale back to normal
            Vector3 scale = transform.localScale;
            scale.y = transform.localScale.y / crouchHeightMultiplier;
            transform.localScale = scale;
        }
    }

    private void handleInventoryInput() {
        int activeItemSlot = inventoryManagement.activeItemSlot;
        Boolean updated = false;

        if (inventoryManagement.InventoryCount() > 0) {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
                // Forward scroll, move to the right
                activeItemSlot--;
                if (activeItemSlot < 0) activeItemSlot = inventoryManagement.InventoryCount() - 1;
                updated = true;
            } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
                // Forward scroll, move to the right
                activeItemSlot++;
                if (activeItemSlot > inventoryManagement.InventoryCount() - 1) activeItemSlot = 0;
                updated = true;
            }
        }

        // Get the input for each of the keys 1 thru 6 for hotbar slots
        if (inventoryManagement.InventoryCount() > 0 && Input.GetKey(KeyCode.Alpha1)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 1;
            updated = true;
        } else if (inventoryManagement.InventoryCount() > 1 && Input.GetKey(KeyCode.Alpha2)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 2;
            updated = true;
        } else if (inventoryManagement.InventoryCount() > 2 && Input.GetKey(KeyCode.Alpha3)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 3;
            updated = true;
        } else if (inventoryManagement.InventoryCount() > 3 && Input.GetKey(KeyCode.Alpha4)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 4;
            updated = true;
        } else if (inventoryManagement.InventoryCount() > 4 && Input.GetKey(KeyCode.Alpha5)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 5;
            updated = true;
        } else if (inventoryManagement.InventoryCount() > 5 && Input.GetKey(KeyCode.Alpha6)) {
            activeItemSlot = inventoryManagement.InventoryCount() - 6;
            updated = true;
        }

        // Actually set the held item in view
        if (updated) {
            inventoryManagement.setActiveItemSlot(activeItemSlot);

            Item item = inventoryManagement.GetItemInSlot(activeItemSlot);
            hotbarItemUpdates.highlightCurrentHotbarSlot(transform, activeItemSlot, item);
            UpdateHeldItemForAllUsersServer(item.name);
        }

        // If drop key pressed, remove active item
        if (Input.GetKey(dropKey) && !cooldowns.cooldownKeypressActive) {
            cooldowns.startKeypressCooldown();
            HandleDrop();
        }
    }

    // If actual item in the slot, drop it!
    private void HandleDrop() {
        if (inventoryManagement.activeItem != null) {
            mapHandler.CreateItem(transform.position + transform.forward * 3, 1, inventoryManagement.activeItem.name.ToLower()); // Create the dropped item in front of the player
            
            removeHeldItemFromInventory();
        }
    }

    public void removeHeldItemFromInventory() {
        inventoryManagement.Remove(inventoryManagement.activeItem.name, 1); // Remove the item from their inventory

        // Update held item to either the next item in the players inventory, or null if none left
        string itemName = (inventoryManagement.activeItem != null) ? inventoryManagement.activeItem.name : null;
        UpdateHeldItemForAllUsersServer(itemName);
    }

    // Spawn player prefab
    public void OnServerAddPlayer(NetworkConnection conn, System.Guid playerControllerId) {
        if (!isServer) {
            GameObject player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
            player.layer = LayerMask.NameToLayer("Players");
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }

    void updateTimerPosition() {
        // Set timer to top right
        string timerPath = "Timer/Timer Canvas/Timer Text";
        GameObject.Find(timerPath).transform.position = new Vector3(Screen.width - 50, Screen.height - 50, 0);
    }

    // Handle picking up items, detects every frame that player touches an item
    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "Item") {
            if (!cooldowns.cooldownInteractActive && isLocalPlayer && stateHandler.isPlayerAlive()) {
                cooldowns.startInteractCooldown();

                string itemName = collider.gameObject.name;

                bool result = inventoryManagement.Add(itemName, 1, collider.gameObject.GetComponent<GenericItem>());

                if (result) {
                    // If this is the first item that the user has...
                    if (inventoryManagement.InventoryCount() == 1) {
                        UpdateHeldItemForAllUsersServer(itemName); // Update held item cuz user have to hold it. Otherwise, don't need to update held item
                        inventoryManagement.setActiveItemSlot(0);
                    }

                    DestroyItemServer(collider.gameObject);
                }
            }
        }
    }

    [Command]
    void DestroyItemServer(GameObject item) {
        NetworkServer.Destroy(item);
    }

    [Command]
    void UpdateHeldItemForAllUsersServer(string itemName) {
        GameObject heldItemDisplayOriginal = null;
        //https://answers.unity.com/questions/47989/is-it-possible-to-findwithtag-only-within-children.html
        foreach (Transform child in transform.Find("Held Item")) if (child.CompareTag("Held Item Display")) heldItemDisplayOriginal = child.gameObject;

        if (heldItemDisplayOriginal != null) {
            // If held item display already exists. Use Contains() because the held item display's name includes "(Clone)" at the end
            if (itemName != null && heldItemDisplayOriginal.name.ToLower().Contains(itemName.ToLower())) return; // If adding a new item and the item is the same as it already is, just ignore
            else {
                // If the item is different, destroy it to get ready for the new one incoming. If nothing's incoming, this acts as removing the held item display.
                NetworkServer.Destroy(heldItemDisplayOriginal);
            }
        }
        
        if (itemName != null) {
            // If item name provided, set held item.
            GameObject heldItemDisplayPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/HeldItems/" + itemName + ".prefab", typeof(GameObject));
            GameObject heldItemDisplay = Instantiate(heldItemDisplayPrefab);

            NetworkServer.Spawn(heldItemDisplay);
            heldItemDisplay.transform.SetParent(transform.Find("Held Item"));

            // Reset transform properties of the held item so that it goes where it's supposed to
            heldItemDisplay.transform.localPosition = Vector3.zero;
            heldItemDisplay.transform.localRotation = Quaternion.identity;
            heldItemDisplay.transform.localScale = Vector3.one;

            UpdateHeldItemForAllUsersClient(heldItemDisplay);
        }
    }

    [ClientRpc]
    void UpdateHeldItemForAllUsersClient(GameObject heldItemDisplay) {
        heldItemDisplay.transform.SetParent(transform.Find("Held Item"));

        // Reset transform properties for all clients too
        heldItemDisplay.transform.localPosition = Vector3.zero;
        heldItemDisplay.transform.localRotation = Quaternion.identity;
        heldItemDisplay.transform.localScale = Vector3.one;
    }

    [Command]
    void ChangePlayerGameObjectNameServer(string oldName, string newName) {
        GameObject.Find(oldName).name = newName;
        ChangePlayerGameObjectNameClient(oldName, newName);
    }

    [ClientRpc]
    void ChangePlayerGameObjectNameClient(string oldName, string newName) {
        GameObject.Find(oldName).name = newName;
    }

    private void setHUDItems() {
        hotbarItemUpdates.setHotbarPosition(transform, inventoryManagement.activeItemSlot, inventoryManagement.InventoryCount(), inventoryManagement.GetItemInSlot(inventoryManagement.activeItemSlot));

        updateHealthBarPosition();

        updateTimerPosition();
    }

    private void showHUDItems() {
        GameObject.Find("Health/Health Canvas/Health Bar").SetActive(true);
    }

    private void hideHUDItems() {
        inventoryManagement.clearInventory();
        GameObject.Find("Health/Health Canvas/Health Bar").SetActive(false);
    }

    private void updateHealthBarPosition() {
        // Set health bar to bottom left
        healthBehavior = GameObject.Find("Health/Health Canvas/Health Bar").GetComponent<HealthBehavior>();

        float healthBarWidth = healthBehavior.GetComponent<RectTransform>().sizeDelta.x;
        float hotbarHeight = transform.Find("Hotbar/Background Canvas/Background").GetComponent<RectTransform>().sizeDelta.y;

        healthBehavior.transform.position = new Vector3(Screen.width - healthBarWidth / 2, hotbarHeight, 0);
    }

    // If player is further than defined distance from the center of the map, automatically send player back to center and send them to spectator mode
    private void checkIfTooFar() {
        if (Vector3.Distance(transform.position, new Vector3(0, 0, 0)) > destroyDistance) {
            controller.transform.position = new Vector3(0, 25, 0);
            stateHandler.enableSpectatorState();
        }
    }
}
