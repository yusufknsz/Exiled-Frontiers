using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "ExiledFrontiers/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public GameObject buildingPrefab; // İnşa edilecek asıl bina
    public int woodCost;              // Gereken odun
    public int stoneCost;             // Gereken taş
}