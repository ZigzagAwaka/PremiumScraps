using UnityEngine;

namespace PremiumScraps.Utils
{
    public class Scrap
    {
        public string asset;
        public int rarity;
        public int behaviourId;
        public Scrap(string asset, int rarity) : this(asset, rarity, 0) { }
        public Scrap(string asset, int rarity, int behaviourId)
        {
            this.asset = asset;
            this.rarity = rarity;
            this.behaviourId = behaviourId;
        }
    }

    public class SfxId
    {
        public int audioId;
        public Vector3 position;
        public SfxId(int audioId, Vector3 position)
        {
            this.audioId = audioId;
            this.position = position;
        }
    }

    public class PlayerDir
    {
        public ulong playerId;
        public Vector3 direction;
        public PlayerDir(ulong playerId, Vector3 direction)
        {
            this.playerId = playerId;
            this.direction = direction;
        }
    }
}
