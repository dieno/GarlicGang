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
        const bool AutocreateModels = true;

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

            if(AutocreateModels)
            {

                foreach (AmmoType at in Enum.GetValues(typeof(AmmoType)))
                {
                    AmmoItemModel aim = new AmmoItemModel(at.ToString(), 0, 1, false, false, at);
                    Models.Add(at.ToString(), aim);
                }

                foreach(MoneyType mt in Enum.GetValues(typeof(MoneyType)))
                {
                    MoneyItemModel mim = new MoneyItemModel(mt.ToString(), 0, 1, false, false, mt);
                    Models.Add(mt.ToString(), mim);
                }
            }

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
                if (i.ItemModel.Name == item && i.Quantity == -1)
                    quantity++;
                else if (i.ItemModel.Name == item && i.Quantity > 0)
                    quantity += i.Quantity;
            }

            return quantity;
        }

        public List<InventoryItemInstance> GetItemsListActual()
        {
            return Items;
        }

        [Obsolete("I don't even know what GetItem was supposed to do")]
        public InventoryItemInstance[] GetItem(string item) //lack of unique keys makes this essentially useless
        {
            Debug.LogWarning("GetItem is deprecated!");

            List<InventoryItemInstance> items = new List<InventoryItemInstance>();

            foreach(InventoryItemInstance i in Items)
            {
                if (i.ItemModel.Name == item)
                    items.Add(i);
            }

            return items.ToArray();
        }

        public InventoryItemModel UseItem(string item, int quantity)
        {
            int foundIndex = -1;
            InventoryItemModel foundModel = null;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ItemModel.Name == item)
                {
                    foundIndex = i;
                    foundModel = Items[i].ItemModel;
                    break;
                }
            }
            if (foundIndex >= 0)
            {
                if (foundModel.Stackable)
                {
                    if (Items[foundIndex].Quantity < quantity)
                        throw new InvalidOperationException();

                    Items[foundIndex].Quantity -= quantity;
                    if (Items[foundIndex].Quantity == 0)
                        Items.RemoveAt(foundIndex);
                }
                else
                {
                    if (quantity > 1)
                    {
                        //TODO fuck this is horrible
                        for(int j = 0; j < quantity; j++)
                        {
                            UseItem(item);
                        }
                    }
                    else
                    {
                        Items.RemoveAt(foundIndex);
                    }

                    
                }

            }


            return foundModel;
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
            {
                if(foundModel.Stackable)
                {
                    Items[foundIndex].Quantity -= 1;
                    if (Items[foundIndex].Quantity == 0)
                        Items.RemoveAt(foundIndex);
                }
                else
                {
                    Items.RemoveAt(foundIndex);
                }
                
            }
                

            return foundModel;
        }

        public void AddItem(string item, int quantity)
        {
            InventoryItemModel mdl = Models[item];

            if(mdl.Stackable)
            {
                InventoryItemInstance instance = null;
                foreach(InventoryItemInstance i in Items)
                {
                    if(i.ItemModel.Name == mdl.Name)
                    {
                        instance = i;
                        break;
                    }
                }
                if(instance == null)
                {
                    instance = new InventoryItemInstance(mdl);
                    Items.Add(instance);
                }

                instance.Quantity += quantity;
            }
            else
            {
                for (int i = 0; i < quantity; i++)
                {
                    Items.Add(new InventoryItemInstance(mdl));
                }
            }

        }

    }
}