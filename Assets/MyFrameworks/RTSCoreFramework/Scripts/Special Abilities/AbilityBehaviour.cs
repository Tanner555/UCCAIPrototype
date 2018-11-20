using System.Collections;
using UnityEngine;

namespace RTSCoreFramework
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        #region Properties
        protected virtual AudioSource audioSource
        {
            get
            {
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                    if (_audioSource == null)
                        _audioSource = gameObject.AddComponent<AudioSource>();

                }
                return _audioSource;
            }
        }
        private AudioSource _audioSource = null;

        protected AllyMember allyMember
        {
            get
            {
                if (_allyMember == null)
                    _allyMember = GetComponent<AllyMember>();

                return _allyMember;
            }
        }
        private AllyMember _allyMember = null;

        protected AllyEventHandler allyEventHandler
        {
            get
            {
                if (_allyEventHandler == null)
                    _allyEventHandler = GetComponent<AllyEventHandler>();

                return _allyEventHandler;
            }
        }
        private AllyEventHandler _allyEventHandler = null;

        #endregion

        #region Fields
        protected AbilityConfig config;

        protected const float PARTICLE_CLEAN_UP_DELAY = 20f;
        #endregion

        #region Getters
        public virtual bool CanUseAbility()
        {
            return false;
        }
        #endregion

        public abstract void Use(GameObject target = null);

        public virtual void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected virtual void PlayParticleEffect()
        {
            var _particlePrefab = config.GetParticlePrefab();
            if (_particlePrefab == null) return;
            var _particleObject = Instantiate(
                _particlePrefab,
                transform.position,
                _particlePrefab.transform.rotation
            );
            _particleObject.transform.parent = transform; // set world space in prefab if required
            _particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestroyParticleWhenFinished(_particleObject));
        }

        protected virtual IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected virtual void PlayAbilityAnimation()
        {
            //Override To Add Ability Animations Functionality
        }

        protected virtual void StopAbilityAnimation()
        {
            //Override To Add Ability Animations Functionality
        }

        protected virtual void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAbilitySound();
            if (abilitySound == null) return;
            audioSource.PlayOneShot(abilitySound);
        }
    }
}