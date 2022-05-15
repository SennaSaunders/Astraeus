using Code._GameControllers;
using Code._Ships.Hulls;
using Code._Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.ShipDestroyed {
    public class ShipDestroyedGUIController : MonoBehaviour {
        private GameController _gameController;

        private void Awake() {
            _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
            SetupButtons();
        }

        private void SetupButtons() {
            GameObjectHelper.FindChild(gameObject, "RespawnBtn").GetComponent<Button>().onClick.AddListener(Respawn);
        }

        private void Respawn() {
            if (GameController.CurrentStation.SolarSystem!= GameController.CurrentSolarSystem) {
                GameController.ChangeSolarSystem(GameController.GalaxyController.GetSolarSystemController(GameController.CurrentStation.SolarSystem), false);
            }
            
            Hull hull = GameController.CurrentShip.ShipHull;
            GameController.CurrentShip.ShipHull.CurrentHullStrength = .5f * hull.BaseHullStrength;
            GameController.PlayerShipController.ResetShields();
            _gameController.RefreshPlayerShip();
            GameController.StartGame();
            Destroy(gameObject);
        }
    }
}