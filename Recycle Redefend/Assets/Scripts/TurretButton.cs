using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [SerializeField] TurretRecipeSO turretRecipe;
    [SerializeField] Image baseImage, pivotImage;
    [SerializeField] UnityEvent<TurretRecipeSO> onOptionSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseImage.sprite = turretRecipe.BaseSprite;
        pivotImage.sprite = turretRecipe.PivotSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectOption()
    {
        onOptionSelected?.Invoke(turretRecipe);
    }
}
