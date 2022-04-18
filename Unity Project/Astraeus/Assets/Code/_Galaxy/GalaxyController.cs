using System.Collections.Generic;
using System.Threading;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._GameControllers;
using Code._Utility;
using UnityEngine;

namespace Code._Galaxy {
    public class GalaxyController : MonoBehaviour {
        public const int ZOffset = 2500;
        private Galaxy _galaxy;
        public GameObject _galaxyHolder;
        public SolarSystemController activeSystemController;
        private List<SolarSystemController> _solarSystemControllers = new List<SolarSystemController>();
        
        
        public Thread GenerateSolarSystemPlanetColours(SolarSystem solarSystem) {
            Thread generationThread = new Thread(() => {
                solarSystem.GenerateSolarSystemPlanetColours();
            });
            generationThread.Start();
            return generationThread;
        }

        public void GenerateSolarSystemTextures(SolarSystem solarSystem) {
            solarSystem.GenerateSolarSystemTextures();
        }
        
        public void SetGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
        }

        public void SetupGalaxyHolder() {
            string holderName = "Galaxy";
            _galaxyHolder = GameObject.Find(holderName);
            if (Application.isEditor) {
                DestroyImmediate(_galaxyHolder);
            }
            else {
                Destroy(_galaxyHolder);
            }
            _galaxyHolder = new GameObject(holderName);
            _galaxyHolder.transform.position = new Vector3(0, 0, ZOffset);
        }
        
        private void DisplaySolarSystemPrimary(SolarSystem solarSystem, int num) {
            CelestialBody primary = (CelestialBody)solarSystem.Bodies[0];
            GameObject primaryObject = primary.GetMapObject();  //change out for Instantiate() when prefabs are made
            primaryObject.transform.SetParent(_galaxyHolder.transform);
            primaryObject.transform.localPosition = new Vector3(solarSystem.Coordinate.x, solarSystem.Coordinate.y);
            primaryObject.name = "System: " + num + " Tier: " + primary.Tier;
            SolarSystemController controller = primaryObject.AddComponent<SolarSystemController>();
            controller.AssignSystem(solarSystem);
            _solarSystemControllers.Add(controller);
            GameObject systemName = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Map/SystemName"));
            if (solarSystem.OwnerFaction != null) {
                GameObjectHelper.SetGUITextValue(systemName, "SystemNameValue", controller._solarSystem.SystemName, new Color(53 / 255f, 157 / 255f, 255 / 255f));
            }
            else {
                GameObjectHelper.SetGUITextValue(systemName, "SystemNameValue", controller._solarSystem.SystemName);
            }
            
            
            systemName.transform.SetParent(primaryObject.transform);
            systemName.transform.localPosition = new Vector3(0, -1, -1);
        }

        public void DisplayGalaxy(Galaxy galaxy) {
            for (int i = 0; i < galaxy.Systems.Count; i++) {   //can potentially be changed to only display some systems later e.g. hidden systems/maybe culling?
                DisplaySolarSystemPrimary(galaxy.Systems[i], i+1);
            }
        }

        public SolarSystemController GetSolarSystemController(SolarSystem solarSystem) {
            return _solarSystemControllers.Find(ssc => ssc._solarSystem == solarSystem);
        }

        public void DisplayGalaxy() {
            SetupGalaxyHolder();
            DisplayGalaxy(_galaxy);
        }

        public List<Faction> GetFactions() {
            return _galaxy.Factions;
        }
        
        //also should contain functions that advance and control the galaxy's state e.g. public void ChangeGalaxyState(){}
    }
}