using UnityEngine;

namespace Code.CoreControllers {
    public class MainMenuController : MonoBehaviour {
        public GameObject menu;
        
        public void DisplayMainMenu() {
            string menuName = "menu";
            GameObject.Find(menuName);
        }
    }
}