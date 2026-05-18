using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using KebanjiRun.Features.Inventory.Data;
using KebanjiRun.Core.Managers;

namespace KebanjiRun.Features.NPCs
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable))]
    public class NPCInteractable : MonoBehaviour
    {
        [Header("NPC Request")]
        [Tooltip("Barang apa yang dibutuhkan NPC ini? (Pakaian/Obat)")]
        [SerializeField] private string requiredItemID;
        [SerializeField] private int bonusPoints = 200;

        [Header("References")]
        [SerializeField] private InventoryData inventoryData;
        [SerializeField] private AudioSource npcAudio;
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip failClip; // Jika player tidak bawa barangnya

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;
        private bool _alreadyHelped = false;

        private void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }

        private void OnEnable() => _interactable.activated.AddListener(TryGiveItem);
        private void OnDisable() => _interactable.activated.RemoveListener(TryGiveItem);

        private void TryGiveItem(ActivateEventArgs args)
        {
            if (_alreadyHelped) return;

            // Cek apakah pemain membawa barang yang diminta
            if (inventoryData.collectedItemIDs.Contains(requiredItemID))
            {
                // Sukses membantu
                ScoreManager.Instance.AddNPCBonus(bonusPoints);
                PlayAudio(successClip);
                _alreadyHelped = true;
                _interactable.enabled = false; // Matikan interaksi agar tidak dispam
                
                // Efek visual sederhana (misal NPC hilang / ganti warna)
                Debug.Log($"Berhasil memberikan {requiredItemID} ke NPC!");
            }
            else
            {
                // Gagal membantu (tidak bawa barang)
                PlayAudio(failClip);
                Debug.Log($"Gagal! Kamu tidak punya {requiredItemID}!");
            }
        }

        private void PlayAudio(AudioClip clip)
        {
            if (npcAudio != null && clip != null)
            {
                npcAudio.clip = clip;
                npcAudio.Play();
            }
        }
    }
}