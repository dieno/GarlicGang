using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//loosely based on the FSVR base controller
public abstract class BaseSceneController : MonoBehaviour
{
    public bool AutorestoreLocation = false;
    public bool SaveOnLoad = false;
    public bool SaveOnExit = true;
    public int Morality;
    public int Reputation;
    public List<string> Flags = new List<string>();

    public virtual void Awake()
    {
        //force for now
        //ConfigState.Restore();

        //hacky fix for debugging
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (GameState.SaveExists)
                GameState.Restore();
        }

        //TODO initialize HUD and controls
    }

    // Use this for initialization
    public virtual void Start()
    {

        if (SaveOnLoad)
        {
            SaveGame(SceneManager.GetActiveScene().name);
        }

        if(AutorestoreLocation && GameState.Instance.OverridePosition.HasValue)
        {
            var player = GameObject.Find("Player");
            player.transform.position = GameState.Instance.OverridePosition.Value;
            player.transform.rotation = GameState.Instance.OverrideRotation.Value;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void EndLevel(string nextLevel)
    {
        EndLevel(nextLevel, SaveOnExit);
    }

    public virtual void EndLevel(string nextLevel, bool saveOnExit)
    {
        //TODO any end-of-level processing should be done here

        if (saveOnExit)
        {
            SaveGame(nextLevel);
        }
        SceneManager.LoadScene(nextLevel);
    }

    protected void SaveGame(string nextLevel)
    {
        var gs = GameState.Instance;
        gs.LastScene = SceneManager.GetActiveScene().name;
        gs.CurrentScene = nextLevel;
        foreach (string s in Flags)
            gs.CampaignFlags.Add(s);
        GameState.Save();
    }
}
