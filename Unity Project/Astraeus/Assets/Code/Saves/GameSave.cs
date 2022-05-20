using Code._GameControllers;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Saves {
    public class GameSave :MonoBehaviour {
        //Galaxy Gen Params
        private string string1;
        private string string2;
        
        public void SaveGame() {
            string1 = JsonConvert.SerializeObject(new Laser(ShipComponentTier.T1));
            // string2 = JsonConvert.SerializeObject(typeof(ShieldHighCapacity));
        }

        public void LoadGame() {
            Debug.Log((Laser)JsonConvert.DeserializeObject<Weapon>(string1));
        }

        private void SavePlayerProfile() {
            
        }

        private void SavePlayerShips() {
            var galaxyString = JsonConvert.SerializeObject(GameController.PlayerProfile.Ships);
            Debug.Log(galaxyString);
        }

        private void SaveMissions() {
            var galaxyString = JsonConvert.SerializeObject(GameController.PlayerProfile.Missions);
            Debug.Log(galaxyString);
        }
    }
}