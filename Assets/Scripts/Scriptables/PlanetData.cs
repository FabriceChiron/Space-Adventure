using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlanetData : ScriptableObject
{
    public string Name;

    //Descirption of the planet
    public string Details;

    public GameObject Prefab;

    public bool Gaseous;

    ///<summary>Texture file (cubemap) for Gaseous planets
    ///First one: texture, Second one: flow
    /// </summary>
    [Header("Gaseous planets: 0 - texture, 1 - flow")]
    public Texture[] Texture;

    //Material file for planet
    [Header("Material for rocky planets")]
    public Material Material;

    //Texture file for clouds (if any)
    public bool Clouds;

    public Material CloudsMaterial;

    //Texture file for clouds (if any)
    public bool TidallyLocked;

    //Orbit in UA
    public float Orbit;

    //Tilt (in degrees)
    public float OrbitTilt;


    //Size (relative to Earth)
    public float Size;

    public float Mass;

    //Tilt (in degrees)
    public float BodyTilt;

    //Year length in Earth years
    public float YearLength;

    //Day length in Earth days
    public float DayLength;

    //Coordinates of planet on orbit plane (e.g "nw")
    public string Coords;

    public bool Rings;

    public GameObject RingsPrefab;

    public PlanetData[] ChildrenItem;
}
