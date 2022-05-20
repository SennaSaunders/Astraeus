using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Galaxy._SolarSystem;
using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Utility;
using UnityEngine;

namespace Code._Galaxy {
    public class GalaxyController : MonoBehaviour {
        public const int ZOffset = 2500;
        public Galaxy _galaxy { get; private set; }
        public GameObject _galaxyHolder;
        public SolarSystemController activeSystemController;
        private List<SolarSystemController> _solarSystemControllers = new List<SolarSystemController>();

        public void SetGalaxy(Galaxy galaxy) {
            _galaxy = galaxy;
        }

        public void SetupGalaxyHolder() {
            string holderName = "Galaxy";
            _galaxyHolder = GameObject.Find(holderName);
            if (_galaxyHolder) {
                DestroyImmediate(_galaxyHolder);
            }

            _galaxyHolder = new GameObject(holderName);
            _galaxyHolder.transform.position = new Vector3(0, 0, ZOffset);
        }

        private void DisplaySolarSystemPrimary(SolarSystem solarSystem, int num) {
            CelestialBody primary = (CelestialBody)solarSystem.Bodies[0];
            GameObject primaryObject = Instantiate(primary.GetMapObject(), _galaxyHolder.transform, true);
            float scale = solarSystem.Bodies[0].Tier.MapScale();
            primaryObject.transform.localScale = new Vector3(scale, scale, scale);
            primaryObject.layer = LayerMask.NameToLayer("GalaxyMap");
            primaryObject.transform.localPosition = new Vector3(solarSystem.Coordinate.x, solarSystem.Coordinate.y);
            primaryObject.name = "System: " + solarSystem.SystemName;
            SolarSystemController controller = primaryObject.AddComponent<SolarSystemController>();
            controller.AssignSystem(solarSystem);
            _solarSystemControllers.Add(controller);
            GameObject systemName = Instantiate((GameObject)Resources.Load("GUIPrefabs/Map/SystemName"), primaryObject.transform, true);
            systemName.layer = LayerMask.NameToLayer("GalaxyMap");
            if (solarSystem.OwnerFaction != null) {
                GameObjectHelper.SetGUITextValue(systemName, "SystemNameValue", controller.SolarSystem.SystemName, solarSystem.OwnerFaction.factionType.MapColor());
            }
            else {
                GameObjectHelper.SetGUITextValue(systemName, "SystemNameValue", controller.SolarSystem.SystemName);
            }

            systemName.transform.localPosition = new Vector3(0, -1, 0);
        }

        private void DisplayGalaxy(Galaxy galaxy) {
            for (int i = 0; i < galaxy.Systems.Count; i++) {
                DisplaySolarSystemPrimary(galaxy.Systems[i], i + 1);
            }
        }

        public SolarSystemController GetSolarSystemController(SolarSystem solarSystem) {
            return _solarSystemControllers.Find(ssc => ssc.SolarSystem == solarSystem);
        }

        public void DisplayGalaxy() {
            SetupGalaxyHolder();
            DisplayGalaxy(_galaxy);
        }

        public List<Faction> GetFactions() {
            return _galaxy.Factions;
        }
    }
}