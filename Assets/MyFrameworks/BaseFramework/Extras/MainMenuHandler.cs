using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseFramework
{
    /// <summary>
    /// Temporary Class For Handling the Main Menu
    /// Will Likely Replace in the Future with a Global
    /// Game Manager Class
    /// </summary>
    public class MainMenuHandler : MonoBehaviour
    {
        GameInstance gameInstance
        {
            get { return GameInstance.thisInstance; }
        }

        [SerializeField]
        public LevelIndex loadLevel;
        [SerializeField]
        public ScenarioIndex scenario;

        public void Btn_PlayGame()
        {
            if(gameInstance != null)
            {
                gameInstance.LoadLevel(loadLevel, scenario);
            }
        }

        public void Btn_QuitGame()
        {
            Application.Quit();
        }

        #region Testing
        //private void Start()
        //{
        //    //Time.timeScale = 0f;
        //    //InvokeInRealTime("Test1", 0.1f);
        //    //MonoBehaviour _caller = (MonoBehaviour)this;
        //    //Utilities.RTSInvoker.InvokeInRealTime(ref _caller, "Test1", 0.1f);
        //    //StartCoroutine(CallTest1(0.4f));
        //    //var _chart = GameObject.FindObjectOfType<Fungus.Flowchart>();
        //    //if (_chart != null)
        //    //{
        //    //    _chart.SendFungusMessage("Start");
        //    //}

        //}

        //void InvokeInRealTime(string methodName, float time)
        //{
        //    StartCoroutine(InvokeFromCoroutine(methodName, time));
        //}

        //IEnumerator InvokeFromCoroutine(string methodName, float time)
        //{
        //    yield return new WaitForSecondsRealtime(time);
        //    Invoke(methodName, 0.0f);
        //}

        //void Test1()
        //{
        //    Debug.Log("Start From Test1");
        //}
        #endregion

    }
}