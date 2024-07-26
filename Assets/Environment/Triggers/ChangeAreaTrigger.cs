using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAreaTrigger : Trigger
{
    [SerializeField]
    List<GameObject> objectsToEnable, objectsToDisable;

    [SerializeField]
    LightSource playerLantern;

    [SerializeField]
    Vector2 minCamera, maxCamera;
    [SerializeField]
    float cameraSize;

    public override void TriggerEffects(PlayerController player)
    {
        foreach(GameObject goe in objectsToEnable)
        {
            goe.SetActive(true);
        }
        foreach (GameObject god in objectsToDisable)
        {
            god.SetActive(false);
        }
        player.minCameraX = minCamera.x;
        player.maxCameraX = maxCamera.x;
        player.minCameraY = minCamera.y;
        player.maxCameraY = maxCamera.y;
        player.cameraSize = cameraSize;
        playerLantern.UpdateActiveTilemaps();

        Vector3 respawnPos = transform.position;
        respawnPos.z = player.transform.position.z;
        player.respawnPosition = respawnPos;

        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
