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
using FYP.InGame.Map;
using MapController = FYP.InGame.AI.Environment.MapController;

namespace FYP.InGame.AI.Agent
{
    public class DashingGameAgent : Unity.MLAgents.Agent
    {
        public AIManager aiManager;
        private MapController mapController;

        private Sensor mySensor;
        private Actuator myActuator;

        public float timer = 0f;
        private float intervalBetweenMove = 0.5f;

        private int selfTeamNumber;

        public int isAiming = 0;
        public int facingIndex = 0;
        public int moveDistance = 0;
        public int isMove = 0;
        public int isAttack = 0;
        public int isRechargeMana = 0;

        private void Awake()
        {
            aiManager = transform.parent.parent.GetComponent<Containers>().aiManager;
            mapController = transform.parent.parent.GetComponent<Containers>().mapController;

            aiManager.onTimerEnd += handleTimerEnd;
            mySensor = GetComponent<Sensor>();
            myActuator = GetComponent<Actuator>();
            selfTeamNumber = GetComponent<CharacterBuilder>().teamNumber;
        }

        private void OnDestroy()
        {
            // AIManager.Instance.onTimerEnd -= handleTimerEnd;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
        }

        public void handleTimerEnd()
        {
            if (!enabled) return;
            EndEpisode();
        }

        public override void OnEpisodeBegin()
        {
            if (!enabled) return;
            aiManager.resetEnvironment();
            aiManager.resetTimer();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            // add own team number
            sensor.AddObservation(selfTeamNumber);
            sensor.AddObservation(mySensor.getHealth());
            sensor.AddObservation(mySensor.getMana());
            Vector2 p = mySensor.getSelfPosition();
            sensor.AddObservation(p);
            sensor.AddObservation(mapController.playableMapSize);

            int selfCellIndex = (int)(p.y * mapController.playableMapSize.x + p.x);

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

            // facing, moveDistance, shouldMove, shouldAttack, shouldRechargeMana
            if (GetComponent<CharacterController>().CharacterState != CharacterStates.idle && GetComponent<CharacterController>().CharacterState != CharacterStates.aiming)
                return;
            if (actions.DiscreteActions[4] == 1) { 
                myActuator.rechargeMana();
            }

            // keep the agent from moving to quickly
            if (timer > 0) return;

            if (actions.DiscreteActions[2] == 1)
            {
                if (actions.DiscreteActions[1] == 0) { isMove = 0; return; }
                myActuator.move(actions.DiscreteActions[0], actions.DiscreteActions[1]);
                timer = intervalBetweenMove;
            }
            //if (actions.DiscreteActions[3] == 1) myActuator.attack();

            // print($"{actions.DiscreteActions[0]}, {actions.DiscreteActions[1]}, {actions.DiscreteActions[2]}, {actions.DiscreteActions[3]}, {actions.DiscreteActions[4]}, {actions.DiscreteActions[5]}");

            AddReward(-1f / aiManager.durationInStep);
        }


        // Heuristic

        public static MouseButtonData mouseButtonData;

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> actions = actionsOut.DiscreteActions;
            actions[0] = facingIndex;
            GetComponent<CharacterMovement>().changeFacing(facingIndex);
            actions[1] = Math.Abs(moveDistance);
            actions[2] = isMove;
            actions[3] = isAttack;
            actions[4] = isRechargeMana;
        }
    }
}