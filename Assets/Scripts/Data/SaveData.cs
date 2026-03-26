using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public List<ResourceSaveData> resources = new List<ResourceSaveData>();
    public List<BuildingSaveData> buildings = new List<BuildingSaveData>();
}

[Serializable]
public class ResourceSaveData
{
    public string resourceName;
    public int amount;
}

[Serializable]
public class BuildingSaveData
{
    public string buildingName;
    public Vector3 position;
}
