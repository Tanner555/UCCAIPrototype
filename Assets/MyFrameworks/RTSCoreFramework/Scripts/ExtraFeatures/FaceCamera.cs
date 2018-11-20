using UnityEngine;

namespace RTSCoreFramework
{
    public class FaceCamera : MonoBehaviour
    {
        Camera cameraToLookAt;

        AllyEventHandler rootEventHandler
        {
            get
            {
                if (_rootEventHandler == null)
                    _rootEventHandler = GetComponentInParent<AllyEventHandler>();

                return _rootEventHandler;
            }
        }
        AllyEventHandler _rootEventHandler = null;

        private void OnEnable()
        {
            cameraToLookAt = Camera.main;

            rootEventHandler.EventAllyDied += OnDeath;
            InvokeRepeating("LookAtMainCamera", 0.1f, 0.2f);
        }

        private void OnDisable()
        {
            rootEventHandler.EventAllyDied -= OnDeath;
        }

        void OnDeath()
        {
            CancelInvoke();
            Destroy(this);
        }

        void LookAtMainCamera()
        {
            transform.LookAt(cameraToLookAt.transform);
        }
    }
}