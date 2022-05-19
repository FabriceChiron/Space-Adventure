using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GalaxyData : ScriptableObject
{
    public string Name;

    //Texture file for planet
    public Texture Texture;

    public ClusterData[] ClusterItem;

}
