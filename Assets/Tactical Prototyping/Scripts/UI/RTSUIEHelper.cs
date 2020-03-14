using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;

namespace RTSPrototype
{
    public class RTSUIEHelper
    {
        #region TransitionLogic
        //Screen Transition Logic From Unity Tank Demo

        public static void SetScreenEnableState(PanelRenderer screen, bool state)
        {
            if (state)
            {
                screen.visualTree.style.display = DisplayStyle.Flex;
                screen.enabled = true;
                screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = true;
            }
            else
            {
                screen.visualTree.style.display = DisplayStyle.None;
                screen.enabled = false;
                screen.gameObject.GetComponent<UIElementsEventSystem>().enabled = false;
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