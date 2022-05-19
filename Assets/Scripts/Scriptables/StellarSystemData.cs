using UnityEngine;

[CreateAssetMenu]
public class StellarSystemData : ScriptableObject
{
    public string Name;

    //Descirption of the planet
    public string StarName;

    public float StarSize;

    public string StarDescription;

    public StarData[] StarsItem;

    public float ScaleFactor = 1f;

    //Texture file for star
    public Texture Texture;

    public Material Material;

    public float Top;

    public float Left;

    public PlanetData[] ChildrenItem;

    public AsteroidBeltData[] AsteroidBeltItem;

    [Header("Objectives")]
    public int Platinum;
    public int Turrets;
}
