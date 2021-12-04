using System;
using System.Threading;
using Code.Galaxy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI {
    public class GalaxyGeneratorGUIController : MonoBehaviour {
        private GalaxyGenerator _generator;
        private GameObject _guiGameObject;
        private GameObject _loadingScreen;
        private Galaxy.Galaxy _galaxy;
        private Thread _generationThread;
        public void Start() {
            SetGUIGameObject();
            _generator = gameObject.AddComponent<GalaxyGenerator>();
            if (!LoadPreviousGeneratorConfig()) LoadDefaultGeneratorConfig();
            InitFields();
            SetupGenerateGalaxyBtn();
        }

        private void SetGUIGameObject() {
            _guiGameObject = GameObject.Find("GalaxyGenerationGUI");
        }

        private bool LoadPreviousGeneratorConfig() {
            return false;
        }

        private void LoadDefaultGeneratorConfig() {
            _generator.seed.SetValue((int)DateTime.Now.Ticks);
            _generator.maxSystems.SetValue(500);
            _generator.width.SetValue(1000);
            _generator.height.SetValue(500);
            _generator.minBodiesPerSystem.SetValue(1);
            _generator.maxBodiesPerSystem.SetValue(10);
            _generator.systemExclusionDiameter.SetValue(5);
        }
        
        private void InitSlider(string newName, GalaxyGeneratorInput input) {
            GameObject inputObject = GameObject.Find(newName);
            Slider slider = inputObject.GetComponent<Slider>();
            GalaxyGenSliderController sliderController = inputObject.GetComponent<GalaxyGenSliderController>();

            if (sliderController == null) {
                sliderController = inputObject.AddComponent<GalaxyGenSliderController>();
            }
            
            input.AddObserver(sliderController);
            sliderController.SetInput(input);
        }

        private void InitTextField(string newName, GalaxyGeneratorInput input) {
            GameObject inputObject = GameObject.Find(newName);
            TMP_InputField txtField = inputObject.GetComponent<TMP_InputField>();
            GalaxyGenInputController inputController = inputObject.GetComponent<GalaxyGenInputController>();
            if (inputController == null) {
                inputController = inputObject.AddComponent<GalaxyGenInputController>();
            }
            input.AddObserver(inputController);
            inputController.SetInput(input);
        }

        private void InitFields() {
            string fieldName = "SeedInput";
            GalaxyGeneratorInput input = _generator.seed;
            InitTextField(fieldName, input);
            input.NotifyObservers();
            
            fieldName = "WidthInput";
            input = _generator.width;
            InitTextField(fieldName, input);
            
            fieldName= "WidthSlider";
            InitSlider(fieldName,input);
            input.NotifyObservers();

            fieldName = "HeightInput";
            input = _generator.height;
            InitTextField(fieldName,input);

            fieldName = "HeightSlider";
            InitSlider(fieldName,input);
            input.NotifyObservers();
            
            fieldName = "MaxSystemsInput";
            input = _generator.maxSystems;
            InitTextField(fieldName,input);
            
            fieldName = "MaxSystemsSlider";
            input = _generator.maxSystems;
            InitSlider(fieldName,input);
            input.NotifyObservers();
            
            fieldName = "SmallestSystemInput";
            input = _generator.minBodiesPerSystem;
            InitTextField(fieldName,input);
            
            fieldName = "SmallestSystemSlider";
            input = _generator.minBodiesPerSystem;
            InitSlider(fieldName,input);
            input.NotifyObservers();
            
            fieldName = "LargestSystemInput";
            input = _generator.maxBodiesPerSystem;
            InitTextField(fieldName,input);
            
            fieldName = "LargestSystemSlider";
            input = _generator.maxBodiesPerSystem;
            InitSlider(fieldName,input);
            input.NotifyObservers();
            
            fieldName = "MinSystemDistanceInput";
            input = _generator.systemExclusionDiameter;
            InitTextField(fieldName,input);
            
            fieldName = "MinSystemDistanceSlider";
            input = _generator.systemExclusionDiameter;
            InitSlider(fieldName,input);
            input.NotifyObservers();
        }

        private void SetupGenerateGalaxyBtn() {
            Button button = GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { GenerateGalaxy(); });
        }
        
        private void GenerateGalaxy() {
            _loadingScreen = (GameObject)Resources.Load("GUIPrefabs/LoadingGUI");
            _loadingScreen = Instantiate(_loadingScreen);
            //Destroy(_guiGameObject);
            
            _generationThread = new Thread(() => {
                _galaxy = _generator.GenGalaxy();
                //Loaded();
            });
            _generationThread.Start();
            Destroy(GameObject.Find("GUIHolder"));//removes visual elements of the GUI so that it doesn't cover the loading screen - may be better to pass the Thread to the loading screen and destroy itself?
        }

        private void Loaded() {
            Destroy(_loadingScreen);
            _generator.ShowGalaxy(_galaxy);
            Destroy(_guiGameObject);
        }

        public void Update() {
            if (_generationThread != null) {
                if (_generationThread.IsAlive) {
                    Debug.Log("Running..");
                }
                else {
                    Loaded();
                    Debug.Log("Stopped");
                }
            }
        }
    }
}