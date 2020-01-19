using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character.Abilities;
using RTSCoreFramework;
using Chronos;

namespace RTSPrototype
{
    public abstract class AbilityBehaviourTPC : AbilityBehaviour
    {
        protected RTSGameMasterWrapper gamemaster => RTSGameMasterWrapper.thisInstance;

        public override abstract void Use(GameObject target = null);

        public Opsive.UltimateCharacterController.Character.Abilities.Ability TPCAbility
        {
            get
            {
                if (_TPCAbility == null)
                    _TPCAbility = GetTPCAbility();

                return _TPCAbility;
            }
        }
        private Opsive.UltimateCharacterController.Character.Abilities.Ability _TPCAbility = null;

        protected override void PlayAbilityAnimation()
        {
            if(CanUseAbility())
            {
                TPCAbility.StartAbility();
                //allyEventHandler.CallEventToggleIsUsingAbility(true);
                //Invoke("StopAbilityAnimation", config.GetAbilityAnimationTime());
            }
        }

        public override void StopAbilityAnimation()
        {
            TPCAbility.StopAbility();
            //allyEventHandler.CallEventToggleIsUsingAbility(false);
        }

        protected virtual Opsive.UltimateCharacterController.Character.Abilities.Ability GetTPCAbility()
        {
            //Override To Get the Actual TPC Ability
            return null;
        }

        public override bool CanUseAbility()
        {
            return TPCAbility != null && TPCAbility.CanStartAbility();
        }

        protected override void PlayParticleEffect()
        {
            var _particlePrefab = config.GetParticlePrefab();
            if (_particlePrefab == null) return;
            var _particleObject = Instantiate(
                _particlePrefab,
                transform.position,
                _particlePrefab.transform.rotation
            );
            _particleObject.transform.parent = transform; // set world space in prefab if required
            var _particles = _particleObject.GetComponent<ParticleSystem>();
            var _timeline = _particleObject.AddComponent<Timeline>();
            _timeline.mode = TimelineMode.Global;
            _timeline.globalClockKey = gamemaster.allyClocksName;
            _timeline.rewindable = false;
            StartCoroutine(DestroyParticleWhenFinished(_particleObject));
        }

        protected override IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {            
            ParticleSystem particlePrefabComp = particlePrefab.GetComponent<ParticleSystem>();
            Timeline _timeline = particlePrefab.GetComponent<Timeline>();       
            float _time = 0.0f;
            float _duration = particlePrefabComp.main.duration;
            float _previousTime = _timeline.time;
            while (particlePrefabComp != null && _timeline != null && _time < _duration)
            {
                yield return new WaitForSeconds(0.2f);
                _time += (_timeline.time - _previousTime);
                _previousTime = _timeline.time;

                if (_timeline.timeScale == 0)
                {
                    if (particlePrefabComp.isPaused == false)
                    {
                        particlePrefabComp.Pause();
                    }
                }
                else
                {
                    if (particlePrefabComp.isPaused)
                    {
                        particlePrefabComp.Play();
                    }
                }             
            }
            yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            Destroy(particlePrefab);
            particlePrefabComp = null;
            yield return new WaitForEndOfFrame();
        }

        protected override void OnAllyDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            base.OnAllyDeath(position, force, attacker);
            //Stop The Ability When Ally Dies
            if (TPCAbility != null && TPCAbility.IsActive)
            {
                StopAbilityAnimation();
            }
        }
    }
}