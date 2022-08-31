using FYP.InGame.PlayerInstance;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.Weapon
{
    public class WeaponAnimationController : MonoBehaviourPun
    {
        [SerializeField]
        private Animator spriteAnimator;
        private float attackAnimationSpeed;

        private void Awake()
        {
            if (!photonView.IsMine) return;
            UIManager.onSpriteEnabled += handleSpriteEnabled;
        }

        public void addCallbacks()
        {
            if (!photonView.IsMine) return;
            GetComponent<WeaponController>().OwnerCharacter.GetComponent<CharacterMovement>().onFacingIndexChanged += handleSetAnimatorValue;
            GetComponent<WeaponController>().OwnerCharacter.GetComponent<CharacterAnimationManager>().startAttackAnimation += handlePlayAttackAnimation;
        }

        private void OnDestroy()
        {
            GetComponent<WeaponController>().OwnerCharacter.GetComponent<CharacterMovement>().onFacingIndexChanged -= handleSetAnimatorValue;
            GetComponent<WeaponController>().OwnerCharacter.GetComponent<CharacterAnimationManager>().startAttackAnimation -= handlePlayAttackAnimation;
            UIManager.onSpriteEnabled -= handleSpriteEnabled;
        }

        private void handleSpriteEnabled() {
            spriteAnimator.SetFloat("attackSpeed", attackAnimationSpeed);
        }

        private void handleSetAnimatorValue(int x, int y)
        {
            spriteAnimator.SetFloat("facingX", x);
            spriteAnimator.SetFloat("facingY", y);
        }

        public void setAttackAnimationSpeed(float speed)
        {
            attackAnimationSpeed = speed;
            spriteAnimator.SetFloat("attackSpeed", speed);
        }

        private void handlePlayAttackAnimation(int facingIndex)
        {
            spriteAnimator.SetBool("isAttacking", true);
            StartCoroutine(attackCoroutine());
        }

        IEnumerator attackCoroutine()
        {
            yield return new WaitForSeconds(1 / attackAnimationSpeed);
            spriteAnimator.SetBool("isAttacking", false);
        }
    }
}