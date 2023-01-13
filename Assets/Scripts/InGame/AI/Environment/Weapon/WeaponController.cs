using FYP.InGame.AI.Environment.Character;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment.Weapon
{
    public class WeaponController : MonoBehaviour
    {
        private GameObject ownerCharacter;
        public GameObject OwnerCharacter
        {
            get { return ownerCharacter; }
            set
            {
                ownerCharacter = value;
                GetComponent<WeaponAnimationController>().addCallbacks();
            }
        }

        private Transform weaponOffsetTransform
        {
            get
            {
                return ownerCharacter.GetComponent<CharacterBuilder>().weaponOffsetTransform;
            }
        }

        public List<InGame.Weapon.ScriptableWeapon.Row[]> ranges;

        public int damage { get; private set; }
        public float attackSpeed { get; private set; }


        public void initialize(InGame.Weapon.ScriptableWeapon sw, GameObject ownerCharacter)
        {
            this.damage = sw.damage;
            this.attackSpeed = sw.attackSpeed;
            ranges = new List<InGame.Weapon.ScriptableWeapon.Row[]>() {
                sw.rightRange,
                sw.upRange,
                sw.leftRange,
                sw.downRange,
            };

            OwnerCharacter = ownerCharacter;

            this.damage = damage;
            this.attackSpeed = attackSpeed;
            GetComponent<WeaponAnimationController>().setAttackAnimationSpeed(attackSpeed);
            transform.SetParent(weaponOffsetTransform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

        }
    }
}