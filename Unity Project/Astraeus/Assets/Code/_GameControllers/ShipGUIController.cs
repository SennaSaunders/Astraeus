using Code._Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code._GameControllers {
    public class ShipGUIController : MonoBehaviour{
        public GameObject guiGameObject;

        private void Awake() {
            SetupShipGUI();
        }

        private void SetupShipGUI() {
            guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/ShipGUI"));
            SetupButtons();
        }

        private void SetupButtons() {
            Button galaxyMapButton = GameObjectHelper.FindChild(guiGameObject, "GalaxyMapBtn").GetComponent<Button>();
            galaxyMapButton.onClick.AddListener(GalaxyMapBtnClick);
        }

        private void GalaxyMapBtnClick() {
            GameController.isPaused = true;
            GameController.GUIController.SetupGalaxyMap();
        }
    }
}