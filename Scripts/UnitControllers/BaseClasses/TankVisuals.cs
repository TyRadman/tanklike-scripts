using TankLike.UnitControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Utils;

    public class TankVisuals : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        [Header("Hit Flash")]
        [SerializeField] private Material _hitFlashMaterial;
        [SerializeField] private float _hitFlashTime;

        [SerializeField] private List<Renderer> _tankMeshes = new List<Renderer>();
        
        [Header("Flashing")]
        [SerializeField] private Color _flashingColor;
        [SerializeField] private float _flashingSpeed = 1.0f;

        private TankComponents _components;
        private Material _originalMaterial; // maybe save the material of each mesh by using a dictionary instead of a list
        private Color _originalColor;
        private Coroutine _hitFlashCoroutine;
        private Coroutine _switchColorCoroutine;
        private Coroutine _flashingCoroutine;

        private const string BASE_COLOR_KEY = "_BaseColor";
        private const string TEXTURE_IMPACT_KEY = "_TextureImpact";
        private const string BASE_MAP_KEY = "_BaseMap";
        private const float TRANSITION_DURATION = 0.5f;
        
        public void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            TankBodyParts parts = components.TankBodyParts;

            parts.Parts.ForEach(p => AddMeshes(p.Meshes));

            _originalMaterial = _tankMeshes[0].material;
            _originalColor = _tankMeshes[0].material.color;

        }

        public void SetTextureForMainMaterial(Texture2D texture)
        {
            _originalMaterial.SetTexture(BASE_MAP_KEY, texture);
            SwitchMaterial(_originalMaterial);
        }

        public void SwitchMaterial(Material material)
        {
            _tankMeshes.ForEach(m => m.material = material);
        }

        public void RestoreOriginalMaterial()
        {
            _tankMeshes.ForEach(m => m.material = _originalMaterial);
        }

        public void RestoreOriginalColor()
        {
            _tankMeshes.ForEach(m => m.material.color = _originalColor);
        }

        public void AddMaterial(Material material)
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                Material[] newMats = new Material[sharedMaterials.Length + 1];
                newMats[0] = mesh.sharedMaterials[0];
                newMats[1] = material;
                mesh.sharedMaterials = newMats;
            }
        }

        public void RemoveMaterial()
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                Material[] sharedMaterials = mesh.sharedMaterials;
                Material[] newMats = new Material[sharedMaterials.Length - 1];
                newMats[0] = mesh.sharedMaterials[0];
                mesh.sharedMaterials = newMats;
            }
        }

        public void AddColorToMeshes(Color color)
        {
            //_tankMeshes.ForEach(m => m.material.color += color);
            if (_switchColorCoroutine != null)
            {
                StopCoroutine(_switchColorCoroutine);
            }

            _switchColorCoroutine = StartCoroutine(SwitchToColor(Color.white, color));
        }

        public void SubtractColorFromMeshes(Color color)
        {
            //_tankMeshes.ForEach(m => m.material.color -= color);
            if (_switchColorCoroutine != null)
            {
                StopCoroutine(_switchColorCoroutine);
            }

            _switchColorCoroutine = StartCoroutine(SwitchToColor(color, Color.white));
        }

        private IEnumerator SwitchToColor(Color startColor, Color endColor)
        {
            float time = 0f;

            while (time < TRANSITION_DURATION)
            {
                time += Time.deltaTime;
                float t = time / TRANSITION_DURATION;
                _tankMeshes.ForEach(m => m.material.color = Color.Lerp(startColor, endColor, t));
                yield return null;
            }
        }

        public void AddMeshes(List<Renderer> meshes)
        {
            _tankMeshes.AddRange(meshes);
        }

        public Material GetOriginalMaterial()
        {
            return _originalMaterial;
        }

        public void HideVisuals()
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.enabled = false;
            }
        }

        public void ShowVisuals()
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.enabled = true;
            }
        }

        public void OnHitHandler()
        {
            if (_hitFlashMaterial != null)
            {
                this.StopCoroutineSafe(_hitFlashCoroutine);
                _hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
            }
        }

        private IEnumerator HitFlashRoutine()
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.material.SetFloat(TEXTURE_IMPACT_KEY, 0);
            }

            yield return new WaitForSeconds(_hitFlashTime);

            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.material.SetFloat(TEXTURE_IMPACT_KEY, 1);
            }
        }

        public void StartFlashing()
        {
            if (_flashingCoroutine != null)
            {
                StopCoroutine(_flashingCoroutine);
            }

            _flashingCoroutine = StartCoroutine(FlashingRoutine());
        }

        private IEnumerator FlashingRoutine()
        {
            float t = 0f;

            while (true)
            {
                // Lerp between the two colors over time
                t = Mathf.PingPong(Time.time * _flashingSpeed, 1f);
                foreach (Renderer mesh in _tankMeshes)
                {
                    mesh.material.SetColor(BASE_COLOR_KEY, Color.Lerp(_originalColor, _flashingColor, t));
                }

                yield return null;
            }
        }

        public void StopFlashing()
        {

            if(_flashingCoroutine != null)
            {
                StopCoroutine(_flashingCoroutine);
            }

            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.material.SetColor(BASE_COLOR_KEY, _originalColor);
            }
        }

        public void ResetMaterialTextureImpact()
        {
            foreach (Renderer mesh in _tankMeshes)
            {
                mesh.material.SetFloat(TEXTURE_IMPACT_KEY, 1);
            }
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            _tankMeshes.ForEach(m => m.material = _originalMaterial);
            _tankMeshes.ForEach(m => m.material.color = _originalColor);
        }

        public void Dispose()
        {
            StopAllCoroutines();
            StopFlashing();
            ResetMaterialTextureImpact();
        }
        #endregion
    }
}
