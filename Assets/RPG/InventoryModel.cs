using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//TODO split this up because, I mean, damn
namespace CommonCore.RPG
{

    public class InventoryModel
    {
        private static Dictionary<string, InventoryItemModel> Models;
        private static Dictionary<string, InventoryItemDef> Defs;

        static InventoryModel()
        {
            //a stupid place to do it but we don't have a loader architecture
            LoadAllModels();
            LoadAllDefs();
        }

        private static void LoadAllModels()
        {
            //I'm too fucking lazy to actually implement a loader
            /*
            Models = new Dictionary<string, InventoryItemModel>();

            Models.Add("m1911", new WeaponItemModel("m1911", 3, 1.0f, false, false, 6.0f, 0, 70f, 5.0f, 0.75f, 7, 3.0f, AmmoType.Acp45, DamageType.Pierce, "PistolEffect", "ReloadNormal"));
            Models.Add("revolver", new WeaponItemModel("revolver", 3, 1.0f, false, false, 7.0f, 2.0f, 70f, 4.0f, 1.0f, 6, 5.0f, AmmoType.Spc38, DamageType.Pierce, "RevolverEffect", "ReloadRevolver"));
            Models.Add("lightarmor", new AidItemModel("lightarmor", 15, 1.0f, false, false, AidType.Armor, RestoreType.Override, 100.0f));
            Models.Add("heavyarmor", new AidItemModel("heavyarmor", 25, 1.0f, false, false, AidType.Armor, RestoreType.Override, 200.0f));
            Models.Add("stimpack", new AidItemModel("stimpack", 1, 1.0f, false, false, AidType.Health, RestoreType.Add, 20.0f));
            Models.Add("medkit", new AidItemModel("medkit", 5, 1.0f, false, false, AidType.Health, RestoreType.Add, 50.0f));
            */

            string data = Resources.Load<TextAsset>("RPGDefs/rpg_items").text;
            Models = JsonConvert.DeserializeObject<Dictionary<string, InventoryItemModel>>(data, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            /*
            string json = JsonConvert.SerializeObject(Models, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            string path = Application.persistentDataPath + "/m.json";
            File.WriteAllText(path, json);
            */
        }

        private static void LoadAllDefs()
        {
            TextAsset ta = Resources.Load<TextAsset>("RPGDefs/rpg_items_defs");
            try
            {

                Defs = JsonConvert.DeserializeObject<Dictionary<string, InventoryItemDef>>(ta.text);
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static InventoryItemModel GetModel(string name)
        {
            return Models[name];
        }

        public static InventoryItemDef GetDef(string name)
        {
            if (!Defs.ContainsKey(name))
                return null;

            return Defs[name];
        }

        [JsonProperty]
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

        public List<InventoryItemInstance> GetItemsListActual()
        {
            return Items;
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