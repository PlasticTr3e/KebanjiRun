using UnityEngine;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.Environment
{
    public class MissionCompleteTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.ChangeState(GameState.MissionComplete);
            }
        }
    }
}