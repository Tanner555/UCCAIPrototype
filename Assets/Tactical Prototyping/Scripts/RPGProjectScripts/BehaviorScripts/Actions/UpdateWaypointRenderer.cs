using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using UnityEngine;
using UnityEngine.AI;

namespace RPGPrototype
{
    [TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Updates Waypoint When Command Navigation Moving.")]
	public class UpdateWaypointRenderer : Action
	{
		#region Shared
		public SharedMaterial waypointMaterial;
		public SharedFloat waypointStartWidth = 0.05f;
        public SharedFloat waypointEndWidth = 0.05f;
        public SharedColor waypointStartColor = Color.yellow;
        public SharedColor waypointEndColor = Color.yellow;

		#endregion

		#region Fields
		protected LineRenderer waypointRenderer;
        private NavMeshPath myNavPath = null;
		#endregion

		#region Properties
		protected AllyMember allyMember
        {
            get
            {
                if (_thisAlly == null)
                    _thisAlly = GetComponent<AllyMember>();

                return _thisAlly;
            }
        }
		private AllyMember _thisAlly = null;

		protected AllyEventHandler myEventHandler
        {
            get
            {
                if(_myEventHandler == null)
                    _myEventHandler = GetComponent<AllyEventHandler>();

                return _myEventHandler;
            }
        }
        private AllyEventHandler _myEventHandler = null;

		protected NavMeshAgent myNavMesh
        {
            get
            {
                if (_myNavMesh == null)
                    _myNavMesh = GetComponent<NavMeshAgent>();

                return _myNavMesh;
            }
        }
        NavMeshAgent _myNavMesh = null;
		#endregion

		#region Overrides
		public override void OnStart()
		{
		    if(myNavMesh == null)
            {
                Debug.LogError("No Nav Mesh Found On Agent");
                OnEnd();
            }
        }

		public override TaskStatus OnUpdate()
		{
            UpdateWaypointRendererMain();
            return TaskStatus.Success;
		}
		#endregion

		#region Helpers
        protected void UpdateWaypointRendererMain()
        {
            if (waypointRenderer != null && waypointRenderer.enabled == false)
            {
                waypointRenderer.enabled = true;
            }
            else if (waypointRenderer == null)
            {
                waypointRenderer = this.gameObject.AddComponent<LineRenderer>();
                if (waypointMaterial != null && waypointMaterial.Value != null)
                    waypointRenderer.material = waypointMaterial.Value;

                waypointRenderer.startWidth = waypointStartWidth.Value;
                waypointRenderer.endWidth = waypointEndWidth.Value;
                waypointRenderer.startColor = waypointStartColor.Value;
                waypointRenderer.endColor = waypointEndColor.Value;
            }

            myNavPath = myNavMesh.path;

            waypointRenderer.positionCount = myNavPath.corners.Length;

            for (int i = 0; i < myNavPath.corners.Length; i++)
            {
                waypointRenderer.SetPosition(i, myNavPath.corners[i]);
            }
        }
		#endregion
	}
}