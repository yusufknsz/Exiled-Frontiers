using UnityEngine;
using System.Linq; 

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    private BuildingData currentBuildingToPlace;

    [Header("Save System")]
    public System.Collections.Generic.List<BuildingData> buildingDatabase = new System.Collections.Generic.List<BuildingData>();
    private System.Collections.Generic.List<GameObject> placedBuildings = new System.Collections.Generic.List<GameObject>();

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
    public bool IsPlacingBuilding()
    {
        return currentBuildingToPlace != null;
    }

    void Update()
    {
        if (currentBuildingToPlace != null)
        {
            // Sağ tıkla iptal et (Planladığımız extra özellik)
            if (Input.GetMouseButtonDown(1))
            {
                currentBuildingToPlace = null;
                Debug.Log("İnşaat iptal edildi.");
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding();
            }
        }
    }

    void PlaceBuilding()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(mousePos);
        spawnPos.z = 0;

        GameObject newBuilding = Instantiate(currentBuildingToPlace.buildingPrefab, spawnPos, Quaternion.identity);
        newBuilding.name = currentBuildingToPlace.buildingName; // Kaydederken türünü bilebilmek için ismini de değiştiriyoruz
        placedBuildings.Add(newBuilding);

        // Kaynakları temiz bir şekilde eksiltelim
        ResourceManager.Instance.RemoveResourceByName("Wood", currentBuildingToPlace.woodCost);
        ResourceManager.Instance.RemoveResourceByName("Stone", currentBuildingToPlace.stoneCost);
        
        currentBuildingToPlace = null;
    }

    public System.Collections.Generic.List<BuildingSaveData> GetBuildingsSaveData()
    {
        var dataList = new System.Collections.Generic.List<BuildingSaveData>();
        foreach (var b in placedBuildings)
        {
            if (b != null)
            {
                dataList.Add(new BuildingSaveData { buildingName = b.name, position = b.transform.position });
            }
        }
        return dataList;
    }

    public void LoadBuildings(System.Collections.Generic.List<BuildingSaveData> loadedDataList)
    {
        // Öncelikler dünyadaki mevcut binaları sil
        foreach (var b in placedBuildings) 
        {
            if (b != null) Destroy(b);
        }
        placedBuildings.Clear();

        // Gelen veriye göre yeni binalar oluştur
        foreach (var data in loadedDataList)
        {
            var bData = buildingDatabase.FirstOrDefault(x => x.buildingName == data.buildingName);
            if (bData != null)
            {
                GameObject newObj = Instantiate(bData.buildingPrefab, data.position, Quaternion.identity);
                newObj.name = bData.buildingName; // İsmini tekrar aynı yapıyoruz ki bir daha kaydettiğimizde sorun olmasın
                placedBuildings.Add(newObj);
            }
            else
            {
                Debug.LogWarning("Bina bulunamadı: " + data.buildingName + " - buildingDatabase içine ekli olduğundan emin olun!");
            }
        }
    }
}