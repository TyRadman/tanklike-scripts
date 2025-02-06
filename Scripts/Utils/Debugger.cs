using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike
{
    public class Debugger : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToScan;
        [SerializeField]
        private string scriptName = ""; // Type the name of the script you're looking for in the Unity Editor
        [Header("Layers finder")]
        [SerializeField] private int _layerToFind;
        [SerializeField] private List<GameObject> _objectsWithLayer;

        [ContextMenu("Find Missed Script")]
        public void LookForMissingScripts()
        {
            FindMissingScripts(_objectToScan);
        }

        private void FindMissingScripts(GameObject obj)
        {
            foreach (Component component in obj.GetComponents<Component>())
            {
                if (component == null)
                {
                    Debug.Log("Missing script found in GameObject: " + obj.name);
                    break;
                }
            }

            foreach (Transform child in obj.transform)
            {
                FindMissingScripts(child.gameObject);
            }
        }


        [ContextMenu("Find Objects With Script")]
        private void FindObjectsWithSpecifiedScript()
        {
            Type scriptType = Type.GetType(scriptName);
            if (scriptType == null)
            {
                Debug.LogError("Script type '" + scriptName + "' not found.");
                return;
            }

            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.transform.parent == null) // Start with root objects
                {
                    FindInChild(obj, "", scriptType);
                }
            }
        }

        private void FindInChild(GameObject obj, string path, Type scriptType)
        {
            string currentPath = string.IsNullOrEmpty(path) ? obj.name : $"{path} -> {obj.name}";

            // Check if the current object has the script
            Component script = obj.GetComponent(scriptType);
            if (script != null)
            {
                Debug.Log("Path to object with " + scriptType.Name + ": " + currentPath);
            }

            // Recursively search in children
            foreach (Transform child in obj.transform)
            {
                FindInChild(child.gameObject, currentPath, scriptType);
            }
        }

        [ContextMenu("Find Objects With Layer")]
        private void GetObjectsWithLayer()
        {
            _objectsWithLayer = new List<GameObject>();
            FindObjectsOfType<GameObject>().ToList().
                FindAll(o => o.layer == _layerToFind).
                ForEach(o => _objectsWithLayer.Add(o));
        }
    }
}