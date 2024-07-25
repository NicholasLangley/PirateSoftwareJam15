using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeUnlocker : MonoBehaviour
{
    float floatingTimer, startingYValue;

    [SerializeField]
    IngredientMixingMenu menu;

    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        startingYValue = transform.position.y;
        floatingTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        floatingTimer += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, startingYValue + Mathf.Sin(floatingTimer * 2) * 0.05f, transform.position.z);
    }

    public void Pickup()
    {
        menu.UnlockGrenades();
        GameObject.Destroy(gameObject);
    }
}
