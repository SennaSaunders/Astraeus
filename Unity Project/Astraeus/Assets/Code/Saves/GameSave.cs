using Code._GameControllers;
using Newtonsoft.Json;
using UnityEngine;

namespace Code.Saves {
    public class GameSave :MonoBehaviour{
        private string jsonString;

        public void SaveGame() {
            jsonString = JsonConvert.SerializeObject(GameController.PlayerProfile);
            Debug.Log(jsonString);
        }
    }
}