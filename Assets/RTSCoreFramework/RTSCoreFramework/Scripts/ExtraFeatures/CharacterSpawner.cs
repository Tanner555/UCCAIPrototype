using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTSCoreFramework
{
    public class CharacterSpawner : MonoBehaviour
    {
        #region Fields
        [Header("Spawner Fields")]
        [Tooltip("Used to Identify a Character")]
        public ECharacterType CharacterType;
        [Header("Gizmos Fields")]
        [Tooltip("Used to Determine Gizmos Color")]
        public bool isFriendlyAlly = false;
        public Color FriendlyColor = Color.green;
        public Color EnemyColor = Color.red;
        public float GizmosRadius = 1.5f;
        //Used For Adding Components and setting up character
        public GameObject spawnedGameObject = null;
        #endregion

        #region Properties
        protected RTSStatHandler statHandler
        {
            get { return RTSStatHandler.thisInstance; }
        }

        protected Color gizmosColor
        {
            get
            {
                return isFriendlyAlly ? FriendlyColor : EnemyColor;
            }
        }
        #endregion

        #region UnityMessages
        protected virtual void OnEnable()
        {
            StartCoroutine(GetReadyToInitializeAllyMember());
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, GizmosRadius);
        }
        #endregion

        #region Helpers
        protected virtual GameObject SpawnCharacterPrefab(GameObject _character, string _name)
        {
            GameObject _spawnedC = Instantiate(_character, transform.position, transform.rotation) as GameObject;
            if (isFriendlyAlly)
            {
                _spawnedC.name = _name;
            }
            return _spawnedC;
        }
        #endregion

        #region Initialization
        protected virtual IEnumerator GetReadyToInitializeAllyMember()
        {
            yield return new WaitForSeconds(0);
            CharacterStats _stats;
            if (statHandler != null && (_stats =
                statHandler.RetrieveAnonymousCharacterStats(CharacterType))
                .CharacterType != ECharacterType.NoCharacterType &&
                _stats.CharacterPrefab != null)
            {
                //Spawn The GameObject From Stats
                spawnedGameObject = SpawnCharacterPrefab(_stats.CharacterPrefab, _stats.CharacterType.ToString());
                //From CharacterBuilder-BuildCharacter
                yield return StartCoroutine(CharacterBuilder_BuildCharacter());
                //From CharacterSetup-SetupCharacter
                yield return StartCoroutine(CharacterSetup_SetupCharacter());
                //From ItemBuilder-BuildItem
                yield return StartCoroutine(ItemBuilder_BuildItem());
                //From CharacterBuilder-UpdateCharacter
                yield return StartCoroutine(CharacterBuilder_UpdateCharacter());
                //From CharacterSetup-UpdateCharacterSetup
                yield return StartCoroutine(CharacterSetup_UpdateCharacterSetup());
            }
            //Wait For All Initialization To Complete
            //Before Destroying This gameObject.
            Destroy(gameObject, 0.5f);
        }
        #endregion

        #region MethodsToOverride
        protected virtual IEnumerator CharacterBuilder_BuildCharacter()
        {
            yield return new WaitForSeconds(0);
        }

        protected virtual IEnumerator CharacterSetup_SetupCharacter()
        {
            yield return new WaitForSeconds(0);
        }

        protected virtual IEnumerator ItemBuilder_BuildItem()
        {
            yield return new WaitForSeconds(0);
        }

        protected virtual IEnumerator CharacterBuilder_UpdateCharacter()
        {
            yield return new WaitForSeconds(0);
        }

        protected virtual IEnumerator CharacterSetup_UpdateCharacterSetup()
        {
            yield return new WaitForSeconds(0);
        }
        #endregion
    }
}