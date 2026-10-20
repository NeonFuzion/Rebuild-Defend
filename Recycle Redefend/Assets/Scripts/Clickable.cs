using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    [SerializeField] Vector2 offset = new Vector2(0, 2);
    [SerializeField] GameObject prefabSignifier;
    [SerializeField] UnityEvent onClick;

    GameObject signifier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        signifier = Instantiate(prefabSignifier, UIManager.Instance.WorldCanvas);
        signifier.transform.position = (Vector2)transform.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        onClick?.Invoke();
    }

    public void DestroySignifier()
    {
        Destroy(signifier);
    }
}
