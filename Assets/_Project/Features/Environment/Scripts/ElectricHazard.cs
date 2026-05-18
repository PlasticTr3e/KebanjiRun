using UnityEngine;
using KebanjiRun.Core.Managers; 
using Unity.XR.CoreUtils;

namespace KebanjiRun.Features.Environment
{
    public class ElectricHazard : MonoBehaviour
    {
        [Header("Effects (Optional)")]
        [SerializeField] private AudioSource zapSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Transform playerRoot = other.transform.root;
                RespawnPlayer(playerRoot);
            }
        }

        private void RespawnPlayer(Transform playerRoot)
        {
            if (zapSound != null) zapSound.Play();
            
            XROrigin xrOrigin = FindAnyObjectByType<XROrigin>();
            PlayerSpawnPoint spawnPoint = FindAnyObjectByType<PlayerSpawnPoint>();

            if (xrOrigin != null && spawnPoint != null)
            {
                CharacterController charController = playerRoot.GetComponentInChildren<CharacterController>();
                if (charController == null) charController = xrOrigin.GetComponentInChildren<CharacterController>();
                if (charController != null) charController.enabled = false;

                xrOrigin.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
                if (charController != null) charController.enabled = true;
                
                Debug.Log("Pemain tersengat listrik dan dikembalikan ke titik awal!");
            }
            else
            {
                Debug.LogWarning("Spawn Point tidak ditemukan di scene ini!");
            }
        }
    }
}