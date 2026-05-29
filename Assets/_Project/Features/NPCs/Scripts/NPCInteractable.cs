using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using TMPro;
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
        [SerializeField] private AudioClip failClip; 
        
        [Header("Speech Bubble")]
        [SerializeField] private string speechText;
        [SerializeField] private Vector3 speechBubbleLocalOffset = new Vector3(0f, 3.1f, 0f);
        [SerializeField] private Vector2 speechBubbleSize = new Vector2(420f, 120f);

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable _interactable;
        private Transform _speechBubbleRoot;
        private bool _alreadyHelped = false;

        private void Awake()
        {
            _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            CreateSpeechBubbleIfNeeded();
        }

        private void OnEnable() => _interactable.activated.AddListener(TryGiveItem);
        private void OnDisable() => _interactable.activated.RemoveListener(TryGiveItem);

        private void LateUpdate()
        {
            if (_speechBubbleRoot == null || Camera.main == null) return;

            var camTransform = Camera.main.transform;
            _speechBubbleRoot.LookAt(_speechBubbleRoot.position + camTransform.rotation * Vector3.forward,
                                     camTransform.rotation * Vector3.up);
        }

        private void TryGiveItem(ActivateEventArgs args)
        {
            if (_alreadyHelped) return;

           
            if (inventoryData.collectedItemIDs.Contains(requiredItemID))
            {
              
                ScoreManager.Instance.AddNPCBonus(bonusPoints);
                PlayAudio(successClip);
                _alreadyHelped = true;
                _interactable.enabled = false; 
                SetSpeechBubbleVisible(false);
                
                
                Debug.Log($"Berhasil memberikan {requiredItemID} ke NPC!");
            }
            else
            {
                
                PlayAudio(failClip);
                Debug.Log($"Gagal! Kamu tidak punya {requiredItemID}!");
            }
        }

        private void CreateSpeechBubbleIfNeeded()
        {
            var resolvedSpeech = ResolveSpeechText();
            if (string.IsNullOrWhiteSpace(resolvedSpeech)) return;

            var bubbleRoot = new GameObject("SpeechBubble");
            _speechBubbleRoot = bubbleRoot.transform;
            _speechBubbleRoot.SetParent(transform, false);
            _speechBubbleRoot.localPosition = speechBubbleLocalOffset;
            _speechBubbleRoot.localRotation = Quaternion.identity;
            _speechBubbleRoot.localScale = Vector3.one * 0.003f;

            var canvasGO = new GameObject("Canvas");
            canvasGO.transform.SetParent(_speechBubbleRoot, false);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 1000;
            var canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = speechBubbleSize;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(canvasGO.transform, false);
            var backgroundImage = backgroundGO.AddComponent<Image>();
            var roundedSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            if (roundedSprite != null)
            {
                backgroundImage.sprite = roundedSprite;
                backgroundImage.type = Image.Type.Sliced;
            }
            backgroundImage.color = new Color(1f, 1f, 1f, 0.45f);
            var backgroundRect = backgroundGO.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 0f);
            backgroundRect.anchorMax = new Vector2(1f, 1f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(backgroundGO.transform, false);
            var textComponent = textGO.AddComponent<TextMeshProUGUI>();
            textComponent.text = resolvedSpeech;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.black;
            textComponent.fontSize = 34f;
            textComponent.textWrappingMode = TextWrappingModes.Normal;
            textComponent.overflowMode = TextOverflowModes.Overflow;

            var textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = new Vector2(20f, 14f);
            textRect.offsetMax = new Vector2(-20f, -14f);
        }

        private string ResolveSpeechText()
        {
            if (!string.IsNullOrWhiteSpace(speechText))
                return speechText;

            var lowerName = gameObject.name.ToLowerInvariant();
            if (lowerName == "npc1" || lowerName == "npc_1")
                return "duh, pusing sekali";
            if (lowerName == "npc2" || lowerName == "npc_2")
                return "dingin sekali, bajuku basah kuyup";

            return string.Empty;
        }

        private void SetSpeechBubbleVisible(bool isVisible)
        {
            if (_speechBubbleRoot != null)
                _speechBubbleRoot.gameObject.SetActive(isVisible);
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
