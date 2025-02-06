using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.SkillTree
{
    using UnitControllers;
    using UI;
    using TankLike.UI.Signifiers;
    using UnityEngine.Windows;

    public class SkillTreesManager : Navigatable, IManager
    {
        [SerializeField] private List<SkillTreeHolder> _skillTrees = new List<SkillTreeHolder>();
        [SerializeField] private Transform _skillTreeParent;

        #region IManager
        public override void SetUp()
        {
            IsActive = true;

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(i);
                SkillTreeHolder skillTreePrefab = player.SkillsController.GetSkillTree();
                SkillTreeHolder newSkillTree = Instantiate(skillTreePrefab, _skillTreeParent);
                newSkillTree.gameObject.name = $"Skill Tree {player.PlayerIndex}";
                _skillTrees.Add(newSkillTree);
                newSkillTree.SetPlayer(player);
                newSkillTree.SetUp();
            }
        }

        public override void Dispose()
        {
            IsActive = false;

            _skillTrees.ForEach(st => st.Dispose());
            _skillTrees.ForEach(st => Destroy(st.gameObject));
            _skillTrees.Clear();
        }
        #endregion

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            base.SetUpActionSignifiers(signifierController);

            if (signifierController is UIActionSignifiersController actionSignifiersController)
            {
                _skillTrees.ForEach(s => s.SetUpActionSignifiers(actionSignifiersController));
            }
        }

        public override void Open(int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            base.Open(playerIndex);
            _skillTrees.ForEach(s => s.gameObject.SetActive(false));
            _skillTrees[playerIndex].gameObject.SetActive(true);
            _skillTrees[playerIndex].Open(playerIndex);
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);

            _skillTrees[playerIndex].Close(playerIndex);
        }
    }
}
