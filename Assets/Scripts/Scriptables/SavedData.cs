using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SavedData : ScriptableObject
{
    public StellarSystemData SelectedSystem;

    public SavedStellarSystem SavedStellarSystem;

    public StellarSystemsArray StellarSystemsArray;

    public int Platinum;

    public int Turrets;
}
