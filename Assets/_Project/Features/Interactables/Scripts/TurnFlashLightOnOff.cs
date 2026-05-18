using System;
using UnityEngine;

namespace KebanjiRun.Features.Interactables
{
    public class TurnFlashLightOnOff : MonoBehaviour
    {
        [SerializeField] private Light flashlightLight;

        public bool isOn {get; private set;}
        public event Action<bool> OnFlashlightStateChanged;

        private void Awake()
        {
            if (flashlightLight == null)
            {
                flashlightLight = GetComponentInChildren<Light>(true);
            }
        }

        public void ToggleLight()
        {
            isOn = !isOn;
            if (flashlightLight != null)
            {
                flashlightLight.enabled = isOn;
            }

            OnFlashlightStateChanged?.Invoke(isOn);
        }

        public void ForceOff()
        {
            isOn = false;
            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }

            OnFlashlightStateChanged?.Invoke(false);
        }
    }
}