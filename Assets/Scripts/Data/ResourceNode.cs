using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceData resourceType; 
    public int amount = 10;           
    public float gatherTime = 2f;    

    public void OnMouseDown()
    {
        SettlerAI settler = FindFirstObjectByType<SettlerAI>();
        if (settler != null)
        {
            settler.StartHarvesting(this);
        }
    }

    public void Gather()
    {
        if (ResourceManager.Instance != null && resourceType != null)
        {
            ResourceManager.Instance.AddResource(resourceType, amount);
        }
        
        Destroy(gameObject);
        Debug.Log(amount + " adet " + resourceType.resourceName + " toplandı!");
    }
}