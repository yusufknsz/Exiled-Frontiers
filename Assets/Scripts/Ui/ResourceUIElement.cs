using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIElement : MonoBehaviour
{
    public ResourceData targetResource; 
    public TextMeshProUGUI amountText;

    void Start()
    {
        // Başlangıçta mevcut değeri çek
        UpdateText(targetResource, ResourceManager.Instance.GetResourceAmountByName(targetResource.resourceName));
    }

    void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += UpdateText;
        }
    }

    void OnDisable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= UpdateText;
        }
    }

    private void UpdateText(ResourceData data, int amount)
    {
        if (targetResource != null && data == targetResource && amountText != null)
        {
            amountText.text = amount.ToString();
        }
    }
}