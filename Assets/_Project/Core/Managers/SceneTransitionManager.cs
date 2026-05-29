using KebanjiRun.Core.Managers;
using KebanjiRun.Features.Environment;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KebanjiRun.Core.Managers
{
    public class SceneTransitionManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private float fadeDuration = 1.0f;

        [Header("Scene Names")]
        [SerializeField] private string preEventSceneName = "PreEvent_Scene";
        [SerializeField] private string eventSceneName = "Event_Scene";
        [SerializeField] private string postEventSceneName = "PostEvent_Scene";

        [Header("Player Reference")]
        [SerializeField] private Transform xrRig;

        [Header("Direction Arrow")]
        [SerializeField] private bool showDirectionArrowInEvent = true;
        [SerializeField] private string eventNextSceneTargetName = "PostEventTeleporter";

        [Header("Event Hints")]
        [SerializeField] private bool showEventHints = true;
        [SerializeField] private float fallenTreeHintDistance = 10f;
        [SerializeField] private float electricHazardHintDistance = 12f;
        [SerializeField] private float hintDuration = 4f;
        [SerializeField] private string fallenTreeHintText = "Ada pohon tumbang di depan. Lompat untuk melewatinya.";
        [SerializeField] private string electricHazardHintText = "Tiang listrik jatuh membuat air bertegangan. Hindari area air tersebut.";

        private string _activeGameplayScene = "";
        private DirectionArrowIndicator _directionArrow;
        private Transform _fallenTreeTarget;
        private Transform _electricHazardTarget;
        private bool _eventHintsActive;
        private bool _fallenTreeHintShown;
        private bool _electricHintShown;
        private CanvasGroup _hintCanvasGroup;
        private TextMeshProUGUI _hintText;
        private Coroutine _hintRoutine;
        private float _hintWorldDistance = 1.8f;
        private float _hintWorldHeightOffset = 0.25f;

        private void Start()
        {
            fadeCanvas.alpha = 1;
            fadeCanvas.blocksRaycasts = true;

            _activeGameplayScene = "";

            GameManager.Instance.OnGameStateChanged += HandleStateChanged;

            if (GameManager.Instance.CurrentState == GameState.PreEvent)
            {
                HandleStateChanged(GameState.PreEvent);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleStateChanged;

            DestroyDirectionArrow();
            DestroyHintUI();
        }

        private void HandleStateChanged(GameState newState)
        {
            string targetScene = GetSceneNameForState(newState);

            if (string.IsNullOrEmpty(targetScene) || targetScene == _activeGameplayScene) return;

            StartCoroutine(TransitionSceneRoutine(_activeGameplayScene, targetScene));
        }

        private string GetSceneNameForState(GameState state)
        {
            return state switch
            {
                GameState.PreEvent => preEventSceneName,
                GameState.Event => eventSceneName,
                GameState.PostEvent => postEventSceneName,
                _ => string.Empty
            };
        }

        private IEnumerator TransitionSceneRoutine(string sceneToUnload, string sceneToLoad)
        {
            yield return Fade(1f);

            if (!string.IsNullOrEmpty(sceneToUnload) && SceneManager.GetSceneByName(sceneToUnload).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }

            yield return SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

            _activeGameplayScene = sceneToLoad;

            PlayerSpawnPoint spawnPoint = FindAnyObjectByType<PlayerSpawnPoint>();
            if (spawnPoint != null && xrRig != null)
            {
                xrRig.position = spawnPoint.transform.position;      
                xrRig.rotation = spawnPoint.transform.rotation;
            }

            UpdateDirectionArrow(sceneToLoad);
            UpdateEventHints(sceneToLoad);

            yield return Fade(0f);
        }

        private void Update()
        {
            if (!_eventHintsActive || xrRig == null) return;

            if (!_fallenTreeHintShown)
            {
                if (_fallenTreeTarget == null)
                {
                    _fallenTreeHintShown = true;
                    ShowHint(fallenTreeHintText);
                }
                else if (IsWithinHorizontalDistance(xrRig.position, _fallenTreeTarget.position, fallenTreeHintDistance))
                {
                    _fallenTreeHintShown = true;
                    ShowHint(fallenTreeHintText);
                }
            }

            if (!_electricHintShown && _electricHazardTarget != null &&
                IsWithinHorizontalDistance(xrRig.position, _electricHazardTarget.position, electricHazardHintDistance))
            {
                _electricHintShown = true;
                ShowHint(electricHazardHintText);
            }
        }

        private void UpdateDirectionArrow(string activeSceneName)
        {
            if (!showDirectionArrowInEvent || xrRig == null)
            {
                DestroyDirectionArrow();
                return;
            }

            if (activeSceneName != eventSceneName)
            {
                DestroyDirectionArrow();
                return;
            }

            Transform nextSceneTarget = FindTransformByNameInScene(activeSceneName, eventNextSceneTargetName);
            if (nextSceneTarget == null)
            {
                Debug.LogWarning($"Direction arrow target '{eventNextSceneTargetName}' was not found in scene '{activeSceneName}'.");
                DestroyDirectionArrow();
                return;
            }

            if (_directionArrow == null)
            {
                GameObject arrowObject = new GameObject("DirectionArrowIndicator");
                _directionArrow = arrowObject.AddComponent<DirectionArrowIndicator>();
            }

            _directionArrow.Configure(xrRig, nextSceneTarget);
            _directionArrow.gameObject.SetActive(true);
        }

        private void DestroyDirectionArrow()
        {
            if (_directionArrow != null)
            {
                Destroy(_directionArrow.gameObject);
                _directionArrow = null;
            }
        }

        private void UpdateEventHints(string activeSceneName)
        {
            if (!showEventHints || xrRig == null || activeSceneName != eventSceneName)
            {
                DisableEventHints();
                return;
            }

            _fallenTreeTarget = FindFallenTreeTarget(activeSceneName);
            _electricHazardTarget = FindElectricHazardTarget(activeSceneName);
            _fallenTreeHintShown = false;
            _electricHintShown = false;
            _eventHintsActive = true;
            EnsureHintUI();
        }

        private void DisableEventHints()
        {
            _eventHintsActive = false;
            _fallenTreeTarget = null;
            _electricHazardTarget = null;

            if (_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
                _hintRoutine = null;
            }

            if (_hintCanvasGroup != null)
            {
                _hintCanvasGroup.alpha = 0f;
                _hintCanvasGroup.gameObject.SetActive(false);
            }
        }

        private Transform FindElectricHazardTarget(string sceneName)
        {
            ElectricHazard[] hazards = FindObjectsByType<ElectricHazard>(FindObjectsSortMode.None);
            for (int i = 0; i < hazards.Length; i++)
            {
                if (hazards[i] != null && hazards[i].gameObject.scene.name == sceneName)
                    return hazards[i].transform;
            }

            return FindTransformByNameInScene(sceneName, "Electric");
        }

        private Transform FindFallenTreeTarget(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded) return null;

            GameObject[] roots = scene.GetRootGameObjects();
            Transform best = null;
            float bestScore = float.MinValue;

            for (int i = 0; i < roots.Length; i++)
            {
                EvaluateTreeRecursive(roots[i].transform, ref best, ref bestScore);
            }

            if (best != null) return best;

            return FindTransformByNameContains(sceneName, "tree");
        }

        private static void EvaluateTreeRecursive(Transform current, ref Transform best, ref float bestScore)
        {
            if (current == null) return;

            string lowerName = current.name.ToLowerInvariant();
            if (lowerName.Contains("tree"))
            {
                float tiltAngle = Vector3.Angle(current.up, Vector3.up);
                if (tiltAngle > 25f)
                {
                    float score = tiltAngle + (current.lossyScale.magnitude * 0.05f);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = current;
                    }
                }
            }

            for (int i = 0; i < current.childCount; i++)
            {
                EvaluateTreeRecursive(current.GetChild(i), ref best, ref bestScore);
            }
        }

        private static Transform FindTransformByNameContains(string sceneName, string keyword)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded) return null;

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                Transform found = FindInChildrenByNameContains(roots[i].transform, keyword);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static Transform FindInChildrenByNameContains(Transform parent, string keyword)
        {
            if (parent.name.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                return parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindInChildrenByNameContains(parent.GetChild(i), keyword);
                if (found != null)
                    return found;
            }

            return null;
        }

        private bool IsWithinHorizontalDistance(Vector3 a, Vector3 b, float distance)
        {
            Vector2 aXZ = new Vector2(a.x, a.z);
            Vector2 bXZ = new Vector2(b.x, b.z);
            return Vector2.Distance(aXZ, bXZ) <= distance;
        }

        private void EnsureHintUI()
        {
            if (_hintCanvasGroup != null) return;

            GameObject root = new GameObject("EventHintUI");
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 1200;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(720f, 150f);
            root.transform.localScale = Vector3.one * 0.0012f;

            _hintCanvasGroup = root.AddComponent<CanvasGroup>();
            _hintCanvasGroup.alpha = 0f;

            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(root.transform, false);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.62f);
            Sprite roundedSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
            if (roundedSprite != null)
            {
                panelImage.sprite = roundedSprite;
                panelImage.type = Image.Type.Sliced;
            }

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 0f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            GameObject textObject = new GameObject("HintText");
            textObject.transform.SetParent(panel.transform, false);
            _hintText = textObject.AddComponent<TextMeshProUGUI>();
            _hintText.alignment = TextAlignmentOptions.Center;
            _hintText.color = Color.white;
            _hintText.fontSize = 52f;
            _hintText.textWrappingMode = TextWrappingModes.Normal;
            _hintText.text = string.Empty;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = new Vector2(30f, 20f);
            textRect.offsetMax = new Vector2(-30f, -20f);

            root.SetActive(false);
        }

        private void ShowHint(string message)
        {
            EnsureHintUI();
            if (_hintCanvasGroup == null || _hintText == null || Camera.main == null) return;

            if (_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
                _hintRoutine = null;
            }

            _hintRoutine = StartCoroutine(HintRoutine(message));
        }

        private IEnumerator HintRoutine(string message)
        {
            _hintText.text = message;
            _hintCanvasGroup.gameObject.SetActive(true);
            _hintCanvasGroup.alpha = 1f;

            float time = 0f;
            while (time < hintDuration)
            {
                UpdateHintCanvasPose();
                time += Time.deltaTime;
                yield return null;
            }

            float fadeTime = 0.25f;
            float elapsed = 0f;
            float startAlpha = _hintCanvasGroup.alpha;
            while (elapsed < fadeTime)
            {
                UpdateHintCanvasPose();
                elapsed += Time.deltaTime;
                _hintCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeTime);
                yield return null;
            }

            _hintCanvasGroup.alpha = 0f;
            _hintCanvasGroup.gameObject.SetActive(false);
            _hintRoutine = null;
        }

        private void UpdateHintCanvasPose()
        {
            if (_hintCanvasGroup == null || Camera.main == null) return;

            Transform cam = Camera.main.transform;
            Transform uiTransform = _hintCanvasGroup.transform;
            uiTransform.position = cam.position + cam.forward * _hintWorldDistance + cam.up * _hintWorldHeightOffset;
            uiTransform.rotation = Quaternion.LookRotation(cam.position - uiTransform.position, Vector3.up);
        }

        private void DestroyHintUI()
        {
            if (_hintCanvasGroup != null)
            {
                Destroy(_hintCanvasGroup.gameObject);
                _hintCanvasGroup = null;
            }

            _hintText = null;
            _hintRoutine = null;
        }

        private static Transform FindTransformByNameInScene(string sceneName, string objectName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded) return null;

            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                Transform found = FindInChildrenByName(roots[i].transform, objectName);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static Transform FindInChildrenByName(Transform parent, string objectName)
        {
            if (parent.name == objectName) return parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindInChildrenByName(parent.GetChild(i), objectName);
                if (found != null)
                    return found;
            }

            return null;
        }

        private IEnumerator Fade(float targetAlpha)
        {
            fadeCanvas.blocksRaycasts = true;
            float startAlpha = fadeCanvas.alpha;
            float time = 0;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                yield return null;
            }

            fadeCanvas.alpha = targetAlpha;
            fadeCanvas.blocksRaycasts = (targetAlpha > 0);
        }
    }

    public class DirectionArrowIndicator : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private Transform playerRig;
        [SerializeField] private Transform target;

        [Header("Placement")]
        [SerializeField] private float distanceFromPlayer = 2.0f;
        [SerializeField] private float heightOffset = 1.8f;
        [SerializeField] private float bobAmplitude = 0.08f;
        [SerializeField] private float bobSpeed = 2.2f;

        [Header("Style")]
        [SerializeField] private Color arrowColor = new Color(1f, 0.88f, 0.2f, 0.95f);

        public void Configure(Transform rig, Transform targetTransform)
        {
            playerRig = rig;
            target = targetTransform;
        }

        private void Awake()
        {
            BuildArrowVisual();
        }

        private void LateUpdate()
        {
            if (playerRig == null || target == null) return;

            Vector3 toTarget = target.position - playerRig.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude < 0.0001f) return;

            Vector3 direction = toTarget.normalized;
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;

            transform.position = playerRig.position + Vector3.up * (heightOffset + bobOffset) + direction * distanceFromPlayer;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private void BuildArrowVisual()
        {
            if (transform.childCount > 0) return;

            Shader selectedShader = Shader.Find("Universal Render Pipeline/Unlit");
            if (selectedShader == null)
            {
                selectedShader = Shader.Find("Unlit/Color");
            }

            Material arrowMaterial = selectedShader != null
                ? new Material(selectedShader)
                : new Material(Shader.Find("Standard"));

            arrowMaterial.color = arrowColor;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "ArrowBody";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0f, 0f);
            body.transform.localScale = new Vector3(0.14f, 0.14f, 0.72f);
            RemoveCollider(body);

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "ArrowHead";
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0f, 0f, 0.46f);
            head.transform.localScale = new Vector3(0.26f, 0.26f, 0.26f);
            RemoveCollider(head);

            ApplyMaterial(body, arrowMaterial);
            ApplyMaterial(head, arrowMaterial);
        }

        private static void RemoveCollider(GameObject go)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
        }

        private static void ApplyMaterial(GameObject go, Material mat)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;

            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material = mat;
        }
    }
}
