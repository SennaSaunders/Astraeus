using Code._GameControllers;
using Code._Ships.Hulls;
using Code._Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.ShipDestroyed {
    public class ShipDestroyedGUIController : MonoBehaviour {
        private GameController _gameController;

        private void Awake() {
            _gameController = GameObjectHelper.GetGameController();
            SetupButtons();
        }

        private void SetupButtons() {
            GameObjectHelper.FindChild(gameObject, "RespawnBtn").GetComponent<Button>().onClick.AddListener(Respawn);
        }

        private void Respawn() {
            if (_gameController.CurrentStation.SolarSystem!= _gameController.CurrentSolarSystem) {
                _gameController.ChangeSolarSystem(_gameController.GalaxyController.GetSolarSystemController(_gameController.CurrentStation.SolarSystem), false);
            }
            
            Hull hull = _gameController.CurrentShip.ShipHull;
            _gameController.CurrentShip.ShipHull.CurrentHullStrength = .5f * hull.BaseHullStrength;
            _gameController.PlayerShipController.ResetShields();
            _gameController.RefreshPlayerShip();
            _gameController.StartGame();
            Destroy(gameObject);
        }
    }
}