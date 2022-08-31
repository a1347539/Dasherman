using FYP.InGame.Photon;
using FYP.InGame.PlayerInstance;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FYP.InGame.Weapon
{
    public class WeaponController : MonoBehaviourPun
    {

        private PhotonView ownerCharacter;
        public PhotonView OwnerCharacter { get { return ownerCharacter; } set {
                ownerCharacter = value;
                GetComponent<WeaponAnimationController>().addCallbacks();
            } }

        private Transform weaponOffsetTransform { get {
                return ownerCharacter.gameObject.GetComponent<CharacterBuilder>().weaponOffsetTransform; 
            } }

        public List<ScriptableWeapon.Row[]> ranges;

        public int damage { get; private set; }
        public float attackSpeed { get; private set; }


        public void initialize(ScriptableWeapon sw, PhotonView ownerCharacter)
        {
            if (!photonView.IsMine) return;
            this.damage = sw.damage;
            this.attackSpeed = sw.attackSpeed;
            ranges = new List<ScriptableWeapon.Row[]>() {
                sw.rightRange,
                sw.upRange,
                sw.leftRange,
                sw.downRange,
            };
            // foreach (var range in ranges) printRangeMatrix(range);
            photonView.RPC("initializeRPC", RpcTarget.AllBuffered, ownerCharacter.ViewID);

        }

        [PunRPC]
        private void initializeRPC(int ownerCharacterPVID) {
            OwnerCharacter = PhotonView.Find(ownerCharacterPVID);
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            GetComponent<WeaponAnimationController>().setAttackAnimationSpeed(attackSpeed);
            transform.SetParent(weaponOffsetTransform);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        public static void printRangeMatrix(ScriptableWeapon.Row[] matrix)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < ScriptableWeapon.SIZE; j++)
            {
                for (int k = 0; k < ScriptableWeapon.SIZE; k++)
                {
                    if (!matrix[j].column[k])
                    {
                        sb.Append('0');
                    }
                    else
                    {
                        sb.Append('1');
                    }
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }
    }
}