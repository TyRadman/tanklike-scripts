using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.UnitControllers;

namespace TankLike.Elements
{
    public abstract class ElementEffect : ScriptableObject
    {
        [System.Serializable]
        public class StackRules
        {
            public Element Element;
            public ElementStack Stack;
        }

        public Element ElementTag;
        [SerializeField] private int _duration;
        [SerializeField] private PoolObjectType _effectParticles;
        [SerializeField] private bool _hasColorEffect = true;
        [SerializeField] private Color _targetColorOverlay;
        [SerializeField] private bool _hasMaterialEffect = true;
        [SerializeField] private Material _targetMaterial;
        [Tooltip("Additive: If the same effect is applied to the target twice in a row or more, the duration accumilates. \n Resetting: If the same effect is applied to the target twice in a row, the duration of the effect is reset to its maximum value")]
        [SerializeField] private BlendType _EffectBlendType;
        [Tooltip("True: the effect will be applied to the target every second throughout the whole effect's duration.\nFalse: the effect will take place on start and will go away at the end.")]
        [field: SerializeField] public bool ContinuousEffect { get; private set; }
        [field: SerializeField] public bool AdditiveMaterial { get; private set; }
        [SerializeField] private List<StackRules> Stacks;

        public virtual void TakeEffect(TankComponents tank)
        {

        }

        public virtual void StopEffect(TankComponents tank)
        {

        }

        public int GetDuration()
        {
            return _duration;
        }

        public PoolObjectType GetParticleTag()
        {
            return _effectParticles;
        }

        public bool GetColorOverlay(ref Color color)
        {
            color = _targetColorOverlay;
            return _hasColorEffect;
        }

        public bool GetEffectMaterial(ref Material material)
        {
            material = _targetMaterial;
            return _hasMaterialEffect;
        }

        public BlendType GetBlendType()
        {
            return _EffectBlendType;
        }

        public List<StackRules> GetStackRules()
        {
            return Stacks;
        }
    }

    /// <summary>
    /// How the element stacks with itself
    /// </summary>
    public enum BlendType
    {
        Additive, Resetting
    }
}
