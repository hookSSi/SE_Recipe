using UnityEngine;
using TMPro;

public class Ingredient : MonoBehaviour
{
    public TMP_Text _ingredientText;

    public void Init(IngredientInfo info)
    {
        _ingredientText.text = info.IRDNT_NM + " - " + info.IRDNT_CPCTY;
    }
}
