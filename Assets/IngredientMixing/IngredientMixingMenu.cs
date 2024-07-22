using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMixingMenu : MonoBehaviour
{
    public bool isActive;

    List<IngredientObject> unlockedIngredients;
    public List<IngredientObject> currentIngredients;

    List<LightSource.LIGHT_TYPE> unlockedLightTypes;
    // Start is called before the first frame update

    [SerializeField]
    GameObject unlockedIngredientGrid, currentIngredientsGrid, unlockedLightGrid, IngredientUIButtonPrefab, FireUIButtonPrefab;

    [Header("Recipies")]
    [SerializeField]
    List<Recipe> recipes;

    [SerializeField]
    LightSource playerLantern;

    void Start()
    {
        isActive = false;
        unlockedIngredients = new List<IngredientObject>();
        currentIngredients = new List<IngredientObject>();

        unlockedLightTypes = new List<LightSource.LIGHT_TYPE>();
        UnlockLight(LightSource.LIGHT_TYPE.mundane);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockIngredient(IngredientObject ingredient)
    {
        if (!unlockedIngredients.Contains(ingredient))
        {
            unlockedIngredients.Add(ingredient);

            GameObject newButton = Instantiate(IngredientUIButtonPrefab, unlockedIngredientGrid.transform);
            IngredientUIButton uiButton = newButton.GetComponent<IngredientUIButton>();
            uiButton.SetIngredient(ingredient);

            newButton.GetComponent<Button>().onClick.AddListener(() => AddIngredientToMix(ingredient));
        }
    }

    public void UnlockLight(LightSource.LIGHT_TYPE lightType)
    {
        if (!unlockedLightTypes.Contains(lightType)) 
        { 
            unlockedLightTypes.Add(lightType);

            GameObject newButton = Instantiate(FireUIButtonPrefab, unlockedLightGrid.transform);
            FireUIButton uiButton = newButton.GetComponent<FireUIButton>();
            uiButton.SetFireType(lightType);

            newButton.GetComponent<Button>().onClick.AddListener(() => playerLantern.changeLightType(lightType));
        }
    }

    public void AddIngredientToMix(IngredientObject ingredient)
    {
        currentIngredients.Add(ingredient);

        GameObject newButton = Instantiate(IngredientUIButtonPrefab, currentIngredientsGrid.transform);
        IngredientUIButton uiButton = newButton.GetComponent<IngredientUIButton>();
        uiButton.SetIngredient(ingredient);

        newButton.GetComponent<Button>().onClick.AddListener(() => RemoveIngredientFromMix(uiButton));
    }

    public void RemoveIngredientFromMix(IngredientUIButton button)
    {
        currentIngredients.Remove(button.ingredient);

        Destroy(button.gameObject);
    }

    public void attemptToCreateFuel()
    {
        foreach(Recipe recipe in recipes)
        {
            if(recipe.ingredientOrder.Count == currentIngredients.Count)
            {
                bool matching = true;
                for(int i = 0; i < currentIngredients.Count; i++)
                {
                    if(currentIngredients[i].ingredientName != recipe.ingredientOrder[i].ingredientName)
                    {
                        matching = false;
                    }
                }
                if (matching == true)
                {
                    UnlockLight(recipe.type);
                    return;
                }
            }
        }
    }

}
