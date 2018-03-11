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
            Frame baseFrame = new Frame(sBackground, sImage, sNext, sMusic, string.Empty, string.Empty, null, null);

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

            //TODO load/parse conditionals and microscripts
            ConditionNode[] conditional = null;
            MicroscriptNode[] microscript = null;

            if(jt["conditional"] != null)
            {
                List<ConditionNode> cList = new List<ConditionNode>();
                JArray ja = (JArray)jt["conditional"];
                foreach (var x in ja)
                {
                    try
                    {
                        cList.Add(ParseConditionNode(x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                conditional = cList.ToArray();
            }

            if (jt["microscript"] != null)
            {
                //TODO parse microscripts
                List<MicroscriptNode> nList = new List<MicroscriptNode>();
                JArray ja = (JArray)jt["microscript"];
                foreach (var x in ja)
                {
                    try
                    {
                        nList.Add(ParseMicroscript(x));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                microscript = nList.ToArray();
            }

            if (type == "choice")
            {
                //parse choices if choice frame
                List<ChoiceNode> choices = new List<ChoiceNode>();
                JArray ja = (JArray)jt["choices"];
                foreach(var x in ja)
                {
                    choices.Add(ParseChoiceNode(x));
                }
                return new ChoiceFrame(background, image, next, music, nameText, text, choices.ToArray(), conditional, microscript);
            }
            else if(type == "text")
            {
                return new TextFrame(background, image, next, music, nameText, text, conditional, microscript);
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

            MicroscriptNode[] microscripts = null;
            if(jt["microscript"] != null)
            {
                //TODO parse microscripts
                List<MicroscriptNode> nList = new List<MicroscriptNode>();
                JArray ja = (JArray)jt["microscript"];
                foreach(var x in ja)
                {
                    try
                    {
                        nList.Add(ParseMicroscript(x));
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning(e);
                    }                    
                }
                microscripts = nList.ToArray();
            }

            ConditionNode[] conditionals = null;
            if(jt["conditional"] != null)
            {
                List<ConditionNode> cList = new List<ConditionNode>();
                JArray ja = (JArray)jt["conditional"];
                foreach(var x in ja)
                {
                    try
                    {
                        cList.Add(ParseConditionNode(x));
                    }
                    catch(Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                    
                }
                conditionals = cList.ToArray();
            }

            Condition showCondition = null;
            if(jt["showCondition"] != null)
            {
                showCondition = ParseSingleCondition(jt["showCondition"]);
            }

            Condition hideCondition = null;
            if(jt["hideCondition"] != null)
            {
                hideCondition = ParseSingleCondition(jt["hideCondition"]);
            }

            return new ChoiceNode(next, text, showCondition, hideCondition, microscripts, conditionals);
        }

        private ConditionNode ParseConditionNode(JToken jt)
        {
            //next and list of conditions
            string next = jt["next"].Value<string>();
            JArray ja = (JArray)jt["conditions"];
            List<Condition> conditions = new List<Condition>();
            foreach(var x in ja)
            {
                conditions.Add(ParseSingleCondition(x));
            }

            return new ConditionNode(next, conditions.ToArray());
        }

        private Condition ParseSingleCondition(JToken jt)
        {
            //types
            ConditionType type;
            string target;
            if(jt["flag"] != null)
            {
                type = ConditionType.Flag;
                target = jt["flag"].Value<string>();
            }
            else if(jt["noflag"] != null)
            {
                type = ConditionType.NoFlag;
                target = jt["noflag"].Value<string>();
            }
            else if(jt["variable"] != null)
            {
                type = ConditionType.Variable;
                target = jt["variable"].Value<string>();
            }
            else if(jt["affinity"] != null)
            {
                type = ConditionType.Affinity;
                target = jt["affinity"].Value<string>();
            }
            else if(jt["quest"] != null)
            {
                type = ConditionType.Quest;
                target = jt["quest"].Value<string>();
            }
            else if(jt["item"] != null)
            {
                type = ConditionType.Item;
                target = jt["item"].Value<string>();
            }
            else
            {
                throw new NotSupportedException();
            }

            //options
            ConditionOption? option = null;
            int optionValue = 0;
            if(type == ConditionType.Item)
            {
                //check for "consume"
                if (jt["consume"] != null)
                {
                    option = ConditionOption.Consume;
                    optionValue = Convert.ToInt32(jt["consume"].Value<bool>());
                }
                    
            }
            else if(type == ConditionType.Affinity || type == ConditionType.Quest || type == ConditionType.Variable)
            {
                if(jt["greater"] != null)
                {
                    option = ConditionOption.Greater;
                    optionValue = jt["greater"].Value<int>();
                }
                else if(jt["less"] != null)
                {
                    option = ConditionOption.Less;
                    optionValue = jt["less"].Value<int>();
                }
                else if (jt["equal"] != null)
                {
                    option = ConditionOption.Equal;
                    optionValue = jt["equal"].Value<int>();
                }
                else if (jt["greaterEqual"] != null)
                {
                    option = ConditionOption.GreaterEqual;
                    optionValue = jt["greaterEqual"].Value<int>();
                }
                else if (jt["lessEqual"] != null)
                {
                    option = ConditionOption.LessEqual;
                    optionValue = jt["lessEqual"].Value<int>();
                }
                else if (jt["started"] != null)
                {
                    option = ConditionOption.Started;
                    optionValue = Convert.ToInt32(jt["started"].Value<bool>());
                }
                else if (jt["finished"] != null)
                {
                    option = ConditionOption.Finished;
                    optionValue = Convert.ToInt32(jt["finished"].Value<bool>());
                }
            }

            return new Condition(type, target, option, optionValue);
        }

        private MicroscriptNode ParseMicroscript(JToken jt)
        {
            MicroscriptType type;
            string target;
            if(jt["flag"] != null)
            {
                type = MicroscriptType.Flag;
                target = jt["flag"].Value<string>();
            }
            else if(jt["item"] != null)
            {
                type = MicroscriptType.Item;
                target = jt["item"].Value<string>();
            }
            else if (jt["variable"] != null)
            {
                type = MicroscriptType.Variable;
                target = jt["variable"].Value<string>();
            }
            else if (jt["affinity"] != null)
            {
                type = MicroscriptType.Affinity;
                target = jt["affinity"].Value<string>();
            }
            else if (jt["quest"] != null)
            {
                type = MicroscriptType.Quest;
                target = jt["quest"].Value<string>();
            }
            else
            {
                throw new NotSupportedException();
            }

            MicroscriptAction action;
            int value = 0;
            if(jt["set"] != null)
            {
                action = MicroscriptAction.Set;
                if (type == MicroscriptType.Flag) //parse as boolean
                    value = Convert.ToInt32(jt["set"].Value<bool>());
                else //otherwise parse as number
                    value = jt["set"].Value<int>(); 
            }
            else if (jt["toggle"] != null)
            {
                action = MicroscriptAction.Toggle;
            }
            else if(jt["add"] != null)
            {
                action = MicroscriptAction.Add;
                value = jt["add"].Value<int>();
            }
            else if (jt["give"] != null)
            {
                action = MicroscriptAction.Give;
                value = jt["give"].Value<int>();
            }
            else if (jt["take"] != null)
            {
                action = MicroscriptAction.Take;
                value = jt["take"].Value<int>();
            }
            else if (jt["start"] != null)
            {
                action = MicroscriptAction.Start;
                value = jt["start"].Value<int>();
            }
            else if (jt["finish"] != null)
            {
                action = MicroscriptAction.Finish;
                value = jt["finish"].Value<int>();
            }
            else
            {
                throw new NotSupportedException();
            }

            return new MicroscriptNode(type, target, action, value);
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
                var cf = (ChoiceFrame)CurrentFrameObject;

                if(cf.Choices[idx].NextConditional != null)
                {
                    choice = cf.Choices[idx].EvaluateConditional();
                }
                else
                {
                    choice = cf.Choices[idx].Next;
                } 

                //exec microscripts
                if(cf.Choices[idx].NextMicroscript != null)
                {
                    cf.Choices[idx].EvaluateMicroscript();
                }
            }
            else
            {
                if (CurrentFrameObject.NextConditional != null && CurrentFrameObject.NextConditional.Length > 0)
                    choice = CurrentFrameObject.EvaluateConditional();
                else
                    choice = CurrentFrameObject.Next;

                if(CurrentFrameObject.NextMicroscript != null)
                {
                    CurrentFrameObject.EvaluateMicroscript();
                }
            }
            
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