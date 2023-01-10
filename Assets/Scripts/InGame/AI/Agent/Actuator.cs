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
        }

        public void attack() {
            GetComponent<CharacterController>().CharacterState = CharacterStates.attacking;
        }

        public void rechargeMana() {
            GetComponent<CharacterVital>().rechargeMana(5);
        }
    }
}