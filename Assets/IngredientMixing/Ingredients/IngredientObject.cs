using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Ingredient")]
public class IngredientObject : ScriptableObject
{
    public Sprite sprite;
    public string ingredientName;
}
