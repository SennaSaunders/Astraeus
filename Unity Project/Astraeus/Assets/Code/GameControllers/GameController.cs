using UnityEngine;

namespace Code.GameControllers {
    public class GameController : MonoBehaviour {
        public enum GameFocus {
            Map,
            SolarSystem,
            Paused
        }

        public static GameFocus GameStateFocus;
        
        
    }
}