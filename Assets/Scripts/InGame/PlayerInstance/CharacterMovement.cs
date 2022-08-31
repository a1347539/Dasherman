using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FYP.InGame.Map;
using FYP.Global;
using FYP.InGame.Photon;
using Random = UnityEngine.Random;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterMovement : MonoBehaviourPun
    {
        public Action<int, int> onFacingIndexChanged = delegate { };

        [SerializeField]
        private DirectionIndicator directionIndicator;

        public Action<int> onFacingChanged = delegate { };

        private CharacterBuilder builder;
        private CharacterController controller;
        private CharacterVital vital;

        public int distanceToTravel { get; private set; } = 0;

        private int facing = -1;

        public int Facing { 
            get { return facing; }
            set { 
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
            if (!photonView.IsMine) return;
            if (photonView.IsRoomView) return;
            builder.onAnyCharacterAssignComponents += handleInitCharacter;
        }

        private void Start()
        {
            if (!photonView.IsMine) return;
            if (photonView.IsRoomView) return;
            if (InputManager.Instance.isTouchControl)
            {
                InputManager.onTouchMove += handleGetCharacterFacing;
                InputManager.onTouchUp += handleMove;
            }
            else
            {
                InputManager.onMouseHoldDown += handleGetCharacterFacingForMouseControl;
                InputManager.onMouseLeftButtonUp += handleMoveForMouseControl;
            }
        
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            builder.onAnyCharacterAssignComponents -= handleInitCharacter;

            InputManager.onTouchMove -= handleGetCharacterFacing;
            InputManager.onTouchUp -= handleMove;

            InputManager.onMouseHoldDown -= handleGetCharacterFacingForMouseControl;
            InputManager.onMouseLeftButtonUp -= handleMoveForMouseControl;

        }

        private void handleInitCharacter() {
            Facing = Random.Range(0, 4);
        }

        private void handleGetCharacterFacing(Touch t)
        {
            if (!builder.isInitialized) return;
            if (t.fingerId != 0) return;

            if (controller.CharacterState != CharacterController.CharacterStates.idle && controller.CharacterState != CharacterController.CharacterStates.aiming)
                return;
            if (distanceToTravel == 0)
            {
                vital.rechargeMana();
            }

            InputManager.TouchData td = InputManager.touchDatas.First(td => td.touchID == t.fingerId);
            float angle = td.getAngleFromOnTouch(t.position);

            // facing left: 0, facing up: 1, facing right: 2, facing down: 3
            if (angle == -1) { return; }
            Facing = (Mathf.FloorToInt((angle + 45) / 90)) % 4;

            if (Time.time - controller.startTimeToTryAttack < controller.attackFrames)
            {
                return;
            }
            calculateDraggedDistance(facing, t);
        }

        private void calculateDraggedDistance(int facing, Touch t) {
            // facing vertical direction 
            int maxDistance = calculateMaxDistances(facing);
            maxDistance = Mathf.Min(maxDistance, vital.currentMana);
            InputManager.TouchData td = InputManager.touchDatas.First(td => td.touchID == t.fingerId);
            Vector2 deltaPosition = td.getDeltaFromCurrentToOriginInWorldSpace(Camera.main.ScreenToWorldPoint(t.position));
            if (facing == 1 || facing == 3)
            {
                distanceToTravel = (int)(deltaPosition.y / MapController.Instance.cellSize);
            }
            // facing horizontal direction 
            else if (facing == 0 || facing == 2)
            {
                distanceToTravel = (int)(deltaPosition.x / MapController.Instance.cellSize);
            }
            if (facing == 0 || facing == 1)
            {
                distanceToTravel = Mathf.Min(distanceToTravel, maxDistance);
            }
            else if (facing == 2 || facing == 3)
            {
                distanceToTravel = Mathf.Max(distanceToTravel, -maxDistance);
            }
            directionIndicator.scaleIndicator(facing, distanceToTravel);
        }

        public int calculateMaxDistances(int facing) {
            bool isTileWalkable(Tile tile)
            {
                if (tile.tileState != Tile.TileStates.empty)
                {
                    if (tile.tileState == Tile.TileStates.hasPlayer)
                    {
                        if (tile.currentObjects.Any(pv => PlayerManager.isSameTeam(photonView, pv))) {
                            print("a player in the same team");
                            return false;
                        }
                        else 
                        {
                            // the players in the tile are all enemies 

                            if (tile.currentObjects.Any((pv) =>
                            {
                                CharacterController.CharacterStates state =
                                    ((GameObject)pv.Owner.TagObject).GetComponent<CharacterController>().CharacterState;
                                if (state == CharacterController.CharacterStates.takeItemEffect ||
                                state == CharacterController.CharacterStates.respawning)
                                {
                                    return true;
                                };
                                return false;
                            })) {
                                // if any of the enemies are using skill or respawning
                                return false;
                            } else {
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
                    for (int i = controller.currentPoint.x + 1; i < MapController.Instance.playableMapSize.x; ++i)
                    {
                        if (!isTileWalkable(MapController.Instance.tileMatrix[controller.currentPoint.y][i])) {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 1:
                    for (int i = controller.currentPoint.y-1; i >= 0; --i) {
                        if (!isTileWalkable(MapController.Instance.tileMatrix[i][controller.currentPoint.x])) {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 2:
                    for (int i = controller.currentPoint.x - 1; i >= 0; --i)
                    {
                        if (!isTileWalkable(MapController.Instance.tileMatrix[controller.currentPoint.y][i]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
                case 3:
                    for (int i = controller.currentPoint.y + 1; i < MapController.Instance.playableMapSize.y; ++i)
                    {
                        if (!isTileWalkable(MapController.Instance.tileMatrix[i][controller.currentPoint.x]))
                        {
                            return distance;
                        }
                        distance++;
                    }
                    break;
            }
            return distance;
        }

        private void handleMove(Touch t)
        {
            if (t.fingerId != 0) return;
            if (controller.CharacterState == CharacterController.CharacterStates.attacking)
            {
                directionIndicator.resetIndicator();
                return;
            }
                
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

            if (controller.CharacterState != CharacterController.CharacterStates.takeItemEffect)
            {
                vital.setMana(-Mathf.Abs(distanceToTravel));
                photonView.RPC("move", RpcTarget.All, p);
            }
            distanceToTravel = 0;
            directionIndicator.resetIndicator();
        }

        [PunRPC]
        private void move(Point p) {
            controller.setCurrentPoint(p, true);
        }

        #region mouseControl
        private void handleGetCharacterFacingForMouseControl(InputManager.MouseButtonData mouseButtonData, Vector2 position)
        {
            if (!builder.isInitialized) return;
            if (InputManager.isFKeyPressed) return;

            if (controller.CharacterState != CharacterController.CharacterStates.idle && controller.CharacterState != CharacterController.CharacterStates.aiming)
                return;
            if (distanceToTravel == 0)
            {
                vital.rechargeMana();
            }

            float angle = mouseButtonData.getAngleFromOnTouch(position);

            // facing left: 0, facing up: 1, facing right: 2, facing down: 3
            if (angle == -1) { return; }
            Facing = (Mathf.FloorToInt((angle + 45) / 90)) % 4;

            if (Time.time - controller.startTimeToTryAttack < controller.attackFrames)
            {
                return;
            }
            calculateDraggedDistanceForMouseControl(facing, mouseButtonData, position);
        }
        private void calculateDraggedDistanceForMouseControl(int facing, InputManager.MouseButtonData mouseButtonData, Vector2 position)
        {
            // facing vertical direction 
            int maxDistance = calculateMaxDistances(facing);
            maxDistance = Mathf.Min(maxDistance, vital.currentMana);
            Vector2 deltaPosition = mouseButtonData.getDeltaFromCurrentToOriginInWorldSpace(Camera.main.ScreenToWorldPoint(position));
            if (facing == 1 || facing == 3)
            {
                distanceToTravel = (int)(deltaPosition.y / MapController.Instance.cellSize);
            }
            // facing horizontal direction 
            else if (facing == 0 || facing == 2)
            {
                distanceToTravel = (int)(deltaPosition.x / MapController.Instance.cellSize);
            }
            if (facing == 0 || facing == 1)
            {
                distanceToTravel = Mathf.Min(distanceToTravel, maxDistance);
            }
            else if (facing == 2 || facing == 3)
            {
                distanceToTravel = Mathf.Max(distanceToTravel, -maxDistance);
            }
            directionIndicator.scaleIndicator(facing, distanceToTravel);
        }

        private void handleMoveForMouseControl(InputManager.MouseButtonData mouseButtonData, Vector2 pos)
        {
            if (controller.CharacterState == CharacterController.CharacterStates.attacking)
            {
                directionIndicator.resetIndicator();
                return;
            }

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
            print(facing);
            print(p.x + " " + p.y);
            print(controller.CharacterState);
            if (controller.CharacterState != CharacterController.CharacterStates.takeItemEffect &&
                controller.CharacterState != CharacterController.CharacterStates.died &&
                controller.CharacterState != CharacterController.CharacterStates.respawning)
            {
                vital.setMana(-Mathf.Abs(distanceToTravel));
                photonView.RPC("move", RpcTarget.All, p);
            }

            distanceToTravel = 0;
            directionIndicator.resetIndicator();
        }

        #endregion
    }
}