using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonCore.Messaging;

//loosely based on the FSVR base controller
public abstract class BaseSceneController : MonoBehaviour
{

    public static BaseSceneController Current { get
        {
            return GameObject.Find("WorldRoot").GetComponent<BaseSceneController>();
        } }

    public bool AutoloadUI = true;
    public bool SaveOnLoad = false;
    public bool SaveOnExit = true;
    public int Morality;
    public int Reputation;

    protected QdmsMessageInterface MessageInterface;

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

        //initialize message bus
        QdmsMessageBus.ForceCreate();
        QdmsMessageBus.Instance.ForceCleanup();
        MessageInterface = new QdmsMessageInterface(gameObject);

        //TODO initialize HUD and controls
        if (AutoloadUI)
            InitUI();
    }

    // Use this for initialization
    public virtual void Start()
    {
        GameState.Instance.CurrentScene = SceneManager.GetActiveScene().name;

        if (SaveOnLoad)
        {
            SaveGame(SceneManager.GetActiveScene().name);
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
        GameState.Save();
    }

    protected void InitUI()
    {
        Instantiate<GameObject>(Resources.Load<GameObject>("DynamicUI/HUDCanvas"), transform).name = "HUDCanvas";
        Instantiate<GameObject>(Resources.Load<GameObject>("DynamicUI/MenuSystem"), transform).name = "MenuSystem";
    }
}
