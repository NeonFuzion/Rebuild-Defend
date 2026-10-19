using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] float changeSpeed;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI amountText;

    int targetAmount;
    float changeTime, currentAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (changeTime >= 1) return;
        changeTime += changeSpeed * Time.deltaTime;
        currentAmount = Mathf.Lerp(currentAmount, targetAmount, changeSpeed);
        amountText.SetText(Mathf.RoundToInt(currentAmount) + "");
    }

    public void UpdateAmount(int newAmount)
    {
        targetAmount = newAmount;
        changeTime = 0;
    }

    public void Initialize(Sprite sprite, int amount)
    {
        currentAmount = amount;
        targetAmount = amount;
        changeTime = 1;

        amountText.SetText(amount + "");
        itemImage.sprite = sprite;
    }
}
