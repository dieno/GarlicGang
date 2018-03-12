using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum Sex
{
    Female, Male
}

public class PlayerModel
{
    public Sex Gender { get; set; } //triggered
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }

    //really need to reimplement inventory
    public Dictionary<string, int> Inventory;


    public PlayerModel()
    {
        Inventory = new Dictionary<string, int>();

        //defaults
        Gender = Sex.Female;
        MaxHealth = 100;
        Health = 100;
        Level = 1;
        XP = 0;
    }

    public int CountItem(string item)
    {
        if(Inventory.ContainsKey(item))
        {
            return Inventory[item];
        }

        return 0;
    }

    public bool UseItem(string item)
    {
        if(Inventory.ContainsKey(item))
        {
            if(Inventory[item] > 0)
            {
                Inventory[item]--;
                return true;
            }
        }

        return false;
    }

    public void AddItem(string item)
    {
        AddItem(item, 1);
    }

    public void AddItem(string item, int quantity)
    {
        if(!Inventory.ContainsKey(item))
        {
            Inventory.Add(item, 0);
        }

        Inventory[item] += quantity;

        //clamp negative inventory values
        if (Inventory[item] < 0)
            Inventory[item] = 0;
    }


    public T GetAV<T>(string key)
    {
        key = key.ToLowerInvariant();

        object value = null;

        switch(key)
        {
            case "health":
                value = Health;
                break;
            case "maxHealth":
                value = MaxHealth;
                break;
            case "level":
                value = Level;
                break;
            case "xp":
                value = XP;
                break;
            case "gender":
                value = Gender;
                break;
            default:
                throw new KeyNotFoundException();
        }

        if(typeof(T) == typeof(float))
        {
            float fvalue = Convert.ToSingle(value);
            return (T)(object)fvalue;
        }
        else if(typeof(T) == typeof(int))
        {
            float ivalue = Convert.ToInt32(value);
            return (T)(object)ivalue;
        }
        else if(typeof(T) == typeof(string))
        {
            string svalue = (string)value;
            return (T)(object)svalue;
        }
        else
        {
            //fuck it
            return (T)value;
        }
    }

    //and the award for "misuse of generics" goes to...
    public void SetAV<T>(string key, T value)
    {
        key = key.ToLowerInvariant();

        switch (key)
        {
            case "health":
                Health = Convert.ToSingle(value);
                break;
            case "maxHealth":
                MaxHealth = Convert.ToSingle(value);
                break;
            case "level":
                if (IsKindaInt<T>())
                    Level = (int)(object)value;
                else
                    Level = (int)Convert.ToSingle(value);
                break;
            case "xp":
                if (IsKindaInt<T>())
                    XP = (int)(object)value;
                else
                    XP = (int)Convert.ToSingle(value);
                break;
            case "gender":
                if (typeof(T) == typeof(Sex))
                    Gender = (Sex)(object)value;
                else if (IsKindaInt<T>())
                    Gender = (Sex)(int)(object)value;
                else
                    throw new NotSupportedException();
                break;
            default:
                throw new KeyNotFoundException();
        }
    }

    

    public void ModAV<T>(string key, T value)
    {
        key = key.ToLowerInvariant();

        switch (key)
        {
            case "health":
                Health += Convert.ToSingle(value);
                break;
            case "maxHealth":
                MaxHealth += Convert.ToSingle(value);
                break;
            case "level":
                if (IsKindaInt<T>())
                    Level += (int)(object)value;
                else
                    Level += (int)Convert.ToSingle(value);
                break;
            case "xp":
                if (IsKindaInt<T>())
                    XP += (int)(object)value;
                else
                    XP += (int)Convert.ToSingle(value);
                break;
            default:
                throw new KeyNotFoundException();
        }
    }

    private static bool IsKindaInt<T>()
    {
        return typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(byte) || typeof(T) == typeof(char) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort);
    }

}

