using Code._GameControllers;
using Code._Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.ShipGUI {
    public class ShipGUIController : MonoBehaviour{
        public GameObject guiGameObject;

        private void Awake() {
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
            GameController.IsPaused = true;
            GameController.GUIController.SetupGalaxyMap();
        }
        
        private void LocalMapBtnClick() {
            GameController.IsPaused = true;
            GameController.GUIController.SetupLocalMapGUI();
        }

        public void SetSystemDetails() {
            GameObjectHelper.SetGUITextValue(guiGameObject, "SystemName",GameController.CurrentSolarSystem.SystemName);
            GameObjectHelper.SetGUITextValue(guiGameObject, "FactionValue", GameController.CurrentSolarSystem.OwnerFaction == null ? "Unoccupied" : GameController.CurrentSolarSystem.OwnerFaction.GetFactionName());
        }
    }
}