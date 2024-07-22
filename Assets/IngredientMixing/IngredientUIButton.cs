using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientUIButton : MonoBehaviour
{
    public IngredientObject ingredient;

    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetIngredient(IngredientObject ingredientObject)
    {
        ingredient = ingredientObject;
        GetComponent<Image>().sprite = ingredientObject.sprite;
    }

}

