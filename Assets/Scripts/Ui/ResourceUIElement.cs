using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIElement : MonoBehaviour
{
    public ResourceData targetResource; 
    public TextMeshProUGUI amountText;

    void Update()
    {
        if (ResourceManager.Instance != null && amountText != null && targetResource != null)
        {
            int currentAmount = ResourceManager.Instance.GetResourceAmountByName(targetResource.resourceName);            amountText.text = currentAmount.ToString();
        }
    }
}