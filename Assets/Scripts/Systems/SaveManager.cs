using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        // Oyunu oynayan oyuncunun bilgisayarında yerel bir alana kaydeder
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        
        if (ResourceManager.Instance != null)
            data.resources = ResourceManager.Instance.GetResourcesSaveData();
            
        if (BuildingManager.Instance != null)
            data.buildings = BuildingManager.Instance.GetBuildingsSaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Oyun başarıyla kaydedildi: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.LoadResources(data.resources);
                
            if (BuildingManager.Instance != null)
                BuildingManager.Instance.LoadBuildings(data.buildings);
                
            Debug.Log("Oyun yüklendi!");
        }
        else
        {
            Debug.Log("Kayıt dosyası bulunamadı: " + saveFilePath);
        }
    }

}
