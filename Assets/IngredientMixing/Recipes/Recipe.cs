using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
public class Recipe : ScriptableObject
{
    public LightSource.LIGHT_TYPE type;

    public List<IngredientObject> ingredientOrder;
}
