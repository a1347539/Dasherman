using FYP.Global.InGame;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterAnimationManager : MonoBehaviourPun
    {
        public Action<int> startAttackAnimation = delegate { };

        [SerializeField]
        private Animator spriteAnimator;
        private CharacterMovement characterMovement;
        private CharacterBuilder builder;
        private CharacterAttackController attackController;

        private float speed;

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
            builder = GetComponent<CharacterBuilder>();
            attackController = GetComponent<CharacterAttackController>();
            characterMovement.onFacingIndexChanged += handleSetAnimatorFacing;

            if (!photonView.IsMine) return;
            UIManager.onSpriteEnabled += handleSpriteEnabled;
        }

        private void OnDestroy()
        {
            characterMovement.onFacingIndexChanged -= handleSetAnimatorFacing;
            if (!photonView.IsMine) return;
        }

        public void initializeAnimator() {
            spriteAnimator.SetFloat("deathAnimationSpeed", CharacterKeys.deathAnimationSpeed);
            spriteAnimator.SetFloat("attackAnimationType", (int)attackController.attackType);
        }

        private void handleSpriteEnabled()
        {
            spriteAnimator.SetFloat("attackSpeed", speed);
            spriteAnimator.SetFloat("attackAnimationType", (int)attackController.attackType);
        }

        private void handleSetAnimatorFacing(int x, int y)
        {
            spriteAnimator.SetFloat("facingX", x);
            spriteAnimator.SetFloat("facingY", y);
        }

        public void setAttackAnimationSpeed(float speed) {
            this.speed = speed;
            spriteAnimator.SetFloat("attackSpeed", speed);
        }

        public void changeStateAnimation(CharacterController.CharacterStates state) {
            switch (state)
            {
                case CharacterController.CharacterStates.idle:
                    spriteAnimator.SetBool("isCharging", false);
                    spriteAnimator.SetBool("isAttacking", false);
                    spriteAnimator.SetBool("isCastingSkill", false);
                    break;
                case CharacterController.CharacterStates.aiming:
                    spriteAnimator.SetBool("isCharging", true);
                    break;
                case CharacterController.CharacterStates.attacking:
                    spriteAnimator.SetBool("isAttacking", true);
                    startAttackAnimation?.Invoke(characterMovement.Facing);
                    break;
                case CharacterController.CharacterStates.useSkill:
                    spriteAnimator.SetBool("isCastingSkill", true);
                    break;
                case CharacterController.CharacterStates.takeItemEffect:
                    break;
                case CharacterController.CharacterStates.died:
                    spriteAnimator.SetBool("isDead", true);
                    break;
            }
        }
    }
}