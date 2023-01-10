using FYP.InGame.AI.Environment;
using FYP.InGame.AI.Environment.Character;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace FYP.InGame.AI.Agent
{
    public class DashingGameAgent : Unity.MLAgents.Agent
    {
        private Sensor mySensor;
        private Actuator myActuator;

        private int selfTeamNumber;

        void Start()
        {
            mySensor = GetComponent<Sensor>();
            myActuator = GetComponent<Actuator>();
            selfTeamNumber = GetComponent<CharacterBuilder>().teamNumber;
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
            Sensor.processTileMatrix();
            while (!Sensor.isTileMatrixProcessed) { }
            List<int> mapStates = Sensor.processedTileMatrix;
            // make the cell the character is on an empty cell, for testing
            mapStates[selfCellIndex] = 0;
            foreach (int state in mapStates)
            {
                sensor.AddObservation(state);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // facing, moveDistance, shouldAttack, shouldRechargeMana
            myActuator.changeFacing(actions.DiscreteActions[0]);
            myActuator.move(actions.DiscreteActions[1]);
            if (actions.DiscreteActions[2] == 1)  myActuator.attack();
            if (actions.DiscreteActions[3] == 1) myActuator.rechargeMana();

            AddReward(1f / AIManager.Instance.durationInStep);
        }

    }
}