using FYP.InGame.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Photon.Pun.Demo.PunBasics;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace FYP.InGame.AI
{
    public class TesterAgent : Unity.MLAgents.Agent
    {
        void Start()
        {

        }

        public override void OnEpisodeBegin()
        {
        }

        public override void CollectObservations(VectorSensor sensor)
        {
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
        }
    }
}
