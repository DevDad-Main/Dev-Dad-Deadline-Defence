using UnityEngine;

[CreateAssetMenu(menuName = "BuildSystem/Buildable Item")]
public class BuildableData : ScriptableObject
{
    public GameObject buildableGO;
    public int cost;
    public string displayName;
    public Sprite icon;
}