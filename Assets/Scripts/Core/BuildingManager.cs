using UnityEngine;
using System.Linq; 

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    private BuildingData currentBuildingToPlace;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

  public void SelectBuilding(BuildingData data)
{
    int currentWood = ResourceManager.Instance.GetResourceAmountByName("Wood");
    int currentStone = ResourceManager.Instance.GetResourceAmountByName("Stone");

    if (currentWood >= data.woodCost && currentStone >= data.stoneCost)
    {
        currentBuildingToPlace = data;
        Debug.Log(data.buildingName + " inşaatı hazır!");
    }
    else
    {
        Debug.Log($"Yetersiz! Gereken: {data.woodCost} Odun, Elindeki: {currentWood}");
    }
}
    void Update()
    {
        if (currentBuildingToPlace != null && Input.GetMouseButtonDown(0))
        {
            PlaceBuilding();
        }
    }

    void PlaceBuilding()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(mousePos);
        spawnPos.z = 0;

        Instantiate(currentBuildingToPlace.buildingPrefab, spawnPos, Quaternion.identity);

        var wood = ResourceManager.Instance.availableResources.Find(x => x.resourceName == "Wood");
        var stone = ResourceManager.Instance.availableResources.Find(x => x.resourceName == "Stone");

        ResourceManager.Instance.AddResource(wood, -currentBuildingToPlace.woodCost);
        if (stone != null) ResourceManager.Instance.AddResource(stone, -currentBuildingToPlace.stoneCost);
        
        currentBuildingToPlace = null;
    }
}