using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DialogueController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        LoadDialogue("coldopen");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadDialogue(string dialogueName)
    {
        TextAsset ta = Resources.Load<TextAsset>("dData/" + dialogueName);
        JObject jo = JObject.Parse(ta.text);
        Debug.Log(jo);
    }
}
