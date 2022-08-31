using FYP.InGame.PlayerInstance;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using FYP.InGame.Map;
using Photon.Pun;
using static FYP.InGame.PlayerInstance.CharacterController;
using FYP.InGame.PlayerItemInstance;

namespace FYP.InGame.AI
{
    public class AIInput : MonoBehaviourPun
    {
        private CharacterMovement movement;
        private CharacterVital vital;
        private PlayerInstance.CharacterController controller;

        private void Awake()
        {
            movement = GetComponent<CharacterMovement>();
            vital = GetComponent<CharacterVital>();
            controller = GetComponent<PlayerInstance.CharacterController>();
        }

        private void Start()
        {
            StartCoroutine(continuousSkillActionCoroutine());
        }

        private void Update()
        {
            // vital.rechargeMana();
        }

        private IEnumerator continuousMoveActionCoroutine()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(1f);
                randomMove();
            }
        }

        private IEnumerator continuousAttackActionCoroutine()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(1f);
                randomAttack();
            }
        }

        private IEnumerator continuousSkillActionCoroutine()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(1f);
                randomSkill();
            }
        }

        private void randomMove() {

            if (controller.CharacterState != CharacterStates.idle)
            {
                return;
            }

            int facing = Random.Range(0, 4);
            movement.Facing = facing;
            int maxDistance = movement.calculateMaxDistances(facing);
            maxDistance = Mathf.Min(maxDistance, vital.currentMana);

            int distanceToTravel = Random.Range(0, maxDistance + 1);

            print($"facing {facing}, distance {distanceToTravel}");

            if (distanceToTravel == 0) { return; }

            // facing right: 0, facing up: 1, facing left: 2, facing down: 3
            Point p = controller.currentPoint;
            if (facing == 2 || facing == 3)
            {
                distanceToTravel = -distanceToTravel;
            }

            if (facing == 1 || facing == 3)
            {
                p.y -= distanceToTravel;
            }
            else if (facing == 0 || facing == 2)
            {
                p.x += distanceToTravel;
            }
            p.print();
            // vital.setMana(-Mathf.Abs(distanceToTravel));

            photonView.RPC("move", RpcTarget.All, p);
        }

        private void randomAttack() {
            if (controller.CharacterState != CharacterStates.idle)
            {
                return;
            }

            int facing = Random.Range(0, 4);
            movement.Facing = facing;
            controller.CharacterState = CharacterStates.attacking;
            controller.attack(facing);
        }

        private void randomSkill() {
            if (controller.CharacterState != CharacterStates.idle)
            {
                return;
            }

            PlayerItemManager.Instance.globalPlayerItems[Random.Range(0, PlayerItemManager.Instance.globalPlayerItems.Count)].onSelected(photonView);
        }
    }
}