using FYP.InGame.AI.Environment.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FYP.InGame.AI.Environment.Character.CharacterController;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;

namespace FYP.InGame.AI.Agent
{
    public class Actuator : MonoBehaviour
    {
        public void changeFacing(int facingIndex) {
            GetComponent<CharacterMovement>().changeFacing(facingIndex);
        }

        public void move(int distanceToTravel)
        {
            GetComponent<CharacterMovement>().move(distanceToTravel);
            GetComponent<DashingGameAgent>().AddReward(-GetComponent<DashingGameAgent>().aiManager.microReward);
            GetComponent<DashingGameAgent>().isMove = 0;
        }

        public void attack() {

            GetComponent<CharacterController>().CharacterState = CharacterStates.attacking;
            GetComponent<DashingGameAgent>().AddReward(-GetComponent<DashingGameAgent>().aiManager.microReward);
            GetComponent<DashingGameAgent>().isAttack = 0;
        }

        public void rechargeMana() {
            GetComponent<CharacterVital>().rechargeMana();
            GetComponent<DashingGameAgent>().AddReward(GetComponent<DashingGameAgent>().aiManager.microReward);
            GetComponent<DashingGameAgent>().isRechargeMana = 0;
        }
    }
}