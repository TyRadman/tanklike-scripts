using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.UnitControllers;

namespace TankLike.EditorStuff
{
    [CustomEditor(typeof(DamageDetector))]
    public class DamageDetectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DamageDetector detector = (DamageDetector)target;

            // if there is no TankHealth assigned to the damage detector, then do nothing
            if (!detector.HasHealthReference()) 
            {
                Transform currentParent = detector.transform;
                TankHealth tankHealth = null;

                while(true)
                {
                    if(currentParent.parent == null)
                    {
                        break;
                    }

                    if (currentParent.parent.TryGetComponent(out TankHealth health))
                    {
                        tankHealth = health;
                        break;
                    }

                    currentParent = currentParent.parent;
                }

                detector.SetHealth(tankHealth);
                return;
            }

            if (!GUI.changed) return;

            int layer = detector.GetLayerMask();
            detector.gameObject.layer = layer;
        }
    }
}
