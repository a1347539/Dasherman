using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP.InGame.AI.Environment
{
    public enum DamageType
    {
        dashing = 0,
        physicalHit = 1,
        skillHit = 2
    }

    public interface IDamageable
    {
        public int currentHealth { get; }
        public int maxHealth { get; }

        public void takeDamage(int damaga, DamageType damageType, GameObject go = null);
    }
}