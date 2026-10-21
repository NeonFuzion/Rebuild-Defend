using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurretButton : MonoBehaviour
{
    [SerializeField] AudioClip clickAudioClip;
    [SerializeField] Color highlightColor;
    [SerializeField] Image baseImage, pivotImage, backgroundImage;
    [SerializeField] UnityEvent<int> onOptionSelected;

    int index;

    UIButton uiButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectOption()
    {
        SoundEffectsManager.Instance.PlaySoundEffect(clickAudioClip, transform.position, 1);
        onOptionSelected?.Invoke(index);
    }

    public void Initialize(int index, TurretRecipeSO turretRecipe, UnityAction<int> onClick)
    {
        this.index = index;

        onOptionSelected?.AddListener(onClick);
        
        uiButton = GetComponent<UIButton>();
        uiButton.Initialize();
        uiButton.OnClick.AddListener(SelectOption);
        uiButton.OnEnter.AddListener(() => BattleManager.Instance.InvokeOnTurretHoverEnterEvent(index));
        uiButton.OnExit.AddListener(BattleManager.Instance.InvokeOnTurretHoverExitEvent);

        Transform spriteBase = turretRecipe.PrefabTurret.transform;
        pivotImage.sprite = spriteBase.GetChild(1).GetComponentInChildren<SpriteRenderer>().sprite;
        baseImage.sprite = spriteBase.GetComponentInChildren<SpriteRenderer>().sprite;
        pivotImage.SetNativeSize();
        baseImage.SetNativeSize();
    }

    public void Highlight(int index)
    {
        uiButton.Highlight(this.index == index);
    }
}
