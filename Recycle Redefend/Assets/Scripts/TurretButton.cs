using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [SerializeField] AudioClip clickAudioClip;
    [SerializeField] TurretRecipeSO turretRecipe;
    [SerializeField] Color highlightColor;
    [SerializeField] Image baseImage, pivotImage, backgroundImage;
    [SerializeField] UnityEvent<TurretRecipeSO, int> onOptionSelected;

    int index;

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

    public void Initialize(int index)
    {
        this.index = index;
    }

    public void Highlight(int index)
    {
        if (this.index == index)
        {
            backgroundImage.color = highlightColor;
        }
        else
        {
            backgroundImage.color = Color.white;
        }
    }

    public void SelectOption()
    {
        SoundEffectsManager.Instance.PlaySoundEffect(clickAudioClip, transform.position, 1);
        onOptionSelected?.Invoke(turretRecipe, index);
    }
}
