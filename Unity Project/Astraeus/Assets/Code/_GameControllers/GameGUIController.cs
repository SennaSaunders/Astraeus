using UnityEngine;

namespace Code._GameControllers {
    public class GameGUIController : MonoBehaviour {
        private GameController _gameController;

        public void SetupGameController(GameController gameController) {
            _gameController = gameController;
        }

    }
}