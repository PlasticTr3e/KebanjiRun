using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using KebanjiRun.Features.Inventory.Data;
using KebanjiRun.Features.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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

            if (!isValidItem)
            {
                Debug.Log($"Item {gameObject.name} tidak penting dibawa!");
                return;
            }

            if (keepInHandItem)
            {
                IXRSelectInteractor selectingInteractor = _interactable.firstInteractorSelecting;
                if (selectingInteractor != null)
                {
                    EquipToHand(selectingInteractor);
                    ChecklistUIManager.Instance.MarkItemCollected(itemID);
                }
                return;
            }

            if (itemID != "Backpack" && !inventoryData.hasBackpack)
            {
                Debug.Log("Harus ambil Tas Ransel dulu sebelum mengambil barang lain!");
                return;
            }

            bool isStored = inventoryData.TryStoreItem(itemID);
            if (isStored)
            {
                Debug.Log($"Berhasil memasukkan {itemID} ke dalam tas.");
                if (ChecklistUIManager.Instance != null)
                {
                    ChecklistUIManager.Instance.MarkItemCollected(itemID);
                }
                gameObject.SetActive(false);
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

            // Collider[] colliders = GetComponentsInChildren<Collider>();
            // foreach (var col in colliders) col.enabled = false;

            transform.SetParent(handTransform);
            transform.localPosition = equipPositionOffset;
            transform.localEulerAngles = equipRotationOffser;

            if (_glowEffect != null) _glowEffect.StopGlow();

            if (_rightControllerVisual != null)
            {
                _rightControllerVisual.SetActive(false);
            }
        }
    }
}