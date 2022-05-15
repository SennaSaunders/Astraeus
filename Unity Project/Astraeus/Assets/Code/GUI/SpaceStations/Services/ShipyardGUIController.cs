using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using Code._Utility;
using Code.GUI.SpaceStations.Services.Outfitting;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class ShipyardGUIController : MonoBehaviour {
        private ShipyardService _shipyardService;
        private StationGUIController _stationGUIController;
        private GameObject _guiGameObject;
        GameController _gameController;

        private void Awake() {
            _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }

        public void StartShipyardGUI(ShipyardService shipyardService, StationGUIController stationGUIController) {
            _shipyardService = shipyardService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = Instantiate((GameObject)Resources.Load(_shipyardService.GUIPath));
            SetupHomeBtn();
            SetupShop();
            SetupOwnedShips();
        }

        private void SetupHomeBtn() {
            Button homeBtn = GameObject.Find("HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(Exit);
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void SetupShop() {
            GameObject shipsContentView = GameObjectHelper.FindChild(_guiGameObject, "BuyShipsContent");
            foreach (Type shipHullType in _shipyardService.GetShipHulls()) {
                Hull hull = (Hull)Activator.CreateInstance(shipHullType);
                GameObject hullPanel = SetupBuyShipPanel(hull);
                hullPanel.transform.SetParent(shipsContentView.transform, false);
            }
        }

        private GameObject SetupBuyShipPanel(Hull hull) {
            //take hull - pull info
            List<(ShipComponentTier tier, int count)> thrusterTierCount = new List<(ShipComponentTier tier, int count)>();
            List<(ShipComponentTier tier, int count)> weaponTierCount = new List<(ShipComponentTier tier, int count)>();
            List<(ShipComponentTier tier, int count)> internalTierCount = new List<(ShipComponentTier tier, int count)>();

            foreach (var mainThrusterComponent in hull.MainThrusterComponents) {
                if (thrusterTierCount.Select(s => s.tier).Contains(mainThrusterComponent.maxSize)) {
                    int idx = thrusterTierCount.FindIndex(ts => ts.tier == mainThrusterComponent.maxSize);
                    var tierCount = thrusterTierCount[idx];
                    tierCount.count++;
                    thrusterTierCount[idx] = tierCount;
                }
                else {
                    thrusterTierCount.Add((mainThrusterComponent.maxSize, 1));
                }
            }

            foreach (var weaponComponent in hull.WeaponComponents) {
                if (weaponTierCount.Select(s => s.tier).Contains(weaponComponent.maxSize)) {
                    int idx = weaponTierCount.FindIndex(ws => ws.tier == weaponComponent.maxSize);
                    var tierCount = weaponTierCount[idx];
                    tierCount.count++;
                    weaponTierCount[idx] = tierCount;
                }
                else {
                    weaponTierCount.Add((weaponComponent.maxSize, 1));
                }
            }

            foreach (var internalComponent in hull.InternalComponents) {
                if (internalTierCount.Select(s => s.tier).Contains(internalComponent.maxSize)) {
                    int idx = internalTierCount.FindIndex(s => s.tier == internalComponent.maxSize);
                    var tierCount = internalTierCount[idx];
                    tierCount.count++;
                    internalTierCount[idx] = tierCount;
                }
                else {
                    internalTierCount.Add((internalComponent.maxSize, 1));
                }
            }

            GameObject hullPanel = Instantiate((GameObject)Resources.Load("GUIPrefabs/Station/Services/Shipyard/HullPanel"));
            GameObjectHelper.SetGUITextValue(hullPanel, "HullName", hull.HullName);
            GameObjectHelper.SetGUITextValue(hullPanel, "MassValue", hull.HullMass + "KG");
            GameObjectHelper.SetGUITextValue(hullPanel, "ThrusterValue", GetTierCountString(thrusterTierCount));
            GameObjectHelper.SetGUITextValue(hullPanel, "WeaponValue", GetTierCountString(weaponTierCount));
            GameObjectHelper.SetGUITextValue(hullPanel, "InternalValue", GetTierCountString(internalTierCount));
            GameObjectHelper.SetGUITextValue(hullPanel, "PriceValue", hull.HullPrice + " Credits");
            GameObjectHelper.FindChild(hullPanel, "BuyBtn").GetComponent<Button>().onClick.AddListener(delegate { BuyBtnClick(hull); });

            return hullPanel;
        }

        private void SetupOwnedShips() {
            foreach (Ship ship in GameController.PlayerProfile.Ships) {
                AddOwnedShip(ship);
            }
        }

        private void AddOwnedShip(Ship ship) {
            GameObject ownedShipsView = GameObjectHelper.FindChild(_guiGameObject, "OwnedShipsContent");
            GameObject shipPanel = SetupOwnedShipPanel(ship);
            shipPanel.transform.SetParent(ownedShipsView.transform, false);
        }

        private GameObject SetupOwnedShipPanel(Ship ship) {
            GameObject shipPanel = Instantiate((GameObject)Resources.Load("GUIPrefabs/Station/Services/Shipyard/ShipPanel"));
            GameObjectHelper.SetGUITextValue(shipPanel, "HullName", ship.ShipHull.HullName);
            GameObjectHelper.FindChild(shipPanel, "OutfittingBtn").GetComponent<Button>().onClick.AddListener(delegate { OutfittingBtnClick(ship); });
            GameObjectHelper.FindChild(shipPanel, "SetActiveBtn").GetComponent<Button>().onClick.AddListener(delegate { SetActiveShipBtnClick(ship); });
            return shipPanel;
        }

        private void OutfittingBtnClick(Ship ship) {
            OutfittingGUIController outfittingGUIController = gameObject.AddComponent<OutfittingGUIController>();
            OutfittingService outfittingService = (OutfittingService)GameController.GUIController.stationGUIController.FindStationService<OutfittingService>();
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            outfittingGUIController.StartOutfitting(outfittingService, _guiGameObject, gameController, ship);
        }

        private void BuyBtnClick(Hull hull) {
            //check if there is enough money
            if (GameController.PlayerProfile.ChangeCredits(-hull.HullPrice)) {
                Ship ship = GameController.ShipCreator.CreateShipFromHull(hull);
                GameController.PlayerProfile.Ships.Add(ship);
                AddOwnedShip(ship);
            }
            else {
                NotEnoughMoneyMsg();
            }
        }

        private void SetActiveShipBtnClick(Ship ship) {
            GameController.CurrentShip = ship;
            _gameController.RefreshPlayerShip();
        }

        private void NotEnoughMoneyMsg() {
        }

        private string GetTierCountString(List<(ShipComponentTier tier, int count)> tierCount) {
            string tierCountString = "";

            for (int i = 0; i < tierCount.Count; i++) {
                tierCountString += tierCount[i].count + "x " + tierCount[i].tier;
                if (i != tierCount.Count - 1) {
                    tierCountString += "    ";
                }
            }

            return tierCountString;
        }
    }
}