using System.Collections.Generic;
using Code.Saves;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.GUI {
    public class MainMenuController : MonoBehaviour {
        private GameObject _menu;
        private List<GameSave> _saves = new List<GameSave>();
        private List<(GameObject obj, UnityAction func)> _buttonObjFuncTuple;

        public void Start() {
            SetMenu();
            SetButtonObjects();
            SetupMenu();
        }
        private void SetButtonObjects() {
            _buttonObjFuncTuple = new List<(GameObject, UnityAction function)>(){
                (GameObject.Find("ContinueBtn"), ContinueBtnClick),
                (GameObject.Find("NewGameBtn"), NewGameBtnClick),
                (GameObject.Find("LoadGameBtn"), LoadGameBtnClick),
                (GameObject.Find("SettingsBtn"), SettingsBtnClick),
                (GameObject.Find("ExitBtn"), ExitBtnClick)
            };
        }

        private void SetMenu() {
            string menuName = "MenuGUI";
            _menu = GameObject.Find(menuName);
        }

        private bool GetGameSaves() {
            SetSaves();

            return _saves.Count > 0;
        }

        private void SetSaves() {
            
        }

        private void SetupNoSavesMenu() {
            //remove continue button
            GameObject buttonObject = GetButtonObjectFromName("ContinueBtn");
            buttonObject.SetActive(false);
            //remove load game button
            buttonObject = GetButtonObjectFromName("LoadGameBtn");
            buttonObject.SetActive(false);
        }

        private static void InitFocusedButton(Button button) {
            button.Select();
        }

        private void SetupMenu() {
            if (!GetGameSaves()) {
                SetupNoSavesMenu();
                InitFocusedButton(GetButtonComponent(GetButtonObjectFromName("NewGameBtn")));
            }
            else {
                InitFocusedButton(GetButtonComponent(GetButtonObjectFromName("ContinueBtn")));
            }
            SetupActiveButtons();
        }

        private GameObject GetButtonObjectFromName(string buttonName) {
            return _buttonObjFuncTuple.Find(b => b.obj.name == buttonName).obj;
        }

        private static Button GetButtonComponent(GameObject buttonObject) {
            return buttonObject.GetComponent<Button>();
        }

        private void SetupActiveButtons() {
            foreach ((GameObject obj, UnityAction func) buttonObject in _buttonObjFuncTuple) {
                if (buttonObject.obj.activeSelf) {
                    Button button = GetButtonComponent(buttonObject.obj);  
                    button.onClick.AddListener( buttonObject.func);
                }
            }
        }
        private static void ContinueBtnClick() {
            //load last save
            Debug.Log("Pressed Continue");
        }

        private void NewGameBtnClick() {
            //galaxy gen GUI
            Debug.Log("Pressed New Game");
            GameObject newGUI = (GameObject)Resources.Load("GUIPrefabs/GalaxyGenerationGUI");
            newGUI = Instantiate(newGUI);
            newGUI.name = "GalaxyGenerationGUI";
            Destroy(_menu);
        }

        private static void LoadGameBtnClick() {
            //brings up all saved games to choose from
            Debug.Log("Pressed Load Game");
        }

        private static void SettingsBtnClick() {
            //goes to settings
            Debug.Log("Pressed Settings");
        }

        private static void ExitBtnClick() {
            //pops up a confirmation prompt which quits the game if yes
            Debug.Log("Pressed Exit");
        }
    }
}