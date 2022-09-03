using FYP.Global.InGame;
using FYP.Global.Photon;
using FYP.InGame.Map;
using FYP.InGame.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static FYP.Global.InputUtilities;

namespace FYP.InGame.PlayerInstance
{
    public class CharacterController : MonoBehaviourPun, IPunObservable
    {
        public Action<Point, Point> onPointChanged = delegate { };
        public Action<int> onAttack = delegate { };

        public enum CharacterStates
        {
            idle = 0,
            aiming = 1,
            attacking = 2,
            useSkill = 3,
            takeItemEffect = 4,
            respawning = 5,
            died = 6,
        };

        private CharacterBuilder builder;
        private CharacterAnimationManager animationManager;
        private CharacterMovement characterMovement;
        private CharacterAttackController attackController;

        public Point currentPoint { get; private set; }

        public float attackFrames { get; private set; } = 0.085f;
        public float startTimeToTryAttack { get; private set; }
        private float attackSpeed;

        public float AttackSpeed { get { return attackSpeed; } set {
                attackSpeed = value;
                animationManager.setAttackAnimationSpeed(value);
            } }

        [SerializeField]
        private CharacterStates characterState;
        public CharacterStates CharacterState
        {
            get { return characterState; }
            set
            {
                onChangeCharacterState(characterState, value);
            }
        }

        public void setCurrentPoint(Point p, bool dashing)
        {
            onChangeCurrenPoint(currentPoint, p, dashing);
        }


        private void Awake()
        {
            builder = GetComponent<CharacterBuilder>();
            animationManager = GetComponent<CharacterAnimationManager>();
            characterMovement = GetComponent<CharacterMovement>();
            attackController = GetComponent<CharacterAttackController>();
        }

        private void Start()
        {
            if (!photonView.IsMine) return;
            if (photonView.IsRoomView) return;
            if (InputManager.Instance.isTouchControl)
            {
                InputManager.onTouchDown += handleMouseLeftButtonDown;
                InputManager.onTouchUp += handleMouseLeftButtonUp;
            }
            else
            {
                InputManager.onMouseLeftButtonDown += handleMouseLeftButtonDown;
                InputManager.onMouseLeftButtonUp += handleMouseLeftButtonUp;
            }
            
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            InputManager.onTouchDown -= handleMouseLeftButtonDown;
            InputManager.onTouchUp -= handleMouseLeftButtonUp;

            InputManager.onMouseLeftButtonDown -= handleMouseLeftButtonDown;
            InputManager.onMouseLeftButtonUp -= handleMouseLeftButtonUp;
        }

        private void onChangeCurrenPoint(Point oldPoint, Point newPoint, bool dashing)
        {
            print(newPoint.x + " " + newPoint.y);

            Vector2 cellCenterPosition = MapController.Instance.pointToTile(newPoint).worldPositionOfCellCenter;
            transform.position = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + MapController.Instance.characterSpriteOffsetInY
                );

            MapController.Instance.tileMatrix[oldPoint.y][oldPoint.x].objectExit(photonView);
            MapController.Instance.tileMatrix[newPoint.y][newPoint.x].objectEnter(photonView);
            currentPoint = newPoint;
            builder.Sprite.GetComponent<SortingGroup>().sortingOrder = newPoint.y;

            if (!dashing) { return; }
            onPointChanged?.Invoke(oldPoint, newPoint);
        }

        private void onChangeCharacterState(CharacterStates oldState, CharacterStates newState)
        {
            // call by self, but also all players after the variable is synced

            characterState = newState;
            animationManager.changeStateAnimation(newState);
            if (newState == CharacterStates.attacking)
            {
                attack(characterMovement.Facing);
            }
            else if (newState == CharacterStates.takeItemEffect) {
                characterMovement.Facing = 3;
            }
            else if (newState == CharacterStates.died)
            {
                StartCoroutine(characterDeathCoroutine());
            }
        }

        public void attack(int facing)
        {
            StartCoroutine(attackCoroutine(facing));
        }

        IEnumerator attackCoroutine(int facing)
        {
            yield return new WaitForSeconds(1 / attackSpeed);
            onAttack?.Invoke(facing);
            CharacterState = CharacterStates.idle;
        }
        IEnumerator characterDeathCoroutine()
        {
            // called by all games
            if (photonView.Owner == PhotonNetwork.LocalPlayer)
            {
                yield return new WaitForSeconds(1 / CharacterKeys.deathAnimationSpeed);
                MapController.Instance.exitFromTile(currentPoint, photonView);
                transform.position = CharacterKeys.DeadCharacterPosition;
                if (!photonView.IsRoomView)
                {
                    GameManager.onSelfDie?.Invoke();
                }
                PhotonEvents.InGameEvents.playerDiedEvent();
            }
        }

        private void handleMouseLeftButtonDown(Touch t)
        {
            if (t.fingerId == 0)
            {
                if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                    return;
                startTimeToTryAttack = Time.time;
                CharacterState = CharacterStates.aiming;
            }
        }

        private void handleMouseLeftButtonUp(Touch t)
        {
            if (t.fingerId == 0)
            {
                if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                    return;
                if (Time.time - startTimeToTryAttack < attackFrames)
                {
                    CharacterState = CharacterStates.attacking;
                }
                else { CharacterState = CharacterStates.idle; }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(characterState);
                stream.SendNext(currentPoint);
                stream.SendNext(attackController.physicalDamageScaling);
                stream.SendNext(attackController.magicDamageScaling);
            }
            else if (stream.IsReading)
            {
                characterState = (CharacterStates)stream.ReceiveNext();
                currentPoint = (Point)stream.ReceiveNext();
                attackController.physicalDamageScaling = (float)stream.ReceiveNext();
                attackController.magicDamageScaling = (float)stream.ReceiveNext();
            }
        }

        #region mouseControl

        private void handleMouseLeftButtonDown(MouseButtonData mouseButtonData)
        {
            if (InputManager.isFKeyPressed) return;
            if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                return;
            startTimeToTryAttack = Time.time;
            CharacterState = CharacterStates.aiming;
        }

        private void handleMouseLeftButtonUp(MouseButtonData mouseButtonData, Vector2 position)
        {
            if (InputManager.isFKeyPressed) {
                CharacterState = CharacterStates.idle;
                return; 
            }
            if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                return;
            if (Time.time - startTimeToTryAttack < attackFrames)
            {
                CharacterState = CharacterStates.attacking;
            }
            else { CharacterState = CharacterStates.idle; }

        }

            #endregion
        }
}