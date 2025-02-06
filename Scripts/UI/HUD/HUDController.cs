using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.UI
{
    using UI.HUD;

    public class HUDController : MonoBehaviour, IManager
    {
        [field: SerializeField] public List<PlayerHUD> PlayerHUDs { get; private set; }
        [field: SerializeField] public BossHUD BossHUD { private set; get; }
        [field: SerializeField] public CoinsHUD CoinsHUD { private set; get; }
        [field: SerializeField] public DamageScreenUIController DamageScreenUIController { private set; get; }
        [field: SerializeField] public PlayersOffScreenIndicator OffScreenIndicator { get; private set; }

        [SerializeField] private TextMeshProUGUI _keysCountText;

        public bool IsActive { get; private set; }

        public void SetReferences()
        {
            OffScreenIndicator.SetReferences();
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            for (int i = 0; i < PlayerHUDs.Count; i++)
            {
                PlayerHUDs[i].PlayerIndex = i;
                PlayerHUDs[i].SetUp();
            }

            CoinsHUD.SetUp();
            DamageScreenUIController.SetUp();
            BossHUD.SetUp();
            //OffScreenIndicator.SetUp();

            DisplayHUD();

            GameManager.Instance.PlayersManager.Coins.OnCoinsAdded += DisplayCoins;
        }

        public void Dispose()
        {
            IsActive = false;

            PlayerHUDs.ForEach(p => p.Dispose());
            CoinsHUD.Dispose();
            DamageScreenUIController.Dispose();
            BossHUD.Dispose();
            OffScreenIndicator.Dispose();

            GameManager.Instance.PlayersManager.Coins.OnCoinsAdded -= DisplayCoins;
        }
        #endregion

        public void DisplayHUD()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayerHUDs.ForEach(h => h.Disable());

            GameManager.Instance.MinimapManager.Display();

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                PlayerHUDs[i].Enable();
            }
        }

        public void HideHUD()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            PlayerHUDs.ForEach(h => h.Disable());

            GameManager.Instance.MinimapManager.Hide();
        }

        private void DisplayCoins(int addedCoins, int totalCoins)
        {
            CoinsHUD.DisplayCoinsText(totalCoins);
        }

        public void UpdateKeysCount(int currentCount, int maxCount)
        {
            _keysCountText.text = $"{currentCount} / {maxCount}";
        }
    }
}
