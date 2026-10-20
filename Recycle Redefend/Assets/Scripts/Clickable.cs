using UnityEngine;

public class Clickable : MonoBehaviour
{
    bool isMouseIn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        isMouseIn = true;
    }

    void OnMouseExit()
    {
        isMouseIn = false;
    }
}
