using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Elements;
using TankLike.Utils;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// Responsible for managing the elements' effects and damages to the tank it's attached to.
    /// </summary>
    public abstract class TankElementsEffects : MonoBehaviour, IController, IElementTarget
    {
        [System.Serializable]
        public class Effect
        {
            public int Duration;
            public int Counter = 0;
            public Coroutine PrcoessCoroutine;
            public bool Active = false;
            public ElementEffect ElementEffect;
        }

        private WaitForSeconds _waitingTime;
        private TankComponents _components;
        private List<Effect> _effects = new List<Effect>();
        [SerializeField] protected bool _canGetEffects = true;

        private ParticleSystem _effectParticle;
        private Color _tankColor;
        private Material _tankMaterial;

        public bool IsActive { get; set; }

        public void SetUp(IController controller)
        {
            if (controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
            _waitingTime = new WaitForSeconds(1f);
        }

        public virtual void TakeElementEffect(ElementEffect element)
        {
            if (!_canGetEffects) return;

            // create the new effect
            Effect newEffect = new Effect();
            newEffect.Duration = element.GetDuration();
            newEffect.ElementEffect = element;
            newEffect.Active = true;

            // check for stacks
            List<ElementEffect.StackRules> stacks = element.GetStackRules();

            // if any of the effects taking place contain an element that the new element has a stacking rule with
            if (_effects.Exists(e => stacks.Exists(s => s.Element == e.ElementEffect.ElementTag)))
            {
                // cache the effect that is already taking place
                Effect stackedEffect = _effects.Find(e => stacks.Exists(s => s.Element == e.ElementEffect.ElementTag));
                // cache the activate stack
                ElementStack stack = stacks.Find(s => s.Element == stackedEffect.ElementEffect.ElementTag).Stack;
                // perform the stack method
                stack.StackEffects(stackedEffect, newEffect, this);

                // this.Log($"{element.ElementTag} has a stack definition for {stackedEffect.ElementEffect.ElementTag}", CD.DebugType.Elements);
            }

            // after checking on stacking rules, we add the effect to the list
            _effects.Add(newEffect);

            // if the effect is active, then activate it. Otherwise, remove it from the list
            if (newEffect.Active)
            {
                //this.Log($"It is active", Color.green, CD.DebugType.Elements);
                newEffect.PrcoessCoroutine = StartCoroutine(EffectProcess(newEffect));
            }
            else
            {
                //this.Log($"It is not active", Color.red, CD.DebugType.Elements);
                _effects.Remove(newEffect);
            }
        }

        public IEnumerator EffectProcess(Effect effect)
        {
            //int duration = element.GetDuration();
            _effectParticle = null;
            _tankColor = Color.black;
            _tankMaterial = _components.Visuals.GetOriginalMaterial();

            // play the VFX of the effect if the element has any
            if (effect.ElementEffect.GetParticleTag() != PoolObjectType.NONE)
            {
                _effectParticle = GameManager.Instance.PoolingManager.GetObject(effect.ElementEffect.GetParticleTag()).GetComponent<ParticleSystem>();
                _effectParticle.transform.parent = transform;
                _effectParticle.transform.localPosition = Vector3.zero;
                _effectParticle.gameObject.SetActive(true);
            }

            // set the effect material if the effect has a material
            if (effect.ElementEffect.GetColorOverlay(ref _tankColor))
            {
                _components.Visuals.AddColorToMeshes(_tankColor);
            }

            // set the effect color if the effect has a color
            if (effect.ElementEffect.GetEffectMaterial(ref _tankMaterial))
            {
                if(effect.ElementEffect.AdditiveMaterial) _components.Visuals.AddMaterial(_tankMaterial);
                else _components.Visuals.SwitchMaterial(_tankMaterial);
            }

            // if the effect is continuous then it will take place every loop. Otherwise, it just happens at the start and then the loop only acts as a counter
            if (effect.ElementEffect.ContinuousEffect)
            {
                for (effect.Counter = 0; effect.Counter < effect.Duration; effect.Counter++)
                {
                    if (!effect.Active) break;

                    effect.ElementEffect.TakeEffect(_components);
                    yield return _waitingTime;
                }
            }
            else
            {
                effect.ElementEffect.TakeEffect(_components);

                for (effect.Counter = 0; effect.Counter < effect.Duration; effect.Counter++)
                {
                    if (!effect.Active) break;

                    yield return _waitingTime;
                }
            }

            effect.ElementEffect.StopEffect(_components);

            // turn off the VFX after the effect stops taking place
            if(_effectParticle != null) _effectParticle.Stop();
                
            // return the color to normal
            if(effect.ElementEffect.GetColorOverlay(ref _tankColor)) _components.Visuals.SubtractColorFromMeshes(_tankColor);
            
            if (effect.ElementEffect.GetEffectMaterial(ref _tankMaterial))
            {
                if (effect.ElementEffect.AdditiveMaterial) _components.Visuals.RemoveMaterial();
                else _components.Visuals.RestoreOriginalMaterial();
            }
            
            effect.Active = false;
            // end the effect (does it wipe it out existence? or is it stored somewhere?)
            _effects.Remove(effect);
        }

        public Effect GetEffectByTag(Element elementTag)
        {
            return _effects.Find(e => e.ElementEffect.ElementTag == elementTag);
        }

        public void SetCanGetEffects(bool canGetEffects)
        {
            _canGetEffects = canGetEffects;
        }

        #region IController
        public virtual void Activate()
        {
            _canGetEffects = true;
            IsActive = true; // not used yet
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            foreach (Effect effect in _effects)
            {
                effect.ElementEffect.StopEffect(_components);

                // turn off the VFX after the effect stops taking place
                if (_effectParticle != null)
                {
                    _effectParticle.Stop();
                    _effectParticle.gameObject.SetActive(false);
                }

                // return the color to normal
                if (effect.ElementEffect.GetColorOverlay(ref _tankColor)) _components.Visuals.RestoreOriginalColor();

                if (effect.ElementEffect.GetEffectMaterial(ref _tankMaterial))
                {
                    if (effect.ElementEffect.AdditiveMaterial)
                    {
                        _components.Visuals.RemoveMaterial();
                    }
                    else
                    {
                        _components.Visuals.RestoreOriginalMaterial();
                    }
                }

                if (effect.PrcoessCoroutine != null)
                {
                    StopCoroutine(effect.PrcoessCoroutine);
                }
            }

            _effects.Clear();
        }

        public virtual void Dispose()
        {

        }
        #endregion
    }

    public enum Element
    {
        Fire, Ice, Lightning, Air
    }
}
