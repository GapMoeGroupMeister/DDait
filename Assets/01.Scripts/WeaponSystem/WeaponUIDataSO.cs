using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;
using Crogen.AttributeExtension;

[Serializable]
public struct WeaponUIData
{
    public Sprite icon;
    public string weaponName;
    [TextArea]
    public string description;
    public bool isUniqueWeapon;
    public PlayerPartType parentPart;
}

[CreateAssetMenu(menuName = "SO/Weapon/WeaponUIData")]
public class WeaponUIDataSO : ScriptableObject
{
    public WeaponUIData this[WeaponType key]
    {
        get => this.weaponDataDictionary[key];
        set => this.weaponDataDictionary[key] = value;
    }
    public SerializedDictionary<WeaponType, WeaponUIData> weaponDataDictionary;

    private void Reset()
    {
        weaponDataDictionary = new SerializedDictionary<WeaponType, WeaponUIData>();
        foreach (WeaponType value in Enum.GetValues(typeof(WeaponType)))
        {
            weaponDataDictionary.Add(value, new WeaponUIData());
        }
    }
}
