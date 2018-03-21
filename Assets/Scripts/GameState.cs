using CommonCore.RPG;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WanzyeeStudio;

public class GameState
{

    [JsonIgnoreAttribute]
    private static GameState instance;

    [JsonIgnoreAttribute]
    public static GameState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameState();
            }

            return instance;
        }
    }

    private GameState()
    {
        // any initial setup
        CampaignFlags = new HashSet<string>();
        CampaignVars = new Dictionary<string, int>();
        CampaignQuests = new Dictionary<string, int>();
        Player = new PlayerModel();
    }

    //actual state data
    public bool InCampaign;
    public int CurrentLevel;

    //temporary
    public HashSet<string> CampaignFlags; //should REALLY REALLY be a Set<string>
    public Dictionary<string, int> CampaignVars;
    public Dictionary<string, int> CampaignQuests;
    public PlayerModel Player { get; private set; }


    //scene transition stuff
    public string LastScene;
    public string CurrentScene;
    public string CurrentDialogue;
    public Vector3? OverridePosition;
    public Quaternion? OverrideRotation;

    public static void Save()
    {
        string json = JsonConvert.SerializeObject(instance,Formatting.Indented, new JsonSerializerSettings
        {
            Converters = JsonNetUtility.defaultSettings.Converters,
            TypeNameHandling = TypeNameHandling.Auto
        });
        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
    }

    public static void Restore()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (!File.Exists(path))
        {
            Debug.Log("no save file exists!");
            throw new FileNotFoundException();
        }
        instance = JsonConvert.DeserializeObject<GameState>(File.ReadAllText(path), new JsonSerializerSettings
        {
            Converters = JsonNetUtility.defaultSettings.Converters,
            TypeNameHandling = TypeNameHandling.Auto
        });
    }

    public static void Purge()
    {
        Delete();
        instance = null;
    }

    public static void Delete()
    {
        string path = Application.persistentDataPath + "/save.json";
        File.Delete(path);
    }

    public static bool SaveExists
    {
        get
        {
            string path = Application.persistentDataPath + "/save.json";
            return File.Exists(path);
        }
    }
}
