using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] float changeSpeed;
    [SerializeField] Image itemImage;
    [SerializeField] Color insufficientColor = Color.red, sufficientColor = Color.black;
    [SerializeField] TextMeshProUGUI amountText;

    int updateAnimHash = Animator.StringToHash("ItemUpdate");

    int targetAmount;
    float changeTime, currentAmount;
    string costText;

    Animator animator;

    public int Amount => targetAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < changeTime) return;
        if (currentAmount == targetAmount) return;
        changeTime = Time.time + changeSpeed;
        currentAmount += Mathf.Sign(targetAmount - currentAmount);
        UpdateText();
    }

    void UpdateText()
    {
        amountText.SetText(costText + Mathf.RoundToInt(currentAmount));
        animator.CrossFade(updateAnimHash, 0, 0);
    }

    public void UpdateAmount(int newAmount)
    {
        targetAmount = newAmount;
        changeTime = 0;
    }

    public void SetCost(int cost = -1)
    {
        costText = cost == -1 ? "" : cost + "/";
        amountText.color = cost > targetAmount ? insufficientColor : sufficientColor;
        UpdateText();
    }

    public virtual void Initialize(ItemSO item, int amount)
    {
        currentAmount = amount;
        targetAmount = amount;
        changeTime = 1;

        amountText.SetText(amount + "");
        itemImage.sprite = item.Sprite;
    }
}
