using UnityEngine;

public class Shipment : MonoBehaviour
{
    [SerializeField] float pointerFadeDistance = 1, pointerMaxY, pointerMinY, pointerMaxX, pointerMinX;

    Clickable clickable;
    RectTransform pointer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clickable = GetComponent<Clickable>();
        pointer = UIManager.Instance.RetrievePointer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pointer) return;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector2 position = new Vector3(
            Mathf.Clamp(
                transform.position.x,
                cameraPosition.x + pointerMinX * Camera.main.orthographicSize,
                cameraPosition.x + pointerMaxX * Camera.main.orthographicSize
                ),
            Mathf.Clamp(
                transform.position.y,
                cameraPosition.y + pointerMinY * Camera.main.orthographicSize, 
                cameraPosition.y + pointerMaxY * Camera.main.orthographicSize
                )
        );
        pointer.anchoredPosition = position;

        Vector2 positionDelta = cameraPosition - transform.position;
        pointer.eulerAngles = Vector3.forward * Mathf.Atan2(positionDelta.y, positionDelta.x) * 180 / Mathf.PI;

        if (Vector2.Distance(position, transform.position) > pointerFadeDistance) return;
        UIManager.Instance.ReturnPointer(pointer);
    }

    public void OnClick()
    {
        UIManager.Instance.OpenShipment();
        clickable.DestroySignifier();
        UIManager.Instance.ReturnPointer(pointer);
        Destroy(gameObject);
    }
}
