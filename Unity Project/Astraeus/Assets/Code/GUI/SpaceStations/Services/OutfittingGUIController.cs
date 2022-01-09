using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._Ships;
using Code._Ships.Power_Plants;
using Code._Ships.Storage;
using Code._Ships.Thrusters;
using Code._Ships.Weapons;
using TMPro;
using UnityEngine;

namespace Code.GUI.SpaceStations.Services {
    public class OutfittingGUIController : MonoBehaviour {
        private Ship _ship;
        private OutfittingService _outfittingService;
        private GameObject _guiGameObject;

        private string _cardPath = "GUIPrefabs/SpaceStation/Services/Outfitting/";
        private string _cargoBayCardPath = "CargoBayCard";
        private string _weaponCardPath = "WeaponCard";
        private string _thrusterCardPath = "ThrusterCard";
        private string _powerPlantCardPath = "PowerPlantCard";

        private void Start() {
            SetGUIGameObject();
            SetupOutfittingService();
            DisplayWeaponComponents();
            DisplayThrusterComponents();
            DisplayInternalComponents();
            DisplayShip();
        }

        private void DisplayShip() {
            _ship = _outfittingService.Ships[1];
            GameObject shipObject = Instantiate(_ship.ShipHull.hullObject);
            shipObject.transform.position = _ship.ShipHull.outfittingPosition;
            shipObject.transform.rotation = _ship.ShipHull.outfittingRotation;
        }

        private void SetupOutfittingService() {
            _outfittingService = gameObject.AddComponent<OutfittingService>();
            _outfittingService.SetAllAvailableComponents();
            _outfittingService.AddAllShips();
        }
        
        private void SetGUIGameObject() {
            _guiGameObject = GameObject.Find("OutfittingGUI");
        }

        private GameObject GetScrollViewContentContainer() {
            GameObject contentContainer = _guiGameObject.transform.Find("Canvas/MainPanel/ComponentsPanel/ComponentsInteractablePanel/ComponentScrollView/Viewport/ComponentContent").gameObject;
            return contentContainer;
        }

        private void DisplayThrusterComponents() {
            List<Thruster> thrusters = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(Thruster))).Cast<Thruster>().ToList();

            foreach (Thruster thruster in thrusters) {
                DisplayThruster(thruster);
            }
        }

        private void DisplayThruster(Thruster thruster) {
            GameObject thrusterCard = (GameObject)Resources.Load(_cardPath + _thrusterCardPath);
            thrusterCard = Instantiate(thrusterCard, GetScrollViewContentContainer().transform);

            Transform cardComponents = thrusterCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = thruster.Name + " - " + thruster.ComponentSize;
            
            TextMeshProUGUI force = cardComponents.Find("Force").GetComponent<TextMeshProUGUI>();
            force.text = thruster.Force + " N";
            
            TextMeshProUGUI powerDraw = cardComponents.Find("PowerDraw").GetComponent<TextMeshProUGUI>();
            powerDraw.text = thruster.PowerDraw + " GW/s";
            
            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (thruster.ComponentMass / 1000) + " T";
        }

        private void DisplayWeaponComponents() {
            List<Weapon> weapons = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(Weapon))).Cast<Weapon>().ToList();

            foreach (Weapon weapon in weapons) {
                DisplayWeapon(weapon);
            }
        }

        private void DisplayWeapon(Weapon weapon) {
            GameObject weaponCard = (GameObject)Resources.Load(_cardPath + _weaponCardPath);
            weaponCard = Instantiate(weaponCard, GetScrollViewContentContainer().transform);

            Transform cardComponents = weaponCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = weapon.Name + " - " + weapon.ComponentSize;
            
            TextMeshProUGUI damage = cardComponents.Find("Damage").GetComponent<TextMeshProUGUI>();
            damage.text = weapon.Damage + " DMG";
            
            TextMeshProUGUI rateOfFire = cardComponents.Find("RoF").GetComponent<TextMeshProUGUI>();
            rateOfFire.text = weapon.FireRate * 60 + " RPM";
            
            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (weapon.ComponentMass / 1000) + " T";
        }

        private void DisplayInternalComponents() {
            List<PowerPlant> powerPlants = _outfittingService.AvailableComponents.Where(c => c.GetType().IsSubclassOf(typeof(PowerPlant))).Cast<PowerPlant>().ToList();

            foreach (PowerPlant powerPlant in powerPlants) {
                DisplayPowerPlant(powerPlant);
            }

            List<CargoBay> cargoBays = _outfittingService.AvailableComponents.Where(c => c.GetType() == typeof(CargoBay)).Cast<CargoBay>().ToList();

            foreach (CargoBay cargoBay in cargoBays) {
                DisplayCargoBay(cargoBay);
            }
        }
        
        private void DisplayPowerPlant(PowerPlant powerPlant) {
            GameObject powerPlantCard = (GameObject)Resources.Load(_cardPath + _powerPlantCardPath);
            powerPlantCard = Instantiate(powerPlantCard, GetScrollViewContentContainer().transform);

            Transform cardComponents = powerPlantCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = powerPlant.Name + " - " + powerPlant.ComponentSize;
            
            TextMeshProUGUI capacity = cardComponents.Find("Capacity").GetComponent<TextMeshProUGUI>();
            capacity.text = powerPlant.EnergyCapacity + " GW";
            
            TextMeshProUGUI rechargeRate = cardComponents.Find("Recharge").GetComponent<TextMeshProUGUI>();
            rechargeRate.text = powerPlant.RechargeRate * 60 + " GW/s";
            
            TextMeshProUGUI mass = cardComponents.Find("Mass").GetComponent<TextMeshProUGUI>();
            mass.text = (powerPlant.ComponentMass / 1000) + " T";
        }

        private void DisplayCargoBay(CargoBay cargoBay) {
            GameObject cargoBayCard = (GameObject)Resources.Load(_cardPath + _cargoBayCardPath);
            cargoBayCard = Instantiate(cargoBayCard, GetScrollViewContentContainer().transform);

            Transform cardComponents = cargoBayCard.transform.Find("Details");

            TextMeshProUGUI title = cardComponents.Find("ComponentName").GetComponent<TextMeshProUGUI>();
            title.text = cargoBay.Name + " - " + cargoBay.ComponentSize;
            
            TextMeshProUGUI capacity = cardComponents.Find("Capacity").GetComponent<TextMeshProUGUI>();
            capacity.text = cargoBay.CargoVolume + " Units";
        }
    }
}