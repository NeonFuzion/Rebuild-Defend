using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] AudioClip clickAudioClip;
    [SerializeField] Color highlightColor;
    [SerializeField] Image baseImage, pivotImage, backgroundImage;
    [SerializeField] UnityEvent<int> onOptionSelected;

    int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int index, TurretRecipeSO turretRecipe, UnityAction<int> onClick)
    {
        this.index = index;
        onOptionSelected?.AddListener(onClick);

        Transform spriteBase = turretRecipe.PrefabTurret.transform;
        pivotImage.sprite = spriteBase.GetChild(1).GetComponentInChildren<SpriteRenderer>().sprite;
        baseImage.sprite = spriteBase.GetComponentInChildren<SpriteRenderer>().sprite;
        pivotImage.SetNativeSize();
        baseImage.SetNativeSize();
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
        onOptionSelected?.Invoke(index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BattleManager.Instance.InvokeOnTurretHoverEnterEvent(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BattleManager.Instance.InvokeOnTurretHoverExitEvent();
    }
}
