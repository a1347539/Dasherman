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
        public void move(int facingIndex, int distanceToTravel)
        {
            GetComponent<CharacterMovement>().changeFacing(facingIndex);
            if (facingIndex == 2 || facingIndex == 3)
                GetComponent<CharacterMovement>().move(-distanceToTravel);
            else {
                GetComponent<CharacterMovement>().move(distanceToTravel);
            }
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