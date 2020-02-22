using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using RTSCoreFramework;
using BaseFramework;
#if RTSAStarPathfinding
using Pathfinding;
#endif

namespace RPGPrototype { 
    public class RPGCharacterSpawner : CharacterSpawner
    {
        #region CharacterSetupFields
        //Character Setup Fields
        [Header("Character Setup Fields")]
        [Header("Ally Instance Setup Fields")]
        [SerializeField]
        protected AllyComponentSpecificFieldsRPG AllySpecificComponentsToSetUp;

        [Header("All Allies Setup Fields")]
        [SerializeField]
        protected RPGAllyComponentSetupObject AllAllyComponentFieldsObject;

        protected AllyComponentsAllCharacterFieldsRPG AllAllyComponentFields
        {
            get { return AllAllyComponentFieldsObject.AllyComponentSetupFields; }
        }

        protected 
        #endregion

        #region Properties
        RPGGameMode gamemode => RPGGameMode.thisInstance;
        RPGGameMaster gamemaster => RPGGameMaster.thisInstance;
        #endregion

        #region CharacterBuilder_BuildCharacter
        protected override IEnumerator CharacterBuilder_BuildCharacter()
        {
            yield return new WaitForSeconds(0.05f);
        }
        #endregion

        #region CharacterSetup_SetupCharacter
        protected override IEnumerator CharacterSetup_SetupCharacter()
        {
            yield return new WaitForSeconds(0f);
            //Immediately Add Event Handler For Easy Access
            spawnedGameObject.AddComponent<AllyEventHandlerRPG>();
        }
        #endregion

        #region ItemBuilder_BuildItem
        protected override IEnumerator ItemBuilder_BuildItem()
        {
            yield return new WaitForSeconds(0f);
        }
        #endregion

        #region CharacterBuilder_UpdateCharacter
        protected override IEnumerator CharacterBuilder_UpdateCharacter()
        {
            yield return new WaitForSeconds(0f);
        }
        #endregion

