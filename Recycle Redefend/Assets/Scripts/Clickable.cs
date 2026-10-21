using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    [SerializeField] bool isClickable = true;
    [SerializeField] Vector2 offset = new Vector2(0, 2);
    [SerializeField] GameObject prefabSignifier;
    [SerializeField] UnityEvent onClick, onEnter, onExit;

    GameObject signifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isClickable) return;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (!isClickable) return;
        onClick?.Invoke();
    }

    void OnMouseEnter()
    {
        onEnter?.Invoke();
    }

    void OnMouseExit()
    {
        onExit?.Invoke();
    }

    void Initialize()
    {
        signifier = Instantiate(prefabSignifier, UIManager.Instance.WorldCanvas);
        signifier.transform.position = (Vector2)transform.position + offset;
    }

    public void Show()
    {
        if (!signifier) Initialize();
        isClickable = true; 
        signifier.SetActive(true);
    }

    public void Hide()
    {
        if (!signifier) return;
        isClickable = false;
        signifier.SetActive(false);
    }

    public void DestroySignifier()
    {
        Destroy(signifier);
    }
}
