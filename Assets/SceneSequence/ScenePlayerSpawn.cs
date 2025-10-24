using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;

    private GameObject currentPlayer;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        // Destroy any existing player (for safety if you restart)
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // Determine spawn position
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        // Instantiate player prefab
        currentPlayer = Instantiate(playerPrefab, spawnPos, spawnRot);

        // Make sure this camera becomes the active one
        Camera playerCamera = currentPlayer.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            // Disable all other cameras first
            foreach (var cam in Camera.allCameras)
                if (cam != playerCamera)
                    cam.enabled = false;

            playerCamera.enabled = true;
        }
    }
}

