using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    
    // UI güncellemeleri için Event
    public Action<ResourceData, int> OnResourceChanged;

    
    private Dictionary<ResourceData, int> resourceStocks = new Dictionary<ResourceData, int>();
    public List<ResourceData> availableResources;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        foreach (var resource in availableResources)
        {
            if (resource != null && !resourceStocks.ContainsKey(resource))
            {
                resourceStocks.Add(resource, resource.resourceAmount);
            }
        }
    }

    public int GetResourceAmountByName(string name)
    {
        var data = availableResources.FirstOrDefault(x => x.resourceName == name);
        if (data != null && resourceStocks.ContainsKey(data))
        {
            return resourceStocks[data];
        }
        return 0;
    }

    public void AddResource(ResourceData data, int amount)
    {
        if (data != null)
        {
            if (!resourceStocks.ContainsKey(data)) resourceStocks.Add(data, 0);
            resourceStocks[data] += amount;
            Debug.Log($"{data.resourceName} yeni miktar: {resourceStocks[data]}");
            OnResourceChanged?.Invoke(data, resourceStocks[data]);
        }
    }

    public void RemoveResourceByName(string name, int amount)
    {
        var data = availableResources.FirstOrDefault(x => x.resourceName == name);
        if (data != null && resourceStocks.ContainsKey(data))
        {
            resourceStocks[data] -= amount;
            OnResourceChanged?.Invoke(data, resourceStocks[data]);
        }
    }

    public List<ResourceSaveData> GetResourcesSaveData()
    {
        var list = new List<ResourceSaveData>();
        foreach(var kv in resourceStocks)
        {
            list.Add(new ResourceSaveData { resourceName = kv.Key.resourceName, amount = kv.Value });
        }
        return list;
    }

    public void LoadResources(List<ResourceSaveData> loadedData)
    {
        foreach(var data in loadedData)
        {
            var resData = availableResources.FirstOrDefault(x => x.resourceName == data.resourceName);
            if (resData != null)
            {
                if (!resourceStocks.ContainsKey(resData)) resourceStocks.Add(resData, 0);
                
                resourceStocks[resData] = data.amount;
                OnResourceChanged?.Invoke(resData, data.amount);
            }
        }
    }
}