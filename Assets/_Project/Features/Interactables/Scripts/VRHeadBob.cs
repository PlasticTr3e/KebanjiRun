using UnityEngine;

namespace KebanjiRun.Features.Locomotion
{
    public class VRHeadBob : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraOffset;

        [Header("Bobbing Settings")]
        [SerializeField] private float bobFrequency = 10f;
        [SerializeField] private float bobAmplitude = 0.015f;
        [SerializeField] private float speedThreshold = 0.1f;

        private float _timer;
        private Vector3 _defaultOffsetPosition;

        private void Start()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (cameraOffset != null)
            {
                _defaultOffsetPosition = cameraOffset.localPosition;
            }
            else
            {
                Debug.LogWarning("VRHeadBob requires the 'Camera Offset' Transform to move.");
            }
        }

        private void Update()
        {
            if (cameraOffset == null || characterController == null) return;

            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

            if (horizontalVelocity.magnitude > speedThreshold && characterController.isGrounded)
            {

                _timer += Time.deltaTime * bobFrequency * (horizontalVelocity.magnitude / 2f);
                
                float verticalOffset = Mathf.Sin(_timer) * bobAmplitude;
                cameraOffset.localPosition = new Vector3(_defaultOffsetPosition.x, _defaultOffsetPosition.y + verticalOffset, _defaultOffsetPosition.z);
            }
            else
            {
                _timer = 0;
                cameraOffset.localPosition = Vector3.Lerp(cameraOffset.localPosition, _defaultOffsetPosition, Time.deltaTime * 5f);
            }
        }
    }
}