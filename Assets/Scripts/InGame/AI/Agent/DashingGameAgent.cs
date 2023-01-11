using FYP.InGame.AI.Environment;
using FYP.InGame.AI.Environment.Character;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UIElements;
using static FYP.Global.InputUtilities;
using CharacterController = FYP.InGame.AI.Environment.Character.CharacterController;
using static FYP.InGame.AI.Environment.Character.CharacterController;

namespace FYP.InGame.AI.Agent
{
    public class DashingGameAgent : Unity.MLAgents.Agent
    {
        private Sensor mySensor;
        private Actuator myActuator;

        private int selfTeamNumber;

        public int isAiming = 0;
        public int facingIndex = 0;
        public int moveDistance = 0;
        public int isMove = 0;
        public int isAttack = 0;
        public int isRechargeMana = 0;

        private void Awake()
        {
            AIManager.Instance.onTimerEnd += handleTimerEnd;
            mySensor = GetComponent<Sensor>();
            myActuator = GetComponent<Actuator>();
            selfTeamNumber = GetComponent<CharacterBuilder>().teamNumber;
        }


        private void OnDestroy()
        {
            // AIManager.Instance.onTimerEnd -= handleTimerEnd;
        }

        public void handleTimerEnd()
        {
            if (!enabled) return;
            EndEpisode();
        }

        public override void OnEpisodeBegin()
        {
            if (!enabled) return;
            AIManager.Instance.resetEnvironment();
            AIManager.Instance.resetTimer();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // add own team number
            sensor.AddObservation(selfTeamNumber);
            sensor.AddObservation(mySensor.getHealth());
            sensor.AddObservation(mySensor.getMana());
            Vector2 p = mySensor.getSelfPosition();
            sensor.AddObservation(p);
            sensor.AddObservation(MapController.Instance.playableMapSize);

            int selfCellIndex = (int)(p.y * MapController.Instance.playableMapSize.x + p.x);

            // add all map states
            List<int> mapStates = mySensor.getProcessedTileMap();
            // make the cell the character is on an empty cell, for testing
            mapStates[selfCellIndex] = 0;
            foreach (int state in mapStates)
            {
                sensor.AddObservation(state);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // remove this part and change the conditions below accordingly if on Heuristic

            // shouldDoMovingAction, facing, moveDistance, shouldMove, shouldAttack, shouldRechargeMana
            if (GetComponent<CharacterController>().CharacterState != CharacterStates.idle && GetComponent<CharacterController>().CharacterState != CharacterStates.aiming)
                return;

            if (actions.DiscreteActions[0] == 1) myActuator.changeFacing(actions.DiscreteActions[1]);

            if (actions.DiscreteActions[3] == 1)
            {
                if (GetComponent<CharacterMovement>().Facing == 2 || GetComponent<CharacterMovement>().Facing == 3) myActuator.move(-actions.DiscreteActions[2]);
                else myActuator.move(actions.DiscreteActions[2]);
            }
            //if (actions.DiscreteActions[4] == 1) myActuator.attack();
            if (actions.DiscreteActions[5] == 1) myActuator.rechargeMana();

            // print($"{actions.DiscreteActions[0]}, {actions.DiscreteActions[1]}, {actions.DiscreteActions[2]}, {actions.DiscreteActions[3]}, {actions.DiscreteActions[4]}, {actions.DiscreteActions[5]}");

            AddReward(-1f / AIManager.Instance.durationInStep);
        }


        // Heuristic

        public static MouseButtonData mouseButtonData;

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> actions = actionsOut.DiscreteActions;
            actions[0] = isAiming;
            actions[1] = facingIndex;
            actions[2] = Math.Abs(moveDistance);
            actions[3] = isMove;
            actions[4] = isAttack;
            actions[5] = isRechargeMana;
        }
    }
}