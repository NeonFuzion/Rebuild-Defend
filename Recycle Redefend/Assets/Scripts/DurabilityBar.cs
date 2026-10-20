using UnityEngine;
using UnityEngine.UI;

public class DurabilityBar : MonoBehaviour
{
    int durability;

    Image[] bars;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(int durability)
    {
        this.durability = durability;
        bars = GetComponentsInChildren<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i > durability) continue;
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UpdateDurability(int newDurability)
    {
        for (int i = durability; i > newDurability; i--)
        {
            bars[i].enabled = false;
        }
    }
}
