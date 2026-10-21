using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public UnityEvent OnEnter { get; private set; }
    [field: SerializeField] public UnityEvent OnExit { get; private set; }
    [field: SerializeField] public UnityEvent OnClick { get; private set; }

    int clickAnimHash = Animator.StringToHash("ButtonClick"),
        enterAnimHash = Animator.StringToHash("ButtonEnter"),
        exitAnimHash = Animator.StringToHash("ButtonExit"),
        highlightAnimHash = Animator.StringToHash("ButtonHighlight"),
        unhighlightAnimHash = Animator.StringToHash("ButtonUnhighlight");

    bool isHighlighted;

    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (isHighlighted) return;
        animator.CrossFade(clickAnimHash, 0, 0);
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (isHighlighted) return;
        animator.CrossFade(enterAnimHash, 0, 0);
        OnEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (isHighlighted) return;
        animator.CrossFade(exitAnimHash, 0, 0);
        OnExit?.Invoke();
    }

    public void Initialize()
    {
        animator = GetComponent<Animator>();
    }

    public void Highlight(bool isHighlighted)
    {
        if (isHighlighted && !this.isHighlighted)
        {
            animator.CrossFade(highlightAnimHash, 0, 0);
        }
        else if (!isHighlighted && this.isHighlighted)
        {
            animator.CrossFade(unhighlightAnimHash, 0, 0);
        }

        this.isHighlighted = isHighlighted;
    }
}
