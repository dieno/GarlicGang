using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;

namespace CommonCore.Dialogue
{

    public class DialogueController : MonoBehaviour
    {
        public Text TextTitle;
        public Text TextMain;
        public Button[] ButtonsChoice;
        public Renderer RendererSubject;

        private string CurrentSceneName;
        private Dictionary<string, Frame> CurrentSceneFrames;

        //private string CurrentFrameName;
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
            PresentNewFrame(CurrentSceneFrames[s]);
        }
        
        private void PresentNewFrame(Frame f) //args?
        {
            //TODO background and music

            //present cutout
            try
            {
                RendererSubject.material.mainTexture = Resources.Load<Texture>("dImages/" + f.Image);
            }
            catch(Exception e) //pokemon
            {
                Debug.Log(e);
            }
            

            //present text
            TextTitle.text = f.NameText;
            TextMain.text = f.Text;

            //present buttons
            foreach(Button b in ButtonsChoice)
            {
                b.gameObject.SetActive(false);
            }

            if(f is ChoiceFrame)
            {
                ChoiceFrame cf = (ChoiceFrame)f;
                for (int i = 0, j = 0; i < cf.Choices.Length && j < ButtonsChoice.Length; i++)
                {
                    //will need to be redone to effectively deal with conditionals
                    ChoiceNode cn = cf.Choices[i];
                    bool showChoice = true;
                    if(cn.ShowCondition != null)
                    {
                        showChoice = cn.ShowCondition.Evaluate();
                    }
                    if(cn.HideCondition != null && showChoice)
                    {
                        showChoice = !cn.HideCondition.Evaluate();
                    }

                    if(showChoice)
                    {
                        Button b = ButtonsChoice[j];
                        b.gameObject.SetActive(true);
                        b.transform.Find("Text").GetComponent<Text>().text = cn.Text;
                        j++;
                    }
                    
                }
            }
            else // if(f is TextFrame)
            {
                Button b = ButtonsChoice[ButtonsChoice.Length - 1];
                b.gameObject.SetActive(true);
                b.transform.Find("Text").GetComponent<Text>().text = "Continue..."; //TODO nextText support
            }

            CurrentFrameObject = f;
        }

        public void OnChoiceButtonClick(int idx)
        {
            string choice = null;
            if(CurrentFrameObject is ChoiceFrame)
            {
                choice = ((ChoiceFrame)CurrentFrameObject).Choices[idx].Next;
            }
            else
            {
                choice = CurrentFrameObject.Next;
            }

            //TODO handle parsing and transition
            Debug.Log(choice);

            GotoNext(choice);

        }

        private KeyValuePair<string, string> ParseLocation(string loc)
        {
            if (!loc.Contains("."))
                return new KeyValuePair<string, string>(null, loc);

            var arr = loc.Split('.');
            return new KeyValuePair<string, string>(arr[0], arr[1]);
        }

        private void GotoNext(string next)
        {
            var nextLoc = ParseLocation(next);

            if(string.IsNullOrEmpty(nextLoc.Key) || nextLoc.Key == "this" || nextLoc.Key == CurrentSceneName)
            {
                PresentNewFrame(nextLoc.Value);
            }
            else if(nextLoc.Key == "meta")
            {
                //TODO any meta ones
            }
            else if (nextLoc.Key == "scene")
            {
                SceneManager.LoadScene(nextLoc.Value);
            }
            else
            {
                LoadDialogue(nextLoc.Key);
                PresentNewFrame(nextLoc.Value);
            }

        }

        
    }
}