        #region CharacterSetup_UpdateCharacterSetup
        protected override IEnumerator CharacterSetup_UpdateCharacterSetup()
        {
            spawnedGameObject.layer = gamemode.SingleAllyLayer;
            spawnedGameObject.tag = gamemode.AllyTag;

            if (AllySpecificComponentsToSetUp.bBuildCharacterCompletely)
            {
                spawnedGameObject.AddComponent<AllyStatControllerRPG>();
                //spawnedGameObject.AddComponent<AllyActionQueueController>();

                //Spawn Child Objects
                if (AllAllyComponentFields.bBuildLOSChildObject)
                {
                    var _losObject = new GameObject("LOSObject");
                    _losObject.transform.parent = spawnedGameObject.transform;
                    _losObject.transform.localPosition = AllAllyComponentFields.LOSChildObjectPosition;
                    _losObject.transform.localEulerAngles = AllAllyComponentFields.LOSChildObjectRotation;
                    AllySpecificComponentsToSetUp.LOSChildObjectTransform = _losObject.transform;
                }

                if (AllySpecificComponentsToSetUp.bBuildEnemyHealthBar &&
                    AllAllyComponentFields.EnemyHealthBarPrefab != null)
                {
                    var _enemyHealthBar = GameObject.Instantiate(AllAllyComponentFields.EnemyHealthBarPrefab,
                        spawnedGameObject.transform, false);
                    var _rect = _enemyHealthBar.GetComponent<RectTransform>();
                    _rect.localPosition = AllAllyComponentFields.EnemyHealthBarPosition;
                    _rect.localEulerAngles = AllAllyComponentFields.EnemyHealthBarRotation;
                    _rect.sizeDelta = AllAllyComponentFields.EnemyHealthSizeDelta;
                    _rect.anchorMin = new Vector2(0.5f, 0.5f);
                    _rect.anchorMax = new Vector2(0.5f, 0.5f);
                    _rect.pivot = new Vector2(0.5f, 0.5f);
                    _rect.localScale = AllAllyComponentFields.EnemyHealthLocalScale;

                    //Attempt To Set Health and ActiveBar Images By Looking For Them in Code
                    foreach (var _image in _enemyHealthBar.transform.GetComponentsInChildren<Image>(true))
                    {
                        if (_image.name.ToLower().Contains("health"))
                        {
                            AllySpecificComponentsToSetUp.EnemyHealthBarImage = _image;
                        }
                        else if (_image.name.ToLower().Contains("active"))
                        {
                            AllySpecificComponentsToSetUp.EnemyActiveBarImage = _image;
                        }
                    }
                }

                if (AllAllyComponentFields.bBuildAllyIndicatorSpotlight &&
                    AllAllyComponentFields.AllyIndicatorSpotlightPrefab != null)
                {
                    var _spotlight = GameObject.Instantiate(AllAllyComponentFields.AllyIndicatorSpotlightPrefab,
                        spawnedGameObject.transform, false);
                    _spotlight.transform.localPosition = AllAllyComponentFields.AllyIndicatorSpotlightPosition;
                    _spotlight.transform.localEulerAngles = AllAllyComponentFields.AllyIndicatorSpotlightRotation;
                    AllySpecificComponentsToSetUp.AllyIndicatorSpotlightInstance = _spotlight;
                    _spotlight.GetComponent<Light>().enabled = false;
                }
            }

            // Wait For 0.05 Seconds
            yield return new WaitForSeconds(0.05f);

            //Delay Adding These Components
            //Added Redundant Check For Now For Organization
            if (AllySpecificComponentsToSetUp.bBuildCharacterCompletely)
            {
                spawnedGameObject.AddComponent<AllyMemberRPG>();
                spawnedGameObject.AddComponent<AIControllerRPG>();
                //needs to be fixed
                //spawnedGameObject.AddComponent<RPGCharacter>();
                spawnedGameObject.AddComponent<RPGSpecialAbilities>();
                //spawnedGameObject.AddComponent<RPGWeaponSystem>();
                spawnedGameObject.AddComponent<AllyVisualsRPG>();
                //spawnedGameObject.AddComponent<AllyTacticsRPG>();
            }

#if RTSAStarPathfinding
            if (AllySpecificComponentsToSetUp.bBuildCharacterCompletely && 
                AllAllyComponentFields.bUseAStarPath &&
                spawnedGameObject.GetComponent<Seeker>() == null &&
                spawnedGameObject.GetComponent<AIPath>() == null)
            {
                var _aiStarSeeker = spawnedGameObject.AddComponent<Seeker>();
                var _aiStarAIPath = spawnedGameObject.AddComponent<AIPath>();
                _aiStarSeeker.graphMask = GraphMask.FromGraphName(AllAllyComponentFields.aStar_traversableGraphs);
                _aiStarAIPath.radius = AllAllyComponentFields.aStar_Radius;
                _aiStarAIPath.height = AllAllyComponentFields.aStar_Height;
                _aiStarAIPath.canSearch = AllAllyComponentFields.aStar_CanSearch;
                _aiStarAIPath.repathRate = AllAllyComponentFields.aStar_RepathRate;
                _aiStarAIPath.canMove = AllAllyComponentFields.aStar_CanMove;
                _aiStarAIPath.maxSpeed = AllAllyComponentFields.aStar_MaxSpeed;
                _aiStarAIPath.orientation = AllAllyComponentFields.aStar_Orientation;
                _aiStarAIPath.enableRotation = AllAllyComponentFields.aStar_EnableRotation;
                _aiStarAIPath.pickNextWaypointDist = AllAllyComponentFields.aStar_PickNextWaypointDistance;
                _aiStarAIPath.slowdownDistance = AllAllyComponentFields.aStar_SlowdownDistance;
                _aiStarAIPath.endReachedDistance = AllAllyComponentFields.aStar_EndReachedDistance;
                _aiStarAIPath.alwaysDrawGizmos = AllAllyComponentFields.aStar_AlwaysDrawGizmos;
                _aiStarAIPath.whenCloseToDestination = AllAllyComponentFields.aStar_CloseToDestination;
                _aiStarAIPath.constrainInsideGraph = AllAllyComponentFields.aStar_ConstrainInsideGraph;
            }
#endif

            //Call Ally Init Comps Event
            var _eventHandler = spawnedGameObject.GetComponent<AllyEventHandler>();
            _eventHandler.CallInitializeAllyComponents(AllySpecificComponentsToSetUp, AllAllyComponentFields);
        }
        #endregion

        #region Helpers
        void TryRetrievingExistingInitData()
        {

        }
        #endregion
    }
}