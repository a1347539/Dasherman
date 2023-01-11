using FYP.InGame.AI.Agent;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static FYP.Global.InputUtilities;

namespace FYP.InGame.AI.Environment.Character
{
    public class CharacterController : MonoBehaviour
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

        public Point currentPoint { get; private set; }

        public float attackFrames { get; private set; } = 0.085f;
        public float startTimeToTryAttack { get; private set; }
        private float attackSpeed;

        public float AttackSpeed
        {
            get { return attackSpeed; }
            set
            {
                attackSpeed = value;
                animationManager.setAttackAnimationSpeed(value);
            }
        }

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
        }

        private void Start()
        {
            InputManager.onMouseLeftButtonDown += handleMouseLeftButtonDown;
            InputManager.onMouseLeftButtonUp += handleMouseLeftButtonUp;
        }

        private void OnDestroy()
        {
            InputManager.onMouseLeftButtonDown -= handleMouseLeftButtonDown;
            InputManager.onMouseLeftButtonUp -= handleMouseLeftButtonUp;
        }

        private void onChangeCurrenPoint(Point oldPoint, Point newPoint, bool dashing)
        {
            // print(newPoint.x + " " + newPoint.y);

            Vector2 cellCenterPosition = MapController.Instance.pointToTile(newPoint).worldPositionOfCellCenter;
            transform.localPosition = new Vector3(
                cellCenterPosition.x,
                cellCenterPosition.y + MapController.Instance.characterSpriteOffsetInY
            );

            MapController.Instance.tileMatrix[oldPoint.y][oldPoint.x].objectExit(gameObject);
            MapController.Instance.tileMatrix[newPoint.y][newPoint.x].objectEnter(gameObject);
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
            else if (newState == CharacterStates.takeItemEffect)
            {
                characterMovement.Facing = 3;
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

        public void handleMouseLeftButtonDown(MouseButtonData mouseButtonData)
        {
            if (InputManager.isFKeyPressed) return;
            if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                return;
            startTimeToTryAttack = Time.time;
            CharacterState = CharacterStates.aiming;

            // AI Heuristic
            GetComponent<DashingGameAgent>().isAiming = 1;
        }

        public void handleMouseLeftButtonUp(int temp)
        {
            // AI Heuristic
            GetComponent<DashingGameAgent>().isMove = 1;
            GetComponent<DashingGameAgent>().isAiming = 0;

            if (InputManager.isFKeyPressed)
            {
                CharacterState = CharacterStates.idle;
                return;
            }
            if (CharacterState != CharacterStates.idle && CharacterState != CharacterStates.aiming)
                return;
            if (Time.time - startTimeToTryAttack < attackFrames)
            {
                // AI Heuristic
                // CharacterState = CharacterStates.attacking;
                GetComponent<DashingGameAgent>().isAttack = 1;
            }
            else { CharacterState = CharacterStates.idle; }
        }
    }
}