using UnityEngine;

public class Shipment : MonoBehaviour
{
    Clickable clickable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickable = GetComponent<Clickable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        UIManager.Instance.OpenShipment();
        clickable.DestroySignifier();
        Destroy(gameObject);
    }
}
