using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Scales : ScriptableObject
{

    public float Orbit = 10f;
    public float Planet = 1f;
    public float Year = 30f;
    public float Day = 10f;
    public bool RationalizeValues = true;
}
