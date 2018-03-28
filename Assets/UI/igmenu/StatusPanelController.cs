using CommonCore.RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonCore.UI
{

    public class StatusPanelController : PanelController
    {
        public RawImage CharacterImage;
        public Text HealthText;
        public Text ArmorText;
        public Text AmmoText;

        public override void SignalPaint()
        {
            PlayerModel pModel = GameState.Instance.Player;
            PlayerControl pControl = PlayerControl.Instance;

            //repaint 
            HealthText.text = string.Format("Health: {0}/{1}", (int) pModel.Health, (int) pModel.MaxHealth);

            //do portrati
            string rid = pModel.Gender == Sex.Female ? "portrait_f" : "portrait_m";
            CharacterImage.texture = Resources.Load<Texture2D>("DynamicUI/" + rid);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}