using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [SerializeField]
    public IngredientObject ingredientObject;

    SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = ingredientObject.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IngredientObject Pickup()
    {
        IngredientObject ingredient = ingredientObject;
        GameObject.Destroy(gameObject);
        return ingredient;
    }
}
