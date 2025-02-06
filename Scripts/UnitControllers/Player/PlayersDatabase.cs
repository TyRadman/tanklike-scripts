using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "Players_DB_Default", menuName = Directories.PLAYERS + "Players DB")]
    public class PlayersDatabase : ScriptableObject
    {
        [SerializeField] private List<PlayerData> _players;

        private Dictionary<PlayerType, PlayerData> _playersDB;

        private void OnEnable()
        {
            _playersDB = new Dictionary<PlayerType, PlayerData>();
            foreach (var player in _players)
                _playersDB.Add(player.PlayerType, player);
        }

        public PlayerData GetPlayerDataByType(PlayerType type)
        {
            if (_playersDB.ContainsKey(type))
                return _playersDB[type];

            Debug.Log("Players DB does not contain a player with type -> " + type);
            return null;
        }

        public List<PlayerData> GetAllPlayers()
        {
            return _players;
        }
    }
}
