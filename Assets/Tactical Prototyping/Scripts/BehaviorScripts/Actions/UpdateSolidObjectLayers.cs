using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Character;
using System.Collections.Generic;

namespace RTSPrototype
{
	[TaskCategory("RPGPrototype/AllyMember")]
	[TaskDescription("Updates Solid Object Layers Property On CharacterLayerManager, Depending On FreeMoving and NavMovement Values.")]
	public class UpdateSolidObjectLayers : Action
	{
		#region Shared
		public SharedBool bIsFreeMoving;
		public SharedBool bHasSetDestination;
		#endregion

		#region PropertiesAndFields
		protected float collisionCheckDistance = 1f;
		protected bool bIsWaitingUntilNoCollisions = false;
		protected Collider[] colliders;
		protected List<Transform> uniqueTransforms = new List<Transform>();

		bool bPreviouslyIsFreeMoving = false;
		bool bIsNavMoving => bIsFreeMoving.Value == false && bHasSetDestination.Value;
		private LayerMask solidObjectCharIncludedLayers => gamemode.SolidObjectCharIncludedLayers;
		private LayerMask solidObjectCharExcludedLayers => gamemode.SolidObjectCharExcludedLayers;
		private LayerMask allyAndCharacterLayers => gamemode.AllyAndCharacterLayers;

		RTSGameModeWrapper gamemode => RTSGameModeWrapper.thisInstance;

		CharacterLayerManager layerManager
		{
			get
			{
				if (_layerManager == null)
				{
					_layerManager = GetComponent<CharacterLayerManager>();
				}
				return _layerManager;
			}
		}
		CharacterLayerManager _layerManager = null;
		#endregion

		#region Overrides
		public override TaskStatus OnUpdate()
		{
			if (gamemode == null) return TaskStatus.Failure;

			NormalUpdateWithChecks();

			return TaskStatus.Success;
		}
		#endregion

		#region Helpers
		void NormalUpdateWithChecks()
		{
			if (bIsWaitingUntilNoCollisions)
			{
				//Waiting Until No Longer Colliding With Other Characters
				UpdateWaitingUntilNoCollisionsAndLayers();
			}
			else if (bPreviouslyIsFreeMoving != bIsFreeMoving.Value)
			{
				bPreviouslyIsFreeMoving = bIsFreeMoving.Value;
				if (bPreviouslyIsFreeMoving == false)
				{
					//Not Currently Free Moving, Check For Near Collisions With Other Characters
					UpdateWaitingUntilNoCollisionsAndLayers();
				}
				else
				{
					//Only Update If FreeMoving Value Changed
					UpdateCharLayersSolidObjects(bPreviouslyIsFreeMoving);
				}
			}
		}

		void UpdateWaitingUntilNoCollisionsAndLayers()
		{
			bIsWaitingUntilNoCollisions = CheckForNearCharCollisions();
			//Debug.Log($"Waiting No Collisions: {bIsWaitingUntilNoCollisions}");
			if (bIsWaitingUntilNoCollisions == false)
			{
				//Reset Because No Collisions Were Found
				UpdateCharLayersSolidObjects(false);
			}
		}

		bool CheckForNearCharCollisions()
		{
			colliders = Physics.OverlapSphere(transform.position, collisionCheckDistance + 1, allyAndCharacterLayers);
			uniqueTransforms.Clear();
			foreach (Collider col in colliders)
			{
				Transform _root = col.transform.root;
				if (uniqueTransforms.Contains(_root)) continue;
				uniqueTransforms.Add(_root);
				if (_root != this.transform)
				{
					if (Vector3.Distance(this.transform.position, _root.position) <= collisionCheckDistance)
					{
						//Found A Near Collision, Wait...
						return true;
					}
				}
			}
			return false;
		}

		void UpdateCharLayersSolidObjects(bool _isFreeMoving)
		{
			layerManager.SolidObjectLayers = _isFreeMoving ?
				solidObjectCharIncludedLayers :
				solidObjectCharExcludedLayers;
		}
		#endregion
	}
}