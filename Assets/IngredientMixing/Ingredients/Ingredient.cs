using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField]
    public IngredientObject ingredientObject;

    float floatingTimer, startingYValue;

    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = ingredientObject.sprite;
        startingYValue = transform.position.y;
        floatingTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        floatingTimer += Time.deltaTime;
        transform.position = new Vector3(transform.position.x, startingYValue + Mathf.Sin(floatingTimer * 2) * 0.05f, transform.position.z);
    }

    public IngredientObject Pickup()
    {
        IngredientObject ingredient = ingredientObject;
        GameObject.Destroy(gameObject);
        return ingredient;
    }
}
