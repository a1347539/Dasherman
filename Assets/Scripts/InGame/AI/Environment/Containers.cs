using FYP.InGame.AI;
using FYP.InGame.AI.Environment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public class Containers : MonoBehaviour
    {
        [SerializeField] public MapController mapController;
        [SerializeField] public GameManager gameManager;
        [SerializeField] public AIManager aiManager;
        [SerializeField] public MapObjectManager mapObjectManager;
        [SerializeField] public InputManager inputManager;
    }
}