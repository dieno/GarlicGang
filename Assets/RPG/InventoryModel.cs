﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO split this up because, I mean, damn
namespace CommonCore.RPG
{
    public enum AmmoType
    {
        NoAmmo, Acp32, Spc38, Acp45, R3006       
    }

    public enum DamageType //doesn't do anything in this game
    {
        Blunt, Pierce, Energy
    }

    public enum AidType //are there even any other stats?
    {
        Health
    }

    public enum RestoreType
    {
        Add, Boost //boost allows going over max, add does not
    }

    //an actual inventory item that the player has
    public class InventoryItemInstance
    {
        public float Condition { get; set; } //it's here but basically unimplemented
        public readonly InventoryItemModel ItemModel;

        public InventoryItemInstance(InventoryItemModel model)
        {
            ItemModel = model;
            Condition = model.MaxCondition;
        }
    }

    //base class for invariant inventory items
    public abstract class InventoryItemModel
    {
        public readonly string Name;
        public readonly float Weight;
        public readonly float MaxCondition;
        public readonly bool Unique;
        public readonly bool Essential;

        public InventoryItemModel(string name, float weight, float maxCondition, bool unique, bool essential)
        {
            Name = name;
            Weight = weight;
            MaxCondition = maxCondition;
            Unique = unique;            
            Essential = essential;
        }

    }

    public class MiscItemModel : InventoryItemModel
    {
        public MiscItemModel(string name, float weight, float maxCondition, bool unique, bool essential)
            : base(name, weight, maxCondition, unique, essential)
        {
        }
    }

    public class WeaponItemModel : InventoryItemModel //yeah no melee... yet
    {
        public readonly float Damage;
        public readonly float DamagePierce;
        public readonly float Spread;
        public readonly float FireRate;
        public readonly int MagazineSize;
        public readonly float ReloadTime;
        public readonly AmmoType AType;
        public readonly DamageType DType;

        public WeaponItemModel(string name, float weight, float maxCondition, bool unique, bool essential,
            float damage, float damagePierce, float spread, float fireRate,
            int magazineSize, float reloadTime, AmmoType aType, DamageType dType)
            : base(name, weight, maxCondition, unique, essential)
        {
            Damage = damage;
            DamagePierce = damagePierce;
            Spread = spread;
            FireRate = fireRate;
            MagazineSize = magazineSize;
            ReloadTime = reloadTime;
            AType = aType;
            DType = dType;
        }
    }

    public class ArmorItemModel : InventoryItemModel //TODO damage types but I'm lazy
    {
        public readonly float DamageResistance;
        public readonly float DamageThreshold;

        public ArmorItemModel(string name, float weight, float maxCondition, bool unique, bool essential,
            float damageResistance, float damageThreshold) 
            : base(name, weight, maxCondition, unique, essential)
        {
            DamageResistance = damageResistance;
            DamageThreshold = damageThreshold;
        }
    }

    public class AidItemModel : InventoryItemModel
    {
        public readonly AidType AType;
        public readonly RestoreType RType;
        public float Amount;

        public AidItemModel(string name, float weight, float maxCondition, bool unique, bool essential,
            AidType aType, RestoreType rType, float amount) 
            : base(name, weight, maxCondition, unique, essential)
        {
            AType = aType;
            RType = rType;
            Amount = amount;
        }
    }

    public class InventoryModel
    {
        static InventoryModel()
        {
            //a stupid place to do it but we don't have a loader architecture
            LoadAllModels();
        }

        private static void LoadAllModels()
        {
            //I'm too fucking lazy to actually implement a loader
            Models = new Dictionary<string, InventoryItemModel>();
            Models.Add("m1911", new WeaponItemModel("m1911", 3, 1.0f, false, false, 6.0f, 0, 5.0f, 0.75f, 7, 3.0f, AmmoType.Acp45, DamageType.Pierce));
            Models.Add("revolver", new WeaponItemModel("revolver", 3, 1.0f, false, false, 7.0f, 2.0f, 4.0f, 1.0f, 6, 5.0f, AmmoType.Spc38, DamageType.Pierce));
        }

        private static Dictionary<string, InventoryItemModel> Models;
        private List<InventoryItemInstance> Items;
        
        public InventoryModel()
        {
            Items = new List<InventoryItemInstance>();
        }

        public int CountItem(string item)
        {
            int quantity = 0;
            foreach(InventoryItemInstance i in Items)
            {
                if (i.ItemModel.Name == item)
                    quantity++;
            }

            return quantity;
        }

        public InventoryItemInstance[] GetItem(string item) //lack of unique keys makes this essentially useless
        {
            List<InventoryItemInstance> items = new List<InventoryItemInstance>();

            foreach(InventoryItemInstance i in Items)
            {
                if (i.ItemModel.Name == item)
                    items.Add(i);
            }

            return items.ToArray();
        }

        public InventoryItemModel UseItem(string item)
        {
            //search list for first instance
            int foundIndex = -1;
            InventoryItemModel foundModel = null;
            for(int i = 0; i < Items.Count; i++)
            {
                if(Items[i].ItemModel.Name == item)
                {
                    foundIndex = i;
                    foundModel = Items[i].ItemModel;
                    break;
                }
            }
            if(foundIndex >= 0)
                Items.RemoveAt(foundIndex);

            return foundModel;
        }

        public void AddItem(string item, int quantity)
        {
            InventoryItemModel mdl = Models[item];

            for(int i = 0; i < quantity; i++)
            {
                Items.Add(new InventoryItemInstance(mdl));
            }
        }

    }
}