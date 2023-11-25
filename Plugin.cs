using BepInEx;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaUI
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        var GameTags = 0;
        var LobbyTags = 0;
        var SessionTags = 0;
        
        Color32 ColourCode;
        GameObject obj;
        GameObject CanvasHolder;
        
        void Start() { Utilla.Events.GameInitialized += OnGameInitialized; }

        void OnEnable()
        {
            CanvasHolder.SetActive(true);
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            CanvasHolder.SetActive(false);
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            //grabs name

            var Name = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/NameTagAnchor/NameTagCanvas/Text").GetComponent<Text>().text;

            //spawns canvas
            Text text;
            CanvasHolder = new GameObject("CanvasHolder");

            // changed thingys
            CanvasHolder.transform.SetParent(GorillaLocomotion.Player.Instance.rightControllerTransform);
            CanvasHolder.transform.localPosition = new Vector3(0f, 0.091f, -0.1f);
            CanvasHolder.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
            CanvasHolder.transform.localRotation = Quaternion.Euler(75f, 0f, 0f);

            // adds text
            CanvasHolder.AddComponent<Canvas>();

            //change name of canvas
            CanvasHolder.GetComponent<Canvas>().name = "GorillaUI";

            // load text
            GameObject TextObject = new GameObject("Text");
            TextObject.transform.SetParent(CanvasHolder.transform, false);
            TextObject.AddComponent<CanvasRenderer>();

            //grabs colour code
            ColourCode = GorillaTagger.Instance.offlineVRRig.playerColor;

            //changed text and font
            text = TextObject.AddComponent<Text>();
            text.font = GorillaTagger.Instance.offlineVRRig.playerText.font;
            text.text = "NAME: " + Name + "\nCOLOUR: " + ColourCode.r + ", " + ColourCode.g + ", " + ColourCode.b;


            obj = GameObject.Find("GorillaUI/Text");

            obj.name = "GorillaUI Text";

            obj = GameObject.Find("GorillaUI/GorillaUI Text");

            //Changes colour

            obj.GetComponent<Text>().color = Color.white;
        }
        void OnTagPlayer()
        {
            LobbyTags++;
            SessionTags++;
        }
        public void OnLeave(string gamemode)
        {
            LobbyTags = 0;
        }
    }
    public class OnTagEvent : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == GorillaTagManager.ReportTagEvent || photonEvent.Code == GorillaTagManager.ReportInfectionTagEvent)
            {
                object[] data = photonEvent.CustomData as object[];
                if(data[0].ToString() == PhotonNetwork.LocalPlayer.UserId)
                {
                    Plugin.OnTagPlayer();
                }
            }
        }
    }
}
