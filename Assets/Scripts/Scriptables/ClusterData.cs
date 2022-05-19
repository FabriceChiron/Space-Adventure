using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ClusterData : ScriptableObject
{
    public string Name;

    public float Top;

    public float Left;

    //Texture file for planet
    public Texture Texture;

    public StellarSystemData[] StellarSystemItem;

}
