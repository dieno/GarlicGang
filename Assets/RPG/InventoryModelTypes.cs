using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

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
        Health, Armor
    }

    public enum RestoreType
    {
        Add, Boost, //boost allows going over max, add does not
        Override //override replaces
    }

    //an actual inventory item that the player has
    [JsonConverter(typeof(InventoryItemSerializer))]
    public class InventoryItemInstance
    {
        public float Condition { get; set; } //it's here but basically unimplemented
        public bool Equipped { get; set; }
        public readonly InventoryItemModel ItemModel;

        internal InventoryItemInstance(InventoryItemModel model, float condition)
        {
            ItemModel = model;
            Condition = condition;
            Equipped = false;
        }

        public InventoryItemInstance(InventoryItemModel model)
        {
            ItemModel = model;
            Condition = model.MaxCondition;
            Equipped = false;
        }
    }

    public class InventoryItemSerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = value as InventoryItemInstance;
            writer.WriteStartObject();
            writer.WritePropertyName("Condition");
            writer.WriteValue(item.Condition);
            writer.WritePropertyName("$ItemModel");
            writer.WriteValue(item.ItemModel.Name);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jsonObject = JObject.Load(reader);
            float condition = jsonObject["Condition"].Value<float>();
            string modelName = jsonObject["$ItemModel"].Value<string>();
            InventoryItemModel model = InventoryModel.GetModel(modelName);

            return new InventoryItemInstance(model, condition);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(InventoryItemInstance).IsAssignableFrom(objectType);
        }
    }

    // class for invariant inventory defs
    public class InventoryItemDef
    {
        public string NiceName;
        public string Image;
        public string Description;

        public InventoryItemDef(string niceName, string image, string description)
        {
            NiceName = niceName;
            Image = image;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}\t{2}]", NiceName, Image, Description);
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

        public virtual string GetStatsString()
        {
            return Essential ? "Essential" : string.Empty;
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
        public readonly float Velocity;
        public readonly float Spread;
        public readonly float FireRate;
        public readonly int MagazineSize;
        public readonly float ReloadTime;
        public readonly AmmoType AType;
        public readonly DamageType DType;
        public readonly string FireEffect;
        public readonly string ReloadEffect;

        public WeaponItemModel(string name, float weight, float maxCondition, bool unique, bool essential,
            float damage, float damagePierce, float velocity, float spread, float fireRate,
            int magazineSize, float reloadTime, AmmoType aType, DamageType dType, string fireEffect, string reloadEffect)
            : base(name, weight, maxCondition, unique, essential)
        {
            Damage = damage;
            DamagePierce = damagePierce;
            Velocity = velocity;
            Spread = spread;
            FireRate = fireRate;
            MagazineSize = magazineSize;
            ReloadTime = reloadTime;
            AType = aType;
            DType = dType;
            FireEffect = fireEffect;
            ReloadEffect = reloadEffect;
        }

        public override string GetStatsString()
        {
            StringBuilder str = new StringBuilder(255);
            //TODO finish impl

            return str.ToString() + base.GetStatsString();
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
}
