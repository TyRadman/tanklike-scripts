using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;
using TankLike.Utils;

namespace TankLike.UnitControllers
{
    public abstract class TankTools : MonoBehaviour, IController
    {
        [System.Serializable]
        public class ToolPack
        {
            public Tool Tool;
            public ToolInfo Info;
        }

        [SerializeField] protected List<ToolPack> _tools = new List<ToolPack>();
        [SerializeField] protected ToolPack _currentTool = new ToolPack();

        protected TankComponents _components;
        
        public bool IsActive { get; protected set; }

        public virtual void SetUp(IController controller)
        {
            if (controller == null || controller is not TankComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = (TankComponents)controller;
        }

        public virtual void AddTool(ToolInfo toolInfo, int count)
        {
            if (toolInfo == null)
            {
                return;
            }

            Tool newTool = null;

            // if the player doesn't have this tool already, then create a new one
            if (!_tools.Exists(t => t.Tool.GetTag() == toolInfo.ToolReference.GetTag()))
            {
                newTool = ScriptableObject.Instantiate(toolInfo.ToolReference);
                //newTool = Instantiate(toolInfo.ToolReference, transform);
                // set up the tool
                newTool.SetUp(_components);
                // set the amount of the tool
                newTool.SetAmount(count);

                _tools.Add(new ToolPack() { Info = toolInfo, Tool = newTool});

                // if there are tools already, then don't set this tool as the active one
                if (_tools.Count == 1)
                {
                    _currentTool = _tools[0];
                }
            }
            // otherwise, just add it to what the player has already
            else
            {
                // the one that we already have
                newTool = _tools.Find(t => t.Tool.GetTag() == toolInfo.ToolReference.GetTag()).Tool;
                newTool.AddAmount(count);
            }
        }

        public virtual void UseTool()
        {
            if (!_currentTool.Tool.HasEnoughAmount() || !_currentTool.Tool.IsReady()) return;

            _currentTool.Tool.UseTool();
        }

        public List<ToolPack> GetTools()
        {
            return _tools;
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            IsActive = false;
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
