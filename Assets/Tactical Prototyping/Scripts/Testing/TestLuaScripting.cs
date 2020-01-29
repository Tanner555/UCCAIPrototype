using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using MoonSharp.Interpreter;

namespace RTSPrototype
{
    public class TestLuaScripting : MonoBehaviour
    {
        /*
        #region Fields
        public Button RunButton;
        public Button CancelButton;
        public InputField CodeInputField;

        public string defaultCodeInField = @"    
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end

		return fact(5)";
        #endregion

        #region Properties
        Script myLuaScript
        {
            get
            {
                if(_myLuaScript == null)
                {
                    _myLuaScript = new Script();
                    SetupLuaGlobals(ref _myLuaScript);
                }
                return _myLuaScript;
            }
        }
        private Script _myLuaScript = null;
        bool bAllCompsAreValid =>
            RunButton != null && CancelButton != null && CodeInputField != null;
        #endregion

        #region UnityMessages
        private void OnEnable()
        {
            if (bAllCompsAreValid)
            {
                RunButton.onClick.AddListener(OnRunButton);
                CancelButton.onClick.AddListener(OnCancelButton);
                CodeInputField.text = defaultCodeInField;
            }
            else
            {
                Debug.Log("TestLua: Not All Comps Are Set.");
            }
        }
        
        private void OnDisable()
        {
            if (bAllCompsAreValid)
            {
                RunButton.onClick.RemoveAllListeners();
                CancelButton.onClick.RemoveAllListeners();
            }
        }
        #endregion

        #region Handlers
        void OnRunButton()
        {
            string _code = CodeInputField.text;
            try
            {
                myLuaScript.DoString(_code);
            }
            catch (DynamicExpressionException ex)
            {
                Debug.LogWarning($"An DynamicExpressionException error occured! {ex.DecoratedMessage}");
            }
            catch (ScriptRuntimeException ex)
            {
                Debug.LogWarning($"An ScriptRuntimeException error occured! {ex.DecoratedMessage}");
            }
            catch(InternalErrorException ex)
            {
                Debug.LogWarning($"An InternalErrorException error occured! {ex.DecoratedMessage}");
            }
            catch(SyntaxErrorException ex)
            {
                Debug.LogWarning($"An SyntaxErrorException error occured! {ex.DecoratedMessage}");
            }

        }

        void OnCancelButton()
        {
            Debug.Log("Pressed Cancel Button");
        }
        #endregion

        #region Helpers
        public void PrintMsg(string msg)
        {
            Debug.Log(msg);
        }
        #endregion

        #region Initialization
        void SetupLuaGlobals(ref Script luaScript)
        {
            luaScript.Globals["printmsg"] = (System.Action<string>)PrintMsg;
        }
        #endregion
        */
    }
}