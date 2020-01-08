using UnityEngine;
using BehaviorDesigner.Runtime;
using RTSCoreFramework;

namespace RTSPrototype
{
	[System.Serializable]
	public class SharedTacticsItem : SharedVariable<AllyTacticsItem>
	{
		public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
		public static implicit operator SharedTacticsItem(AllyTacticsItem value) { return new SharedTacticsItem { mValue = value }; }
	}
}