using FYP.InGame.AI.Agent;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FYP.Global.InputUtilities;
using Random = UnityEngine.Random;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterMovement : MonoBehaviour
    {
        // debug
        [SerializeField]
        private bool controllable = true;

        public Action<int, int> onFacingIndexChanged = delegate { };

        [SerializeField]
        private DirectionIndicator directionIndicator;

        private CharacterBuilder builder;
        private CharacterController controller;
        private CharacterVital vital;

        private int distanceToTravel = 0;

        private int facing = -1;

        public int Facing
        {
            get { return facing; }
            set
            {
                facing = value;
                // facing right: 0, facing up: 1, facing left: 2, facing down: 3
                switch (facing)
                {
                    case 0:
                        onFacingIndexChanged?.Invoke(1, 0);
                        break;
                    case 1:
                        onFacingIndexChanged?.Invoke(0, 1);
                        break;
                    case 2:
                        onFacingIndexChanged?.Invoke(-1, 0);
                        break;
                    case 3:
                        onFacingIndexChanged?.Invoke(0, -1);
                        break;
                }
            }
        }

        private void Awake()
        {
            builder = GetComponent<CharacterBuilder>();
            controller = GetComponent<CharacterController>();
            vital = GetComponent<CharacterVital>();
        }

        private void Start()
        {
            InputManager.onMouseHoldDown += handleGetCharacterFacingForMouseControl;
            //InputManager.onMouseLeftButtonUp += move;
            Facing = Random.Range(0, 4);
        }

        private void OnDestroy()
        {
            InputManager.onMouseHoldDown -= handleGetCharacterFacingForMouseControl;
            //InputManager.onMouseLeftButtonUp -= move;
        }

        public void handleGetCharacterFacingForMouseControl(MouseButtonData mouseButtonData, Vector2 position)
        {
            // debug
            if (!controllable) return;
            if (!builder.isInitialized) return;
            if (InputManager.isFKeyPressed) return;

            if (controller.CharacterState != CharacterController.CharacterStates.idle && controller.CharacterState != CharacterController.CharacterStates.aiming)
                return;
            if (distanceToTravel == 0)
            {
                // AI Heuristic
                // vital.rechargeMana(1);
                GetComponent<DashingGameAgent>().isRechargeMana = 1;
            }

            float angle = mouseButtonData.getAngleFromOnTouch(position);

            // facing left: 0, facing up: 1, facing right: 2, facing down: 3
            if (angle == -1) { return; }
            // AI Heuristic
            GetComponent<DashingGameAgent>().facingIndex = (Mathf.FloorToInt((angle + 45) / 90)) % 4;
            // Facing = (Mathf.FloorToInt((angle + 45) / 90)) % 4;

            if (Time.time - controller.startTimeToTryAttack < controller.attackFrames)
            {
                return;
            }
            calculateDraggedDistanceForMouseControl(facing, mouseButtonData, position);
        }

        private void calculateDraggedDistanceForMouseControl(int facing, MouseButtonData mouseButtonData, Vector2 position)
        {
            // facing vertical direction 
            int maxDistance = calculateMaxDistances(facing);
            maxDistance = Mathf.Min(maxDistance, vital.currentMana);
            Vector2 deltaPosition = mouseButtonData.getDeltaFromCurrentToOriginInWorldSpace(Camera.main.ScreenToWorldPoint(position));
            if (facing == 1 || facing == 3)
            {
                distanceToTravel = (int)(deltaPosition.y / builder.mapController.cellSize);
            }
            // facing horizontal direction 
            else if (facing == 0 || facing == 2)
            {
                distanceToTravel = (int)(deltaPosition.x / builder.mapController.cellSize);
            }
            if (facing == 0 || facing == 1)
            {
                distanceToTravel = Mathf.Min(distanceToTravel, maxDistance);
            }
            else if (facing == 2 || facing == 3)
            {
                distanceToTravel = Mathf.Max(distanceToTravel, -maxDistance);
            }
            // AI Heuristic
            GetComponent<DashingGameAgent>().moveDistance = distanceToTravel;
            directionIndicator.scaleIndicator(facing, distanceToTravel);
        }

        public int calculateMaxDistances(int facing)
        {
            bool isTileWalkable(Tile tile)
            {
                if (tile.tileState != Tile.TileStates.empty)
                {
                    if (tile.tileState == Tile.TileStates.hasPlayer)
                    {
                        if (tile.currentObjects.Any(go => builder.gameManager.isSameTeam(gameObject, go)))
                        {
                            print("a player in the same team");
                            return false;
                        }
                        else
                        {
                            // the players in the tile are all enemies 

                            if (tile.currentObjects.Any((go) =>
                            {
                                CharacterController.CharacterStates state =
                                    go.GetComponent<CharacterController>().CharacterState;
                                if (state == CharacterController.CharacterStates.takeItemEffect ||
                                state == CharacterController.CharacterStates.respawning)
                                {
                                    return true;
                                };
                                return false;
                            }))
                            {
                                // if any of the enemies are using skill or respawning
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        // meaning that the tile has a breakable object
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            int distance = 0;
            switch (facing)
            {
                case 0:
                    for (int i = controller.currentPoint.x + 1; i < builder.mapController.playableMapSize.x; ++i)
                    {
                        if (!isTileWalkable(builder.mapController.tileMatrix[controller.currentPoint.y][i]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 1:
                    for (int i = controller.currentPoint.y - 1; i >= 0; --i)
                    {
                        if (!isTileWalkable(builder.mapController.tileMatrix[i][controller.currentPoint.x]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 2:
                    for (int i = controller.currentPoint.x - 1; i >= 0; --i)
                    {
                        if (!isTileWalkable(builder.mapController.tileMatrix[controller.currentPoint.y][i]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 3:
                    for (int i = controller.currentPoint.y + 1; i < builder.mapController.playableMapSize.y; ++i)
                    {
                        if (!isTileWalkable(builder.mapController.tileMatrix[i][controller.currentPoint.x]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
            }
            return distance;
        }

        // AI-output In-let
        public void changeFacing(int index) {
            Facing = index;
        }

        // AI-output In-let
        public void move(int distanceToTravel)
        {
            // debug
            if (!controllable) return;

            if (controller.CharacterState != CharacterController.CharacterStates.idle)
            {
                return;
            }

            if (distanceToTravel == 0) { distanceToTravel = this.distanceToTravel; }

            int maxDistance = calculateMaxDistances(facing);
            maxDistance = Mathf.Min(maxDistance, vital.currentMana);
            distanceToTravel = Math.Clamp(distanceToTravel, -maxDistance, maxDistance);

            if (distanceToTravel == 0) { return; }
            Point p = controller.currentPoint;
            if (facing == 1 || facing == 3)
            {
                p.y -= distanceToTravel;
            }
            else if (facing == 0 || facing == 2)
            {
                p.x += distanceToTravel;
            }


            vital.setMana(-Mathf.Abs(distanceToTravel));
            controller.setCurrentPoint(p, true);
            this.distanceToTravel = 0;
            GetComponent<DashingGameAgent>().moveDistance = 0;
            directionIndicator.resetIndicator();

        }
    }
}