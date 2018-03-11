using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CommonCore.Dialogue
{

    public class DialogueController : MonoBehaviour
    {
        private string CurrentSceneName;
        private Dictionary<string, Frame> CurrentSceneFrames;

        private string CurrentFrameName;
        private Frame CurrentFrameObject;       
        
        void Start()
        {
            LoadDialogue("coldopen"); //TODO get from gamestate
            PresentNewFrame("matsuda1");
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

            //parse root node (scene)
            string sBackground = string.Empty;
            string sImage = string.Empty;
            string sMusic = string.Empty;
            string sNext = string.Empty;
            string sText = string.Empty;
            if (jo["background"] != null)
                sBackground = jo["background"].Value<string>();
            if (jo["image"] != null)
                sImage = jo["image"].Value<string>();
            if (jo["music"] != null)
                sMusic = jo["music"].Value<string>();
            if (jo["default"] != null)
                sNext = jo["default"].Value<string>();
            if (jo["text"] != null)
                sText = jo["text"].Value<string>();
            Frame baseFrame = new Frame(sBackground, sImage, sNext, sMusic, string.Empty, string.Empty);

            //parse frames
            Dictionary<string, Frame> frames = new Dictionary<string, Frame>();
            JObject jf = (JObject)jo["frames"];
            foreach (var x in jf)
            {
                try
                {
                    string key = x.Key;
                    JToken value = x.Value;
                    Frame f = ParseSingleFrame(value, baseFrame);
                    frames.Add(key, f);
                }
                catch(Exception e)
                {
                    Debug.Log("Failed to parse frame!");
                    Debug.Log(e);
                }
            }

            //confirm
            CurrentSceneName = dialogueName;
            CurrentSceneFrames = frames;
        }

        private Frame ParseSingleFrame(JToken jt, Frame baseFrame)
        {
            string background = baseFrame.Background;
            string image = baseFrame.Image;
            string next = baseFrame.Next;
            string music = baseFrame.Music;
            string nameText = baseFrame.NameText;
            string text = baseFrame.Text;
            string type = null;

            if (jt["background"] != null)
                background = jt["background"].Value<string>();
            if (jt["image"] != null)
                image = jt["image"].Value<string>();
            if (jt["next"] != null)
                next = jt["next"].Value<string>();
            if (jt["music"] != null)
                music = jt["music"].Value<string>();
            if (jt["nameText"] != null)
                nameText = jt["nameText"].Value<string>();
            if (jt["text"] != null)
                text = jt["text"].Value<string>();
            if (jt["type"] != null)
                type = jt["type"].Value<string>();

            
            if(type == "choice")
            {
                //parse choices if choice frame
                List<ChoiceNode> choices = new List<ChoiceNode>();
                JArray ja = (JArray)jt["choices"];
                foreach(var x in ja)
                {
                    choices.Add(ParseChoiceNode(x));
                }
                return new ChoiceFrame(background, image, next, music, nameText, text, choices.ToArray());
            }
            else if(type == "text")
            {
                return new TextFrame(background, image, next, music, nameText, text);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private ChoiceNode ParseChoiceNode(JToken jt)
        {
            string text = jt["text"].Value<string>();
            string next = jt["next"].Value<string>();

            return new ChoiceNode(next, text);
        }

        private void PresentNewFrame(string s)
        {

        }
        
        private void PresentNewFrame(Frame f) //args?
        {

        }

        
    }
}