using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class RTSCursorAffordance : MonoBehaviour
    {
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D unknownCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

        //RTS_CamRaycaster cameraRaycaster;
        private RTSGameMaster gamemaster
        {
            get { return RTSGameMaster.thisInstance; }
        }

        RTSUiMaster uimaster { get { return RTSUiMaster.thisInstance; } }

        private void OnDisable()
        {
            gamemaster.OnMouseCursorChange -= OnLayerChanged;
            gamemaster.GameOverEvent -= HandleGameOver;
            gamemaster.EventAllObjectivesCompleted -= HandleGameOver;
            gamemaster.GoToMenuSceneEvent -= SetCursorToNull;
            uimaster.EventAnyUIToggle -= HandleUIEnabled;
            //uimaster.EventIGBPIToggle -= HandleUIEnabled;
            //uimaster.EventInventoryUIToggle -= HandleUIEnabled;
        }

        // Use this for initialization
        void Start()
        {
            gamemaster.OnMouseCursorChange += OnLayerChanged;
            gamemaster.GameOverEvent += HandleGameOver;
            gamemaster.EventAllObjectivesCompleted += HandleGameOver;
            gamemaster.GoToMenuSceneEvent += SetCursorToNull;
            uimaster.EventAnyUIToggle += HandleUIEnabled;
            //uimaster.EventIGBPIToggle += HandleUIEnabled;
            //uimaster.EventInventoryUIToggle += HandleUIEnabled;
        }

        void OnLayerChanged(rtsHitType hitType, RaycastHit hit)
        {
            //TODO: RTSPrototype Set Cursor To Null Whenever UI is in use
            //[Doesn't Work]Temporary Fix Whenever OnLayerChange is called
            //and Ui is being used
            //if (uimaster.isUiAlreadyInUse)
            //{
            //    Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
            //    return;
            //}
            switch (hitType)
            {
                case rtsHitType.Ally:
                    Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case rtsHitType.Enemy:
                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case rtsHitType.Cover:
                    break;
                case rtsHitType.Walkable:
                    Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                    break;
                case rtsHitType.Unwalkable:
                    Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
                    break;
                case rtsHitType.Unknown:
                    Cursor.SetCursor(unknownCursor, cursorHotspot, CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(unknownCursor, cursorHotspot, CursorMode.Auto);
                    break;
            }
        }

        void HandleUIEnabled()
        {
            if (uimaster.isUiAlreadyInUse)
            {
                Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
            }
        }

        void HandleUIEnabled(bool _state)
        {
            if (_state)
            {
                Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
            }
        }

        void SetCursorToNull()
        {
            Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
        }

        void HandleGameOver()
        {
            Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
            Destroy(this);
        }
    }
}