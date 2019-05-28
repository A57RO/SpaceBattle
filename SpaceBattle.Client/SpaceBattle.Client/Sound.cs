using System.Media;
using SpaceBattle.Data.Entities;

namespace SpaceBattle.Client
{
    public static class Sound
    {
        public static SoundPlayer Thrust = new SoundPlayer(Properties.Resources.Thrust);
        public static SoundPlayer ShotLaser = new SoundPlayer(Properties.Resources.ShotLaser);
        public static SoundPlayer HitHull = new SoundPlayer(Properties.Resources.HitHull);
        public static SoundPlayer HitShield = new SoundPlayer(Properties.Resources.HitShield);
        public static SoundPlayer Explode = new SoundPlayer(Properties.Resources.Explode);

        public static void PlaySoundsAtBeginAct(Player player)
        {
            if (player.WeaponsFiring)
                ShotLaser.Play();
            else if (player.ThrustersRunning)
                Thrust.Play();
        }

        public static void PlaySoundsAtEndAct(Player player)
        {
            if (player.ShieldTakingHit)
                HitShield.Play();
            else if (player.HullTakingHit)
                HitHull.Play();
        }
    }
}