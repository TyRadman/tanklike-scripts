using System.Collections;
using UnityEngine;

namespace TankLike.Environment
{
    using TankLike.Cam;
    using TankLike.UnitControllers;

    public class BossRoom : Room
    {
        [field: SerializeField] public Vector3 RoomSize { get; private set; }
        
        [SerializeField] private AbilityConstraint _onCinematicConstraints;
        [SerializeField] private AbilityConstraint _onRoomEnterConstraints;

        private BossData _bossData;
        private GameObject _boss;
        private Transform _bossSpawnPoint;

        public override void SetUpRoom()
        {
            base.SetUpRoom();

            foreach (GateInfo gate in GatesInfo.Gates)
            {
                gate.Gate.Setup(true, this);
            }
        }

        public override void LoadRoom()
        {
            base.LoadRoom();
            _boss = Instantiate(_bossData.BossPrefab, _bossSpawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
            GameManager.Instance.PlayersManager.ApplyConstraints(true, _onRoomEnterConstraints);
        }

        public void SetBossSpawnPoint(Transform point)
        {
            _bossSpawnPoint = point;
        }

        public Transform GetSpawnPoint()
        {
            return _bossSpawnPoint;
        }

        public void SetBossData(BossData bossData)
        {
            _bossData = bossData;
            RoomSize = bossData.RoomSize;
        }

        public override void OnRoomEnteredHandler()
        {
            base.OnRoomEnteredHandler();

            // close the gates
            CloseGates();
            StartCoroutine(BossStartProcess());
        }

        private IEnumerator BossStartProcess()
        {
            GameManager.Instance.PlayersManager.ApplyConstraints(false, _onRoomEnterConstraints);

            // disable the players movement
            GameManager.Instance.PlayersManager.ApplyConstraints(true, _onCinematicConstraints);

            // Fade out background music
            GameManager.Instance.AudioManager.FadeOutBGMusic();

            GameManager.Instance.CameraManager.PlayerCameraFollow.MoveToPoint(
                _boss.transform, _bossData.CameraMovementToBossDuration);

            yield return new WaitForSeconds(_bossData.CameraMovementToBossDuration);

            GameManager.Instance.BossesManager.AddBoss(_boss.GetComponent<BossComponents>());
            GameManager.Instance.EnemiesManager.AddEnemy(_boss.GetComponent<BossComponents>());

            yield return new WaitForSeconds(_bossData.BossAnimationDuration);

            GameManager.Instance.HUDController.BossHUD.DisplayBossHUD();
            GameManager.Instance.CameraManager.Zoom.SetToZoomValue(_bossData.ZoomValue);

            GameManager.Instance.PlayersManager.ApplyConstraints(false, _onCinematicConstraints);
            // Switch the background music to the boss theme
            GameManager.Instance.AudioManager.SwitchBGMusic(_bossData.BossBGMusic);
            // Fade in background music
            GameManager.Instance.AudioManager.FadeInBGMusic();
            GameManager.Instance.CameraManager.PlayerCameraFollow.MoveBackToPlayers(_bossData.CameraMovementToBossDuration);

            yield return new WaitForSeconds(_bossData.CameraMovementToBossDuration);

            GameManager.Instance.CameraManager.PlayerCameraFollow.EnableFollowingPlayer();
        }
    }
}
