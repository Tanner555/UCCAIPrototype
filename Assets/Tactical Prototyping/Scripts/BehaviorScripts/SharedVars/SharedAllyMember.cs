using UnityEngine;
using BehaviorDesigner.Runtime;
using RTSCoreFramework;

namespace RTSPrototype
{
	[System.Serializable]
	public class SharedAllyMember : SharedVariable<AllyMember>
	{
		public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
		public static implicit operator SharedAllyMember(AllyMember value) { return new SharedAllyMember { mValue = value }; }
	}
}