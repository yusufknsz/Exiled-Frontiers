using UnityEngine;
[CreateAssetMenu(fileName = "NewResource", menuName = "ExiledFrontiers/Resource Data")]
public class ResourceData : ScriptableObject{
    public string resourceName;
    public Sprite resourceIcon;
    public int resourceAmount;
}
