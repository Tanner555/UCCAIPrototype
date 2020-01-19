using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTSCoreFramework;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Audio;

namespace RTSPrototype
{
    public class AllyVisualsWrapper : AllyVisuals
    {
        #region Fields
        AudioClipSet damageSounds;
        AudioClipSet deathSounds;
        AudioSource damageSoundSource;
        AudioSource deathSoundSource;
        #endregion

        #region Overrides
        protected override void OnAllyInitComponents(RTSAllyComponentSpecificFields _specific, RTSAllyComponentsAllCharacterFields _allFields)
        {
            base.OnAllyInitComponents(_specific, _allFields);
            var _allfieldsWrapper = (RTSAllyComponentsAllCharacterFieldsWrapper)_allFields;
            damageSounds = _allfieldsWrapper.damageSounds;
            deathSounds = _allfieldsWrapper.deathSounds;
        }

        protected override void OnHealthAfterTakeDamage(int amount, Vector3 position, Vector3 force, AllyMember _instigator, Collider hitCollider)
        {
            base.OnHealthAfterTakeDamage(amount, position, force, _instigator, hitCollider);
            if (BloodParticles != null)
            {
                ObjectPool.Instantiate(BloodParticles, position, Quaternion.identity);
            }
            if(damageSounds.AudioClips.Length > 0)
            {
                if (damageSoundSource != null && damageSoundSource.isPlaying)
                {
                    damageSoundSource.Stop();
                }
                damageSoundSource = damageSounds.PlayAudioClip(gameObject);
            }
        }

        protected override void HandleDeath(Vector3 position, Vector3 force, GameObject attacker)
        {
            if (deathSounds.AudioClips.Length > 0)
            {
                if (deathSoundSource != null && deathSoundSource.isPlaying)
                {
                    deathSoundSource.Stop();
                }
                deathSoundSource = deathSounds.PlayAudioClip(gameObject);
            }
            base.HandleDeath(position, force, attacker);
        }
        #endregion
    }
}