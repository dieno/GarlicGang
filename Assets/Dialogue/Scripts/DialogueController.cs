﻿using System.Collections;
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
        public Renderer RendererBackground;
        public AudioSource AudioMusic;

        private string CurrentSceneName;
        private Dictionary<string, Frame> CurrentSceneFrames;

        //private string CurrentFrameName;
        private Frame CurrentFrameObject;       
        
        void Start()
        {
            GameState.Instance.CurrentDialogue = "intro.intro1";

            var loc = ParseLocation(GameState.Instance.CurrentDialogue);

            LoadScene(loc.Key);

            PresentNewFrame(loc.Value);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LoadScene(string scene)
        {
            CurrentSceneFrames = DialogueParser.LoadDialogue(scene);
            CurrentSceneName = scene;
        }

        private void PresentNewFrame(string s)
        {
            PresentNewFrame(CurrentSceneFrames[s]);
        }
        
        private void PresentNewFrame(Frame f) //args?
        {
            //present background
            try
            {
                RendererBackground.material.mainTexture = Resources.Load<Texture>("dBackgrounds/" + f.Background);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }

            //present music (will have issues with edge cases)
            try
            {
                if(!string.IsNullOrEmpty(f.Music))
                {
                    AudioClip ac = Resources.Load<AudioClip>("DynamicMusic/" + f.Music);

                    if (AudioMusic.clip != null && AudioMusic.clip.name == ac.name)
                    {
                        //do nothing
                    }
                    else
                    {
                        AudioMusic.clip = ac;
                        AudioMusic.Play();
                    }
                }                
            }
            catch(Exception e)
            {
                Debug.Log(e);
                AudioMusic.Stop();
            }
            

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
            
            //Debug.Log(choice);

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
                if(nextLoc.Value == "return")
                {
                    SceneManager.LoadScene(GameState.Instance.CurrentScene);
                }
            }
            else if (nextLoc.Key == "scene")
            {
                SceneManager.LoadScene(nextLoc.Value);
            }
            else
            {
                LoadScene(nextLoc.Key);
                PresentNewFrame(nextLoc.Value);
            }

        }

        
    }
}