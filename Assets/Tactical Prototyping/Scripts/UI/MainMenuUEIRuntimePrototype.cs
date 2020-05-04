using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using Unity.UIElements.Runtime;
using BaseFramework;

namespace RTSPrototype
{
    public class MainMenuUEIRuntimePrototype : MainMenuHandler
    {
        //public PanelRenderer m_MainMenuScreen;

        //private void OnEnable()
        //{
        //    if(m_MainMenuScreen != null)
        //    {
        //        m_MainMenuScreen.postUxmlReload += BindMainMenuScreen;
        //    }
        //    else
        //    {
        //        Debug.LogError("No Main Menu Panel Assigned To Handler.");
        //    }
        //}

        //private void Start()
        //{
        //    if (m_MainMenuScreen != null)
        //    {
        //        RTSUIEHelper.SetScreenEnableState(ref m_MainMenuScreen, true);
        //    }
        //}

        //private void OnDisable()
        //{
        //    DisableMainMenuScreenIfEnabled();
        //}

        //public override void Btn_PlayGame()
        //{
        //    Debug.Log("Play game called");
        //    DisableMainMenuScreenIfEnabled();
        //    base.Btn_PlayGame();
        //}

        //private IEnumerable<UnityEngine.Object> BindMainMenuScreen()
        //{
        //    var root = m_MainMenuScreen.visualTree;
            
        //    var startButton = root.Q<Button>("start-button");
        //    if (startButton != null)
        //    {
        //        startButton.clickable.clicked += Btn_PlayGame;
        //    }

        //    var exitButton = root.Q<Button>("exit-button");
        //    if (exitButton != null)
        //    {
        //        exitButton.clickable.clicked += Btn_QuitGame;
        //    }

        //    return null;
        //}

        //void DisableMainMenuScreenIfEnabled()
        //{
        //    if(m_MainMenuScreen != null)
        //    {
        //        m_MainMenuScreen.postUxmlReload -= BindMainMenuScreen;
        //        RTSUIEHelper.DisablePanelScreenIfEnabled(ref m_MainMenuScreen);
        //    }
        //}
    }
}