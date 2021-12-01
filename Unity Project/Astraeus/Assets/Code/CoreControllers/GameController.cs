using UnityEngine;

namespace Code.CoreControllers {
    public class GameController : MonoBehaviour {
        public enum GameFocus {
            Map,
            SolarSystem,
            Paused
        }

        public static GameFocus GameStateFocus;
        
        
    }
}