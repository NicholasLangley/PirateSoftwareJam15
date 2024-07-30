using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientMixingMenu : Menu
{
    List<IngredientObject> unlockedIngredients;
    public List<IngredientObject> currentIngredients;

    List<LightSource.LIGHT_TYPE> unlockedLightTypes;
    List<LightSource.LIGHT_TYPE> unlockedGrenadeTypes;
    // Start is called before the first frame update

    [SerializeField]
    GameObject unlockedIngredientGrid, currentIngredientsGrid, unlockedLightGrid, grenadeGrid, IngredientUIButtonPrefab, FireUIButtonPrefab, grenadeUIButtonPrefab;

    [Header("Recipies")]
    [SerializeField]
    List<Recipe> recipes;

    [SerializeField]
    LightSource playerLantern;

    bool grenadesUnlocked;

    void Start()
    {
        isActive = false;
        grenadesUnlocked = false;
        unlockedIngredients = new List<IngredientObject>();
        currentIngredients = new List<IngredientObject>();

        unlockedLightTypes = new List<LightSource.LIGHT_TYPE>();
        //UnlockLight(LightSource.LIGHT_TYPE.black);
        

        unlockedGrenadeTypes = new List<LightSource.LIGHT_TYPE>();
        //UnlockGrenades();
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
            uiButton.x.enabled = false;
            uiButton.SetIngredient(ingredient);

            newButton.GetComponent<Button>().onClick.AddListener(() => AddIngredientToMix(ingredient));
            Navigation noNav = new Navigation();
            noNav.mode = Navigation.Mode.None;
            newButton.GetComponent<Button>().navigation = noNav;
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
            Navigation noNav = new Navigation();
            noNav.mode = Navigation.Mode.None;
            newButton.GetComponent<Button>().navigation = noNav;
        }
        if (grenadesUnlocked) { UnlockGrenadeType(lightType); }
    }

    public void UnlockGrenadeType(LightSource.LIGHT_TYPE lightType)
    {
        if (!unlockedGrenadeTypes.Contains(lightType))
        {
            unlockedGrenadeTypes.Add(lightType);

            GameObject newButton = Instantiate(grenadeUIButtonPrefab, grenadeGrid.transform);
            FireUIButton uiButton = newButton.GetComponent<FireUIButton>();
            uiButton.SetFireType(lightType);

            newButton.GetComponent<Button>().onClick.AddListener(() => playerLantern.ChangeGrenadeLightType(lightType));
            Navigation noNav = new Navigation();
            noNav.mode = Navigation.Mode.None;
            newButton.GetComponent<Button>().navigation = noNav;
        }
    }

    public void UnlockGrenades()
    {
        grenadesUnlocked = true;
        foreach (LightSource.LIGHT_TYPE type in unlockedLightTypes)
        {
            UnlockGrenadeType(type);
        }
        playerLantern.ChangeGrenadeLightType(unlockedGrenadeTypes[0]);
    }

    public void AddIngredientToMix(IngredientObject ingredient)
    {
        if (currentIngredients.Count > 12) { return; }
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

    public void AttemptToCreateFuel()
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
                    Clear();
                    return;
                }
            }
        }
    }

    public override void Clear()
    {
        currentIngredients.Clear();
        foreach(Transform child in currentIngredientsGrid.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
