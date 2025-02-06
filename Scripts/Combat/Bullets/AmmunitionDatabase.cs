using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.Combat
{
    using BT;

    [CreateAssetMenu(fileName = "Ammunition_DB_Default", menuName = Directories.AMMUNITION + "Ammunition Database")]
    public class AmmunitionDatabase : ScriptableObject
    {
        [SerializeField] private List<AmmunationData> _ammunitions;

        [SerializeField] private SerializableDictionary<string, AmmunationData> _ammunitionsDB;
        [field: SerializeField] public string DirectoryToCover { get; private set; }

        private void OnEnable()
        {
            FillDictionary();
        }

        public void FillDictionary()
        {
            _ammunitionsDB = new SerializableDictionary<string, AmmunationData>();

            foreach (AmmunationData ammunition in _ammunitions)
            {
                if (!_ammunitionsDB.ContainsKey(ammunition.GUID))
                {
                    //string guid = GUID.Generate().ToString();
                    string guid = ammunition.GetInstanceID().ToString();
                    ammunition.GUID = guid;
                    _ammunitionsDB.Add(ammunition.GUID, ammunition);
                }
            }
        }

        public AmmunationData GetBulletDataFromGUID(string guid)
        {
            if (_ammunitionsDB.ContainsKey(guid))
            {
                return _ammunitionsDB[guid];
            }

            Debug.Log("Bullets DB does not contain a bullet with GUID -> " + guid);
            return null;
        }

        public List<AmmunationData> GetAllBullets()
        {
            List<AmmunationData> data = new List<AmmunationData>();
            _ammunitions.FindAll(a => a.Ammunition is Bullet).ForEach(a => data.Add(a));
            return data;
        }

        public List<AmmunationData> GetAllLaser()
        {
            List<AmmunationData> data = new List<AmmunationData>();
            _ammunitions.FindAll(a => a.Ammunition is Laser).ForEach(a => data.Add(a));
            return data;
        }

        public string GetAmmunitionName()
        {
            return nameof(_ammunitions);
        }

        public void AddAmmunition(AmmunationData ammo)
        {
            if(string.IsNullOrEmpty(ammo.GUID))
            {
                string guid = ammo.GetInstanceID().ToString();
                ammo.GUID = guid;
            }

            _ammunitions.Add(ammo);
        }

        public void ClearAmmunitionList()
        {
            _ammunitions.Clear(); 
            _ammunitionsDB.Clear();
        }
    }
}
