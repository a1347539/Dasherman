using Photon.Pun;

namespace FYP.InGame
{
    public enum DamageType { 
        dashing = 0,
        physicalHit = 1,
        skillHit = 2
    }

    public interface IDamageable
    {
        public int currentHealth { get; }
        public int maxHealth { get; }

        public void takeDamage(int damaga, DamageType damageType, PhotonView pv = null);
    }
}