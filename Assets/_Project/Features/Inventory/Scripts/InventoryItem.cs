using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using KebanjiRun.Features.Inventory.Data;
using KebanjiRun.Features.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using KebanjiRun.Features.Interactables; // TAMBAHKAN INI

namespace KebanjiRun.Features.Inventory.Components
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class InventoryItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        public string itemID;
        public bool isValidItem = true;

        [Header("Keep in hand item configuration")]
        public bool keepInHandItem = false;
        public Vector3 equipPositionOffset = Vector3.zero;
        public Vector3 equipRotationOffser = Vector3.zero;

        [Header("References")]
        [SerializeField] private InventoryData inventoryData;
        private GameObject _rightControllerVisual;
        private XRGrabInteractable _interactable;
        private GlowEffect _glowEffect;

        private void Awake()
        {
            _interactable = GetComponent<XRGrabInteractable>();
            _glowEffect = GetComponent<GlowEffect>();

            GameObject rightController = GameObject.Find("Right Controller");
            if (rightController != null)
            {
                Transform rightControllerChild = rightController.transform.Find("Right Controller Visual");
                if (rightControllerChild != null)
                {
                    _rightControllerVisual = rightControllerChild.Find("UniversalController")?.gameObject;
                }
            }
        }

        private void OnEnable()
        {
            _interactable.selectEntered.AddListener(OnGrab);
            _interactable.selectExited.AddListener(OnRelease);
            _interactable.activated.AddListener(OnStoreButtonPressed);
        }

        private void OnDisable()
        {
            _interactable.selectEntered.RemoveListener(OnGrab);
            _interactable.selectExited.RemoveListener(OnRelease);
            _interactable.activated.RemoveListener(OnStoreButtonPressed);
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (_glowEffect != null)
            {
                _glowEffect.ShowGlow(isValidItem ? GlowEffect.GlowColor.Green : GlowEffect.GlowColor.Red);
            }
        }

        private void OnRelease(SelectExitEventArgs args)
        {
            if (_glowEffect != null)
            {
                _glowEffect.StopGlow();
            }
        }

        private void OnStoreButtonPressed(ActivateEventArgs args)
        {
            if (inventoryData == null) return;

            // KONDISI A: Salah mengambil barang dekorasi
            if (!isValidItem)
            {
                string pesanSalah = "Jangan bawa barang ini. Ambil barang penting sesuai daftar!";
                if (WarningUIManager.Instance != null)
                    WarningUIManager.Instance.ShowWarning(pesanSalah, true);
                return;
            }

            // KONDISI B: Belum punya ransel
            if (itemID != "Backpack" && !inventoryData.hasBackpack)
            {
                string pesanButuhTas = "Ambil Ransel/Tas Siaga terlebih dahulu sebelum mengumpulkan barang!";
                if (WarningUIManager.Instance != null)
                    WarningUIManager.Instance.ShowWarning(pesanButuhTas, true);
                return;
            }
            
            bool dataTerproses = false;

            if (keepInHandItem)
            {
                IXRSelectInteractor selectingInteractor = _interactable.firstInteractorSelecting;
                if (selectingInteractor != null)
                {
                    EquipToHand(selectingInteractor);
                    
                    if (!inventoryData.collectedItemIDs.Contains(itemID))
                    {
                        inventoryData.collectedItemIDs.Add(itemID);
                    }
                    dataTerproses = true;
                }
            }
            else
            {
                dataTerproses = inventoryData.TryStoreItem(itemID);
            }

            if (dataTerproses)
            {
                if (ChecklistUIManager.Instance != null)
                {
                    ChecklistUIManager.Instance.MarkItemCollected(itemID);
                }

                // Cek apakah ini barang terakhir
                if (inventoryData.IsAllRequiredItemsCollected())
                {
                    string pesanSiapEvakuasi = "Semua barang darurat telah terkumpul! Segera keluar dari rumah sekarang!";
                    if (WarningUIManager.Instance != null)
                        WarningUIManager.Instance.ShowWarning(pesanSiapEvakuasi, false);
                }
                else
                {
                    if (keepInHandItem)
                    {
                        if (itemID.ToLower().Contains("flashlight") || itemID.ToLower().Contains("senter"))
                        {
                            // REVISI PESAN: Beritahu cara mematikan menggunakan tombol N / Controller
                            if (NotificationUIManager.Instance != null)
                                NotificationUIManager.Instance.ShowNotification("Senter OTOMATIS NYALA! Tekan 'Tombol N' (di Keyboard Simulator) atau 'Tombol A/X' (di Controller VR) jika ingin mematikan.");
                        }
                        else
                        {
                            if (NotificationUIManager.Instance != null)
                                NotificationUIManager.Instance.ShowNotification($"Berhasil menggunakan {itemID}.");
                        }
                    }
                    else
                    {
                        string namaBarangTeks = itemID == "Backpack" ? "Ransel Siaga" : itemID;
                        if (NotificationUIManager.Instance != null)
                            NotificationUIManager.Instance.ShowNotification($"{namaBarangTeks} berhasil dimasukkan kedalam tas siaga!");
                    }
                }

                if (!keepInHandItem)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        
        private void EquipToHand(IXRSelectInteractor interactor)
        {
            Transform handTransform = interactor.transform;
            _interactable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)_interactable);
            _interactable.enabled = false;

            if (TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            transform.SetParent(handTransform);
            transform.localPosition = equipPositionOffset;
            transform.localEulerAngles = equipRotationOffser;

            if (_glowEffect != null) _glowEffect.StopGlow();

            if (_rightControllerVisual != null)
            {
                _rightControllerVisual.SetActive(false);
            }

            if (TryGetComponent<TurnFlashLightOnOff>(out TurnFlashLightOnOff flashLightScript))
            {
                flashLightScript.ForceOn(); 
            }
        }
    }
}