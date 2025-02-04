using ACEPlay.Bridge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hunter
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public PlayerTouchMovement playerTouchMovement;
        public HandTutorial handTutorial;
        public TakeMoney takeMoney;
        public ClusterDollar[] clusterDollars;
        int indexClusterDollar;
        public Dictionary<Key, GameObject> keys = new Dictionary<Key, GameObject>();

        public void Awake()
        {
            instance = this;
        }

        public void ResetFxDollars()
        {
            for (int i = 0; i < clusterDollars.Length; i++)
            {
                clusterDollars[i].ResetFx();
            }
        }

        public void PlayDollars(GameObject target, Vector3 startPos, int coin)
        {
            clusterDollars[indexClusterDollar].transform.position = startPos;
            clusterDollars[indexClusterDollar].gameObject.SetActive(true);
            clusterDollars[indexClusterDollar].PlayDollars(target, coin);
            indexClusterDollar++;
            if (indexClusterDollar == clusterDollars.Length) indexClusterDollar = 0;
        }

        public float GetSpeed(Player player)
        {
            float result = playerTouchMovement.GetMovemntAmount().magnitude;
            return result != 0 ? result * player.navMeshAgent.speed / 3 : player.navMeshAgent.velocity.magnitude;
        }

        public void ResetGame()
        {
            if (transform.position != Vector3.zero)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(Vector3.zero, out hit, 100, NavMesh.AllAreas))
                {
                    playerTouchMovement.navMeshAgent.Warp(hit.position);
                }
                else BridgeController.instance.Debug_LogWarning("!");
            }
            playerTouchMovement.navMeshAgent.enabled = true;
            playerTouchMovement.navMeshAgent.speed = 7;
            playerTouchMovement.navMeshAgent.isStopped = false;
        }

        public bool IsKey(Key key)
        {
            if(keys.ContainsKey(key))
            {
                keys.Remove(key);
                return true;
            }
            return false;
        }

        public void IsHasKey(GameObject value)
        {
            foreach (var item in keys)
            {
                if(item.Value == value)
                {
                    item.Key.ResetKey();
                    keys.Remove(item.Key);
                    return;
                }
            }
        }

        public void SetKey(Key key, GameObject player)
        {
            keys.Add(key, player);
        }

        public void Win()
        {
            playerTouchMovement.HandleLoseFinger();
        }

        public void Lose()
        {
            playerTouchMovement.HandleLoseFinger();
        }
    }
}
