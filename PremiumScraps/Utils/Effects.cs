using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace PremiumScraps.Utils
{
    internal class Effects
    {
        public enum DeathAnimation
        {
            Normal,  // classic death
            NoHead1,  // remove head from body
            Spring,  // remove head and replace it with spring
            Haunted,  // body moves a little after classic death
            Mask1,  // comedy mask attached to body
            Mask2,  // tragedy mask attached to body
            Fire,  // burned death
            CutInHalf,  // cut the body in half
            NoHead2  // same as NoHead but without sound
        }

        public static int NbOfPlayers()
        {
            return StartOfRound.Instance.connectedPlayersAmount + 1;
        }

        public static List<PlayerControllerB> GetPlayers(bool includeDead = false)
        {
            List<PlayerControllerB> rawList = Object.FindObjectsOfType<PlayerControllerB>().ToList();
            List<PlayerControllerB> updatedList = new List<PlayerControllerB>(rawList);
            foreach (var p in rawList)
            {
                if (p.playerSteamId <= 0 || (!includeDead && p.isPlayerDead))
                {
                    updatedList.Remove(p);
                }
            }
            return updatedList;
        }

        public static List<EnemyAI> GetEnemies(bool includeDead = false, bool includeCanDie = false)
        {
            List<EnemyAI> rawList = Object.FindObjectsOfType<EnemyAI>().ToList();
            List<EnemyAI> updatedList = new List<EnemyAI>(rawList);
            if (includeDead)
                return updatedList;
            foreach (var e in rawList)
            {
                if (e.isEnemyDead || (!includeCanDie && !e.enemyType.canDie))
                {
                    updatedList.Remove(e);
                }
            }
            return updatedList;
        }

        public static void Damage(PlayerControllerB player, int damageNb, CauseOfDeath cause = 0, int animation = 0, bool criticalBlood = true)
        {
            if (criticalBlood && player.health - damageNb <= 20)
                player.bleedingHeavily = true;
            player.DamagePlayer(damageNb, causeOfDeath: cause, deathAnimation: animation);
        }

        public static IEnumerator DamageHost(PlayerControllerB player, int damageNb, CauseOfDeath cause = 0, int animation = 0, bool criticalBlood = true)
        {
            yield return new WaitForEndOfFrame();
            Damage(player, damageNb, cause, animation, criticalBlood);
        }

        public static void Heal(ulong playerID, int health)
        {
            var player = StartOfRound.Instance.allPlayerScripts[playerID];
            player.health = health;
            player.criticallyInjured = false;
            player.bleedingHeavily = false;
            player.playerBodyAnimator.SetBool("Limp", false);
        }

        public static void Teleportation(PlayerControllerB player, Vector3 position, bool ship = false, bool exterior = false, bool interior = false)
        {
            if (ship)
            {
                player.isInElevator = true;
                player.isInHangarShipRoom = true;
                player.isInsideFactory = false;
            }
            if (exterior)
            {
                player.isInElevator = false;
                player.isInHangarShipRoom = false;
                player.isInsideFactory = false;
            }
            if (interior)
            {
                player.isInElevator = false;
                player.isInHangarShipRoom = false;
                player.isInsideFactory = true;
            }
            player.averageVelocity = 0f;
            player.velocityLastFrame = Vector3.zero;
            player.TeleportPlayer(position, true);
            player.beamOutParticle.Play();
            HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
        }

        public static void Explosion(Vector3 position, float range, int damage = 50, float physicsForce = 1)
        {
            Landmine.SpawnExplosion(position, true, range, range * 2.5f, damage, physicsForce);
        }

        public static void ExplosionLight(Vector3 position, float range, int damage = 10, float physicsForce = 1)
        {
            Landmine.SpawnExplosion(position, true, 0, range, damage, physicsForce);
        }

        public static IEnumerator ExplosionHostDeath(Vector3 position, float range, int damage = 50, float physicsForce = 1)
        {
            yield return new WaitForEndOfFrame();
            Explosion(position, range, damage, physicsForce);
        }

        public static void Knockback(Vector3 position, float range, int damage = 0, float physicsForce = 30)
        {
            Landmine.SpawnExplosion(position, false, 0, range, damage, physicsForce);
        }

        public static void DropItem(Vector3 placingPosition = default)
        {
            GameNetworkManager.Instance.localPlayerController.DiscardHeldObject(true, placePosition: placingPosition);
        }

        public static void Audio(int audioID, float volume)
        {
            RoundManager.PlayRandomClip(HUDManager.Instance.UIAudio, new AudioClip[] { Plugin.audioClips[audioID] }, randomize: false, oneShotVolume: volume);
        }

        public static void Audio(int audioID, Vector3 position, float volume, bool adjust = true)
        {
            var finalPosition = position;
            if (adjust)
                finalPosition += (Vector3.up * 2);
            AudioSource.PlayClipAtPoint(Plugin.audioClips[audioID], finalPosition, volume);
        }

        public static void Audio(int audioID, Vector3 clientPosition, float localVolume, float clientVolume, PlayerControllerB player)
        {
            if (player != null && GameNetworkManager.Instance.localPlayerController.playerClientId == player.playerClientId)
                Audio(audioID, localVolume);
            else
                Audio(audioID, clientPosition, clientVolume);
        }

        public static void Audio(int audioID, Vector3 position, float volume, float pitch, bool adjust = true)
        {
            var clip = Plugin.audioClips[audioID];
            var finalPosition = position;
            if (adjust)
                finalPosition += (Vector3.up * 2);
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = finalPosition;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
            Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }

        public static void Audio(int[] audioIDs, Vector3 position, float volume, bool adjust = true)
        {
            var clips = audioIDs.Select(id => Plugin.audioClips[id]).ToArray();
            var finalPosition = position;
            if (adjust)
                finalPosition += (Vector3.up * 2);
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = finalPosition;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.spatialBlend = 1f;
            audioSource.volume = volume;
            RoundManager.PlayRandomClip(audioSource, clips, randomize: true, oneShotVolume: volume);
            Object.Destroy(gameObject, clips[^1].length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }

        public static void Message(string title, string bottom, bool warning = false)
        {
            HUDManager.Instance.DisplayTip(title, bottom, warning);
        }

        public static IEnumerator Status(string text)
        {
            while (true)
            {
                HUDManager.Instance.DisplayStatusEffect(text);
                yield return new WaitForSeconds(1);
            }
        }

        public static void Spawn(SpawnableEnemyWithRarity enemy, Vector3 position)
        {
            GameObject gameObject = Object.Instantiate(enemy.enemyType.enemyPrefab, position, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);
            RoundManager.Instance.SpawnedEnemies.Add(gameObject.GetComponent<EnemyAI>());
        }

        public static void Spawn(string scrapName, Vector3 position)
        {
            var scrap = RoundManager.Instance.currentLevel.spawnableScrap.FirstOrDefault(i => i.spawnableItem.name.Equals(scrapName));
            GameObject gameObject = Object.Instantiate(scrap.spawnableItem.spawnPrefab, position, Quaternion.identity, RoundManager.Instance.spawnedScrapContainer);
            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = (int)(Random.Range(scrap.spawnableItem.minValue, scrap.spawnableItem.maxValue) * RoundManager.Instance.scrapValueMultiplier);
            NetworkObject network = gameObject.GetComponent<NetworkObject>();
            network.Spawn();
            component.FallToGround(true);
        }

        public static void SpawnQuicksand(int nb)
        {
            System.Random random = new System.Random(StartOfRound.Instance.randomMapSeed + 2);
            var outsideAINodes = (from x in GameObject.FindGameObjectsWithTag("OutsideAINode")
                                  orderby Vector3.Distance(x.transform.position, Vector3.zero)
                                  select x).ToArray();
            NavMeshHit val = default;

            for (int i = 0; i < nb; i++)
            {
                Vector3 position = outsideAINodes[random.Next(0, outsideAINodes.Length)].transform.position;
                Vector3 position2 = RoundManager.Instance.GetRandomNavMeshPositionInBoxPredictable(position, 30f, val, random) + Vector3.up;
                GameObject gameObject = Object.Instantiate(RoundManager.Instance.quicksandPrefab, position2, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
            }
        }
    }
}
