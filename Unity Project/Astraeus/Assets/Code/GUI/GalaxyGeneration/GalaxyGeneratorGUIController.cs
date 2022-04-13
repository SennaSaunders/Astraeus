using System;
using System.Threading;
using Code._Galaxy;
using Code._GameControllers;
using Code.GUI.Loading;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.GalaxyGeneration {
    public class GalaxyGeneratorGUIController : MonoBehaviour {
        private GalaxyGenerator _generator;
        private GameObject _guiGameObject;
        private GameObject _loadingScreen;
        private Galaxy _galaxy;
        private GameController _gameController;
        private Thread _galaxyGenerationThread;
        private Thread _textureGenerationThread;
        
        private bool _startedGalaxyGen;
        private bool _finishedgalaxyGen = false;
        private bool _startedTextureGen = false;
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
            _generator.systemExclusionDistance.SetValue(5);
        }
        
        private void InitSlider(string newName, GalaxyGeneratorInput input) {
            GameObject inputObject = GameObject.Find(newName);
            GalaxyGenSliderController sliderController = inputObject.GetComponent<GalaxyGenSliderController>();

            if (sliderController == null) {
                sliderController = inputObject.AddComponent<GalaxyGenSliderController>();
            }
            
            input.AddObserver(sliderController);
            sliderController.SetInput(input);
        }

        private void InitTextField(string newName, GalaxyGeneratorInput input) {
            GameObject inputObject = GameObject.Find(newName);
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
            input = _generator.systemExclusionDistance;
            InitTextField(fieldName,input);
            
            fieldName = "MinSystemDistanceSlider";
            input = _generator.systemExclusionDistance;
            InitSlider(fieldName,input);
            input.NotifyObservers();
        }

        private void SetupGenerateGalaxyBtn() {
            Button button = GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { GenerateGalaxy(); });
        }
        
        private void GenerateGalaxy() {
            StartLoadingScreen();
            _galaxyGenerationThread = new Thread(() => {
                _galaxy = _generator.GenGalaxy();
            });
            _galaxyGenerationThread.Start();
        }

        private void StartLoadingScreen() {
            _loadingScreen = (GameObject)Resources.Load("GUIPrefabs/LoadingGUI");
            _loadingScreen = Instantiate(_loadingScreen);
            LoadingScreenController.SetLoadingText("Generating Galaxy");
            Destroy(GameObject.Find("GUIHolder"));//removes visual elements of the GUI so that it doesn't cover the loading screen
        }

        private void DestroyLoadingScreen() {
            Destroy(_loadingScreen);
            Destroy(_guiGameObject);
        }

        private void GenerateStartingSystemColours() {
            _textureGenerationThread = GameController._galaxyController.GenerateSolarSystemColours(GameController._currentSolarSystem);
        }

        private void GenerateStartingSystemTextures() {
            GameController._galaxyController.GenerateSolarSystemTextures(GameController._currentSolarSystem);
        }

        private void InitialiseGameController() {
            Destroy(GameObject.Find("EventSystem"));
            string gameControllerObjName = "GameController";
            GameObject gameControllerObj = new GameObject(gameControllerObjName);
            _gameController = gameControllerObj.AddComponent<GameController>();
            _gameController.SetupGalaxyController(_galaxy);
        }

        private void Loaded() {
            //start game controller. pass galaxy through
            
            _gameController.StartGame();
            DestroyLoadingScreen();
            
        }

        public void Update() {
            if (_galaxyGenerationThread != null) {
                if (_galaxyGenerationThread.IsAlive) {
                    if (!_startedGalaxyGen) {
                        Debug.Log("Running Generation in separate thread..");
                        _startedGalaxyGen = !_startedGalaxyGen;
                    }
                }
                else if(!_finishedgalaxyGen){
                    Debug.Log("Finished Generating Galaxy");
                    _finishedgalaxyGen = true;
                }
            }

            if (_finishedgalaxyGen) {
                if (!_startedTextureGen) {
                    Debug.Log("Initialising Game Controller");
                    InitialiseGameController();
                    Debug.Log("Generating planet textures");
                    LoadingScreenController.SetLoadingText("Generating Planet Textures");
                    _startedTextureGen = true;
                    GenerateStartingSystemColours();
                    
                }
                else {
                    if (_textureGenerationThread != null) {
                        if (!_textureGenerationThread.IsAlive) {
                            GenerateStartingSystemTextures();
                            Loaded();
                        }
                    }
                }
                
            }
        }
    }
}