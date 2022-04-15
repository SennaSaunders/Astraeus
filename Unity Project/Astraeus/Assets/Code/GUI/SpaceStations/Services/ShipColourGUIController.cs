using Code._GameControllers;
using UnityEngine;

namespace Code.GUI.SpaceStations.Services {
    public class ShipColourGUIController : MonoBehaviour{
        private OutfittingGUIController _outfittingGUIController;
        private GameObject _outfittingGUIGameObject;
        private GameObject _shipColourGUI;
        
        public void SetupGUI(OutfittingGUIController outfittingGUIController, GameObject outfittingGUIGameObject) {
            _outfittingGUIController = outfittingGUIController;
            _outfittingGUIGameObject = outfittingGUIGameObject;
            _shipColourGUI = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Station/Services/Outfitting/ShipColourGUI"));
        }

    }
}