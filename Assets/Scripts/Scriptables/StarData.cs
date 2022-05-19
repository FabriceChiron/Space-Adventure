using UnityEngine;

public enum StarType
{
    SunLike,
    HotBlue,
    OrangeDwarf,
    RedDwarf,
    RedGiant,
    WhiteDwarf,
    NeutronStar,
    BlackHole,
    Pulsar
}


[CreateAssetMenu]
public class StarData : ScriptableObject
{
    public string Name;

    //Descirption of the planet
    public string StarDescription;

    public StarType starType;

    public GameObject Prefab;

    //Material file for planet
    public Material Material;

    //Orbit in UA
    public float Orbit;

    //Tilt (in degrees)
    public float OrbitTilt;


    //Size (relative to Earth)
    public float Size;

    //Year length in Earth years
    public float YearLength;

    //Coordinates of planet on orbit plane (e.g "nw")
    public string Coords;

    public PlanetData[] ChildrenItem;

    public AsteroidBeltData[] AsteroidBeltItem;

}
