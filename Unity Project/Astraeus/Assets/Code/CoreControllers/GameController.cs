using UnityEngine;

namespace Code.CoreControllers {
    public class GameController : MonoBehaviour {
        public enum GameFocus {
            Map,
            SolarSystem,
            PauseMenu,
            MainMenu
        }

        public static GameFocus GameStateFocus;
        
        
    }
}