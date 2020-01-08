using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using UnityEngine.AI;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
    [TaskDescription("Disables Waypoint Renderer If It Exists and Is Enabled.")]
	public class ResetWaypointRenderer : Action
	{
		#region Properties
		LineRenderer waypointRenderer
		{
			get
			{
				if(_waypointRenderer == null)
				{
					_waypointRenderer = GetComponent<LineRenderer>();
				}
				return _waypointRenderer;
			}
		}
		LineRenderer _waypointRenderer = null;
		#endregion

		#region Overrides
		public override void OnStart()
		{
		
		}

		public override TaskStatus OnUpdate()
		{
			DisableWaypointRenderer();
			return TaskStatus.Success;
		}
		#endregion

		#region Helpers
		void DisableWaypointRenderer()
        {
            if (waypointRenderer != null)
            {
                waypointRenderer.enabled = false;
            }
        }
		#endregion
	}
}