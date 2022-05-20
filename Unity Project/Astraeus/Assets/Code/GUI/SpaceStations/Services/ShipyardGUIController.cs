using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using Code._Utility;
using Code.Camera;
using Code.GUI.SpaceStations.Services.Outfitting;
using Code.GUI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class ShipyardGUIController : MonoBehaviour {
        private ShipyardService _shipyardService;
        private StationGUIController _stationGUIController;
        private GameObject _guiGameObject;
        GameController _gameController;
        private ShipyardCameraController _cameraController;
        private UnityEngine.Camera _camera;
        
        private float currentMsgTime = 0;
        private const float MsgTime = 3;

        private void Awake() {
            _gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }

        private void Update() {
            MsgCheck();
        }

        public void StartShipyardGUI(ShipyardService shipyardService, StationGUIController stationGUIController) {
            CameraUtility.SolidSkybox();
            _shipyardService = shipyardService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = Instantiate((GameObject)Resources.Load(_shipyardService.GUIPath));
            WakeUp wakeUp = _guiGameObject.AddComponent<WakeUp>();
            wakeUp.wakeFunction = WakeUpFunc;
            SetupCamera();
            SetupExitBtn();
            SetupShop();
            SetupOwnedShips();
        }

        private void WakeUpFunc() {
            SetupCamera();
            ClearScrollView(GameObjectHelper.FindChild(_guiGameObject, "BuyShipsContent"));
            ClearScrollView(GameObjectHelper.FindChild(_guiGameObject, "OwnedShipsContent"));
            SetCreditsValue();
            SetupShop();
            SetupOwnedShips();
        }

        private void ClearScrollView(GameObject contentView) {
            if (contentView.transform.childCount > 0) { //clear already assigned components
                for (int i = contentView.transform.childCount; i > 0; i--) {
                    Destroy(contentView.transform.GetChild(i - 1).gameObject);
                }
            }
        }


        private void SetupCamera() {
            if (_guiGameObject) {
                CameraUtility.ChangeCullingMask(1 << LayerMask.NameToLayer("GUIModel"));
                _camera = UnityEngine.Camera.main;
                if (_camera != null) {
                    _cameraController = _camera.gameObject.GetComponent<ShipyardCameraController>();
                    if (_cameraController == null) {
                        _cameraController = _camera.gameObject.AddComponent<ShipyardCameraController>();
                    }
                    _cameraController.TakeCameraControl();
                    _cameraController.SetCameraPos(GameObjectHelper.FindChild(_guiGameObject, "MainContainer").GetComponent<RectTransform>());
                }
            }
            
        }

        private void SetupExitBtn() {
            Button exitBtn = GameObject.Find("ExitBtn").GetComponent<Button>();
            exitBtn.onClick.AddListener(Exit);
        }

        private void Exit() {
            CameraUtility.NormalSkybox();
            CameraUtility.ChangeCullingMask(GameController.DefaultGameMask);
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

        private void SetupShipModelDisplay(Transform parent, Ship ship) {
            GameController.ShipCreator.shipObjectHandler.ManagedShip = ship;
            GameObject shipObject = GameController.ShipCreator.shipObjectHandler.CreateShip(parent);
            shipObject.transform.rotation = Quaternion.Euler(0, 0, -45);
            float scale = ship.ShipHull.ShipyardScale;
            shipObject.transform.localScale = new Vector3(scale, scale, scale);
            SetGameObjectLayer(shipObject, LayerMask.NameToLayer("GUIModel"));
            CenterObjectToGUI centerObjectToGUI = shipObject.AddComponent<CenterObjectToGUI>();
            centerObjectToGUI.SetGUIRect(parent.GetComponent<RectTransform>());
        }

        private void SetGameObjectLayer(GameObject obj, int layer) {
            obj.layer = layer;
            for (int i = 0; i < obj.transform.childCount; i++) {
                SetGameObjectLayer(obj.transform.GetChild(i).gameObject, layer);
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

            SetupShipModelDisplay(GameObjectHelper.FindChild(hullPanel, "ModelPanel").transform, new Ship(hull));

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
            GameObjectHelper.FindChild(shipPanel, "SellBtn").GetComponent<Button>().onClick.AddListener(delegate { SellShipBtnClick(ship, shipPanel); });
            GameObjectHelper.SetGUITextValue(shipPanel, "HealthValue", ship.ShipHull.CurrentHullStrength + "/" + ship.ShipHull.BaseHullStrength);
            GameObjectHelper.SetGUITextValue(shipPanel, "ShieldCapacityValue", ShipStats.GetShieldCapacity(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "ShieldRechargeValue", ShipStats.GetShieldRechargeRate(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "DPSValue", ShipStats.GetDPS(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "PowerCapacityValue", ShipStats.GetPowerCapacity(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "PowerRechargeValue", ShipStats.GetPowerRecharge(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "CargoValue", ShipStats.GetUsedCargoSpace(ship) + "/" + ShipStats.GetMaxCargoSpace(ship));
            GameObjectHelper.SetGUITextValue(shipPanel, "JumpRangeValue", ShipStats.GetJumpRange(ship).ToString("0.0"));
            GameObjectHelper.SetGUITextValue(shipPanel, "PriceValue", ShipStats.GetShipValue(ship).ToString());


            SetupShipModelDisplay(GameObjectHelper.FindChild(shipPanel, "ModelPanel").transform, ship);
            return shipPanel;
        }

        private void SetCreditsValue() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsValue", GameController.PlayerProfile._credits + "Cr");
        }

        private void SellShipBtnClick(Ship ship, GameObject shipPanel) {
            if (GameController.CurrentShip == ship) {
                SetFeedbackMsg("Cannot sell active ship", Color.red);
            } else if (ShipStats.GetUsedCargoSpace(ship)>0) {
                SetFeedbackMsg("Remove cargo from ship before sale", Color.red);
            }
            else {
                int salePrice = ShipStats.GetShipValue(ship);
                GameController.PlayerProfile.AddCredits(salePrice);
                SetCreditsValue();
                GameController.PlayerProfile.Ships.Remove(ship);
                SetFeedbackMsg("Ship sold", Color.green);
                Destroy(shipPanel);
            }
        }

        private void MsgCheck() {
            if (currentMsgTime > 0) {
                currentMsgTime -= Time.deltaTime;
                if (currentMsgTime <= 0) {
                    GameObjectHelper.FindChild(_guiGameObject, "FeedbackMsg").SetActive(false);
                    currentMsgTime = 0;
                }
            }
        }
        private void SetFeedbackMsg(string msg,Color colour) {
            GameObjectHelper.FindChild(_guiGameObject, "FeedbackMsg").SetActive(true);
            GameObjectHelper.SetGUITextValue(_guiGameObject, "FeedbackMsg", msg, colour);
            currentMsgTime = MsgTime;
        }

        private void OutfittingBtnClick(Ship ship) {
            CameraUtility.ChangeCullingMask(GameController.DefaultGameMask);
            OutfittingGUIController outfittingGUIController = gameObject.AddComponent<OutfittingGUIController>();
            OutfittingService outfittingService = (OutfittingService)GameController.GUIController.stationGUIController.FindStationService<OutfittingService>();
            outfittingGUIController.StartOutfitting(outfittingService, _guiGameObject, ship);
        }

        private void BuyBtnClick(Hull hull) {
            //check if there is enough money
            if (GameController.PlayerProfile.AddCredits(-hull.HullPrice)) {
                SetCreditsValue();
                Ship ship = GameController.ShipCreator.CreateShipFromHull(hull);
                GameController.PlayerProfile.Ships.Add(ship);
                AddOwnedShip(ship);
                SetFeedbackMsg("Ship purchased", Color.green);
            }
            else {
                SetFeedbackMsg("Insufficient funds", Color.red);
            }
        }

        private void SetActiveShipBtnClick(Ship ship) {
            GameController.CurrentShip = ship;
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