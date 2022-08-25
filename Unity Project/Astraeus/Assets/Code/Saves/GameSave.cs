using System;
using Code._GameControllers;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.ExternalComponents.Weapons.Types;
using Code._Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Saves {
    public class GameSave :MonoBehaviour {
        //Galaxy Gen Params
        private string string1;
        private GameController _gameController;


        public void SaveGame() {
            Laser weapon = new Laser(ShipComponentTier.T1);
            Debug.Log(weapon.FireDelay+"\n"+weapon.Damage+"\n"+weapon.ProjectileSpeed+"\n"+weapon.MaxTravelTime+"\n"+weapon.PowerDraw+"\n"+weapon.RotationSpeed+"\n"+weapon.ComponentName+"\n"+weapon.ComponentType+"\n"+weapon.ComponentSize+"\n"+weapon.ComponentMass+"\n"+weapon.ComponentPrice);
            string1 = JsonConvert.SerializeObject(weapon,Formatting.Indented);
            Debug.Log(string1);
        }

        public void LoadGame() {
            var weapon = (Laser)JsonConvert.DeserializeObject(string1);
            if (weapon != null) {
                
                Debug.Log(weapon.GetType());
                Debug.Log(weapon.FireDelay+"\n"+weapon.Damage+"\n"+weapon.ProjectileSpeed+"\n"+weapon.MaxTravelTime+"\n"+weapon.PowerDraw+"\n"+weapon.RotationSpeed+"\n"+weapon.ComponentName+"\n"+weapon.ComponentType+"\n"+weapon.ComponentSize+"\n"+weapon.ComponentMass+"\n"+weapon.ComponentPrice);
            }
            else {
                Debug.Log("failed");
            }
            
        }

        private void SavePlayerProfile() {
            
        }

        private void SavePlayerShips() {
            var galaxyString = JsonConvert.SerializeObject(_gameController.PlayerProfile.Ships);
            Debug.Log(galaxyString);
        }

        private void SaveMissions() {
            var galaxyString = JsonConvert.SerializeObject(_gameController.PlayerProfile.Missions);
            Debug.Log(galaxyString);
        }
    }
}