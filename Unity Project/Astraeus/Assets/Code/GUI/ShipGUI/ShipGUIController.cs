using Code._GameControllers;
using Code._Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.ShipGUI {
    public class ShipGUIController : MonoBehaviour{
        public GameObject guiGameObject;
        private GameController _gameController;

        private void Awake() {
            _gameController = GameObjectHelper.GetGameController();
            SetupShipGUI();
        }

        private void SetupShipGUI() {
            guiGameObject = Instantiate((GameObject)Resources.Load("GUIPrefabs/Ship/ShipGUI"));
            SetupButtons();
        }

        private void SetupButtons() {
            Button galaxyMapButton = GameObjectHelper.FindChild(guiGameObject, "GalaxyMapBtn").GetComponent<Button>();
            galaxyMapButton.onClick.AddListener(GalaxyMapBtnClick);
            Button localMapButton = GameObjectHelper.FindChild(guiGameObject, "LocalMapBtn").GetComponent<Button>();
            localMapButton.onClick.AddListener(LocalMapBtnClick);
        }

        private void GalaxyMapBtnClick() {
            _gameController.IsPaused = true;
            _gameController.GUIController.SetupGalaxyMap();
        }
        
        private void LocalMapBtnClick() {
            _gameController.InLocalMap = true;
            _gameController.GUIController.SetupLocalMapGUI();
        }

        public void SetSystemDetails() {
            GameObjectHelper.SetGUITextValue(guiGameObject, "SystemName",_gameController.CurrentSolarSystem.SystemName);
            GameObjectHelper.SetGUITextValue(guiGameObject, "FactionValue", _gameController.CurrentSolarSystem.OwnerFaction == null ? "Unoccupied" : _gameController.CurrentSolarSystem.OwnerFaction.GetFactionName());
        }
    }
}