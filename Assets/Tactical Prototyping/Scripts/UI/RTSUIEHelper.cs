using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;

namespace RTSPrototype
{
    public class RTSUIEHelper
    {
        #region Getters
        public static bool IsPanelScreenEnabled(ref PanelRenderer screen)
        {
            UIElementsEventSystem _myEventSystem = null;
            return IsPanelScreenEnabled(ref screen, out _myEventSystem);
        }

        public static bool IsPanelScreenEnabled(ref PanelRenderer screen, out UIElementsEventSystem myEventSystem)
        {
            myEventSystem = null;
            return screen != null && screen.enabled &&
                (myEventSystem = screen.gameObject.GetComponent<UIElementsEventSystem>()) != null &&
                myEventSystem.enabled;
        }
        #endregion

        #region TransitionHelpers
        public static void SetScreenEnableState(ref PanelRenderer screen, bool state)
        {
            UIElementsEventSystem _myEventSystem = screen.gameObject.GetComponent<UIElementsEventSystem>();
            SetScreenEnableState(ref screen, ref _myEventSystem, state);
        }

        public static void DisablePanelScreenIfEnabled(ref PanelRenderer screen)
        {
            UIElementsEventSystem _myEventSystem = null;
            if (IsPanelScreenEnabled(ref screen, out _myEventSystem))
            {
                SetScreenEnableState(ref screen, ref _myEventSystem, false);
            }
        }
        #endregion

        #region TransitionLogic
        //Screen Transition Logic

        public static void SetScreenEnableState(ref PanelRenderer screen, ref UIElementsEventSystem uieSystem, bool state)
        {
            if (state)
            {
                screen.visualTree.style.display = DisplayStyle.Flex;
                screen.enabled = true;
                uieSystem.enabled = true;
            }
            else
            {
                screen.visualTree.style.display = DisplayStyle.None;
                screen.enabled = false;
                uieSystem.enabled = false;
            }
        }

        public static IEnumerator TransitionScreens(PanelRenderer from, PanelRenderer to)
        {
            from.visualTree.style.display = DisplayStyle.None;
            from.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;

            to.enabled = true;

            yield return null;
            yield return null;
            yield return null;

            to.visualTree.style.display = DisplayStyle.Flex;
            to.visualTree.style.visibility = Visibility.Hidden;
            to.gameObject.GetComponent<UIElementsEventSystem>().enabled = true;

            yield return null;
            yield return null;
            yield return null;

            to.visualTree.style.visibility = Visibility.Visible;

            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;

            from.enabled = false;
        }
        #endregion

    }
}