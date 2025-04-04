using System;
using System.Collections.Generic;
using UnityEngine;
using EasySave.Json;
using System.IO;
using UnityEngine.Events;
public class GameDataManager : MonoSingleton<GameDataManager>
{
    [SerializeField] private WeaponUIDataSO _weaponUIDataSO;
    [SerializeField] private TechTreeSO _treeSO;

    private string _path = "GameData";

    private int coin = 0;
    [HideInInspector] public List<PartSave> parts;
    private List<WeaponSave> weapons;

    public int Coin => coin;

    public UnityEvent<int> onCoinChange;
    public Action OnGatherCoin;
    public Action OnUseCoin;

    protected override void Awake()
    {
        base.Awake();
        //DontDestroyOnLoad(gameObject);
        Load();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            AddCoin(100);
        }
    }

    #region GetDatas

    public bool TryGetPart(PlayerPartType partType, out PartSave part)
    {
        foreach(var p in parts)
        {
            if(p.id == (int)partType)
            {
                part = p;
                return true;
            }
        }

        part = null;
        return false;
    }

    public bool TryGetWeapon(WeaponType weaponType, out WeaponSave weapon)
    {
        foreach (var w in weapons)
        {
            if (w.id == (int)weaponType)
            {
                weapon = w;
                return true;
            }
        }

        weapon = null;
        return false;
    }

    public void SetParts(List<Node> parts)
    {
        foreach(var p in parts)
        {
            PartNodeSO partSO = p.node as PartNodeSO;

            this.parts.ForEach(part =>
            {
                if(part.id == (int)partSO.openPart)
                {
                    Debug.Log(part.id);
                    part.enabled = p.IsNodeEnable;
                }
            });
        }
    }

    public void SetWeapons(List<Node> weapons)
    {
        foreach (var w in weapons)
        {
            WeaponNodeSO weaponSO = w.node as WeaponNodeSO;

            this.weapons.ForEach(weapon =>
            {
                if (weapon.id == (int)weaponSO.weapon)
                {
                    Debug.Log(weapon.id);
                    weapon.enabled = w.IsNodeEnable;
                }
            });
        }
    }

    #endregion


    #region Coin

    /// <summary>
    /// ���� ������ �Ծ��� �� �߱���� ȿ���� �� ���� ���ݾ�?
    /// </summary>
    /// <param name="value"></param>
    public void AddCoin(int value)
    {
        OnGatherCoin?.Invoke(); 
        coin += value;
        Save();

        onCoinChange?.Invoke(coin);
    }

    /// <summary>
    /// ���� ������ �Ҿ��� ���� ȿ���� ���� �� ���Ƽ� �ʿ������ ��
    /// </summary>
    /// <param name="value"></param>
    public void UseCoin(int value)
    {
        OnUseCoin?.Invoke();
        coin -= value;

        onCoinChange?.Invoke(coin);
    }

    #endregion

    #region Save&Load

    public void Save()
    {
        //�����صа� ������
        DataSave save = new DataSave();
        Debug.Log("�ֹ�");

        save.coin = coin;
        save.parts = parts;
        save.weapons = weapons;

        //�ϴ� prettyprint true�� �صΰ� ���� �Ҷ� ��
        EasyToJson.ToJson(save, _path, true);
    }

    public void Load()
    {
        string path = EasyToJson.GetFilePath(_path);// Path.Combine(Application.dataPath, "/10.Database/Json/", _path, ".json");

        
        if (!File.Exists(path))
        {
            DataSave s = new DataSave();
            foreach (var weapon in s.weapons)
            {
                bool isGraph = _treeSO.nodes.Find(x => (x as WeaponNodeSO) != null && (int)(x as WeaponNodeSO).weapon == weapon.id);
                if (!isGraph)
                {
                    weapon.enabled = true;
                }
            }
            EasyToJson.ToJson(s, _path, true);

            coin = s.coin;
            parts = s.parts;
            weapons = s.weapons;
            
            return;
        }

        DataSave save = EasyToJson.FromJson<DataSave>(_path);

        coin = save.coin;
        parts = save.parts;
        weapons = save.weapons;
        onCoinChange?.Invoke(coin);
    }

    public void EnablePart(PlayerPartType openPart)
    {
        parts.ForEach((part) =>
        {
            if (part.id == (int)openPart)
                part.enabled = true;
        });

        Save();
    }

    public void EnableWeapon(WeaponType openWeapon)
    {
        weapons.ForEach((weapon) =>
        {
            if (weapon.id == (int)openWeapon)
            {
                weapon.enabled = true;
                if (_weaponUIDataSO[openWeapon].isUniqueWeapon) return;

                int parentPartId = (int)_weaponUIDataSO[openWeapon].parentPart;
                parts.ForEach((part) =>
                {
                    if (part.id == parentPartId)
                        part.level++;
                });
            }
        });

        Save();
    }

    #endregion
}

[Serializable]
public class DataSave
{
    public int coin;
    public List<PartSave> parts;
    public List<WeaponSave> weapons;

    public DataSave()
    {
        coin = 0;
        parts = new List<PartSave>();
        foreach (var partEnum in Enum.GetValues(typeof(PlayerPartType)))
        {
            PartSave part = new PartSave();
            part.id = (int)partEnum;
            part.enabled = (PlayerPartType)partEnum == PlayerPartType.TITAN;
            part.level = 0;

            parts.Add(part);
        }

        weapons = new List<WeaponSave>();
        foreach (var weaponEnum in Enum.GetValues(typeof(WeaponType)))
        {
            WeaponSave weapon = new WeaponSave();
            weapon.id = (int)weaponEnum;
            weapon.enabled = false;

            weapons.Add(weapon);
        }
    }
}

[Serializable]
public class WeaponSave
{
    public int id;
    public bool enabled = true;
}

[Serializable]
public class PartSave
{
    public int id;
    public int level;
    public bool enabled;
}