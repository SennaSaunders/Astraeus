using System;
using System.Collections.Generic;
using System.Linq;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents.ExternalComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Utility;
using Code.GUI.ShipGUI;
using Code.GUI.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class ShipColourGUIController : MonoBehaviour {
        private GameObject _outfittingGUIGameObject, _shipColourGUI, _displayedShip, _shipPanel;
        private ShipObjectHandler _shipObjectHandler;
        private List<ColourPicker> _colourPickers = new List<ColourPicker>();

        private (List<(List<GameObject>, int channelIdx)> meshChannels, Hull hull) _hullMeshChannelMap;
        private List<(List<(List<GameObject>, int channelIdx)> meshChannels, List<MainThruster> thrusters)> _mainThrustersMeshChannelMap;
        private List<(List<(List<GameObject>, int channelIdx)> meshChannels, List<Weapon> weapons)> _weaponsMeshChannelMap;

        private List<TMP_Dropdown.OptionData> _groupOptions, _thrusterOptions, _weaponOptions, _channelOptions;
        private TMP_Dropdown _groupDropdown, _typeDropdown, _channelDropdown;
        private TMP_Dropdown.OptionData hullGroup = new TMP_Dropdown.OptionData("Hull");
        private TMP_Dropdown.OptionData thrusterGroup = new TMP_Dropdown.OptionData("Thrusters");
        private TMP_Dropdown.OptionData weaponGroup = new TMP_Dropdown.OptionData("Weapons");
        private TMP_Dropdown.OptionData _selectedGroup, _selectedType, _selectedChannel;

        public void SetupGUI(ShipObjectHandler shipObjectHandler, GameObject outfittingGUIGameObject, GameObject displayedShip) {
            _shipObjectHandler = shipObjectHandler;
            _outfittingGUIGameObject = outfittingGUIGameObject;
            outfittingGUIGameObject.SetActive(false);
            _shipColourGUI = Instantiate((GameObject)Resources.Load("GUIPrefabs/Station/Services/Outfitting/ShipColourGUI"));
            SetupColourPickers();
            SetupButtons();
            GroupAllComponents();
            SetupDropdowns();
            _shipPanel = GameObjectHelper.FindChild(_shipColourGUI, "ShipPanel");
            DisplayShip(displayedShip);
        }

        private void SetupColourPickers() {
            ColourPicker colourPicker = GameObjectHelper.FindChild(_shipColourGUI, "ColourPanel1").GetComponentInChildren<ColourPicker>();
            colourPicker.Setup(this, 1, true);
            _colourPickers.Add(colourPicker);
            colourPicker = GameObjectHelper.FindChild(_shipColourGUI, "ColourPanel2").GetComponentInChildren<ColourPicker>();
            colourPicker.Setup(this, 0, false);
            _colourPickers.Add(colourPicker);
            colourPicker = GameObjectHelper.FindChild(_shipColourGUI, "ColourPanel3").GetComponentInChildren<ColourPicker>();
            colourPicker.Setup(this, 0.5f, true);
            _colourPickers.Add(colourPicker);
        }

        private void DisplayShip(GameObject ship) {
            _displayedShip = ship;
            _displayedShip.transform.SetParent(_shipPanel.transform, false);
            _displayedShip.transform.position = _shipObjectHandler.ManagedShip.ShipHull.OutfittingPosition;
            AddDraggableToShip();
        }

        private void AddDraggableToShip() {
            ShipRotatable shipRotatable = _shipPanel.GetComponent<ShipRotatable>();
            if (shipRotatable == null) {
                shipRotatable = _shipPanel.AddComponent<ShipRotatable>();
            }

            shipRotatable.RotatableObject = _shipObjectHandler.ManagedShip.ShipObject;
        }


        private void SetupButtons() {
            SetupExitBtn();
        }

        private void SetupExitBtn() {
            Button exitBtn = GameObjectHelper.FindChild(_shipColourGUI, "ExitBtn").GetComponent<Button>();
            exitBtn.onClick.AddListener(Exit);
        }

        private void Exit() {
            _outfittingGUIGameObject.SetActive(true);
            _displayedShip.transform.SetParent(GameObjectHelper.FindChild(_outfittingGUIGameObject, "ShipPanel").transform);
            Destroy(_shipColourGUI);
            Destroy(this);
        }

        private void SetupDropdowns() {
            _groupDropdown = GameObjectHelper.FindChild(_shipColourGUI, "GroupTypeSelect").GetComponent<TMP_Dropdown>();
            _groupDropdown.ClearOptions();
            _groupOptions = new List<TMP_Dropdown.OptionData>();
            _groupOptions.Add(hullGroup);
            _typeDropdown = GameObjectHelper.FindChild(_shipColourGUI, "SubTypeSelect").GetComponent<TMP_Dropdown>();
            _channelDropdown = GameObjectHelper.FindChild(_shipColourGUI, "ChannelSelect").GetComponent<TMP_Dropdown>();

            //check if there are thrusters/weapons
            if (_mainThrustersMeshChannelMap != null) {
                if (_mainThrustersMeshChannelMap.Count > 0) {
                    _groupOptions.Add(thrusterGroup);
                }
            }

            if (_weaponsMeshChannelMap != null) {
                if (_weaponsMeshChannelMap.Count > 0) {
                    _groupOptions.Add(weaponGroup);
                }
            }

            _groupDropdown.options = _groupOptions;
            _groupDropdown.onValueChanged.AddListener(CheckGroupOptions);
            SelectedHullGroup();
        }

        private void CheckGroupOptions(int i) {
            _selectedGroup = _groupOptions[i];
            
            if (_selectedGroup == hullGroup) {
                SelectedHullGroup();
            }
            else if (_selectedGroup == thrusterGroup) {
                SelectedThrustersGroup();
            }
            else {
                SelectedWeaponsGroup();
            }
        }

        private void SelectedThrustersGroup() {
            Debug.Log("Selected thrusters");
            _thrusterOptions = new List<TMP_Dropdown.OptionData>();

            foreach (List<MainThruster> thrusters in _mainThrustersMeshChannelMap.Select(mt => mt.thrusters)) {
                _thrusterOptions.Add(new TMP_Dropdown.OptionData(thrusters[0].ComponentName));
            }

            _typeDropdown.gameObject.SetActive(true);
            _typeDropdown.ClearOptions();
            _typeDropdown.options = _thrusterOptions;
            SetupThrusterChannels(0);
            _typeDropdown.onValueChanged.RemoveAllListeners();
            _typeDropdown.onValueChanged.AddListener(SetupThrusterChannels);
        }

        private void SelectedWeaponsGroup() {
            Debug.Log("Selected weapons");
            _weaponOptions = new List<TMP_Dropdown.OptionData>();
            foreach (List<Weapon> weapons in _weaponsMeshChannelMap.Select(w => w.weapons)) {
                _weaponOptions.Add(new TMP_Dropdown.OptionData(weapons[0].ComponentName));
            }

            _typeDropdown.gameObject.SetActive(true);
            _typeDropdown.ClearOptions();
            _typeDropdown.options = _weaponOptions;
            SetupWeaponChannels(0);
            _typeDropdown.onValueChanged.RemoveAllListeners();
            _typeDropdown.onValueChanged.AddListener(SetupWeaponChannels);
        }

        private void SelectedHullGroup() {
            Debug.Log("Selected hull");
            _selectedGroup = hullGroup;
            _typeDropdown.gameObject.SetActive(false);
            SetupHullChannels();
        }

        private void SetupThrusterChannels(int selectedThruster) {
            _selectedType = _thrusterOptions[selectedThruster];
            SetChannelOptions(_mainThrustersMeshChannelMap[selectedThruster].meshChannels);
        }

        private void SetupWeaponChannels(int selectedWeapon) {
            _selectedType = _weaponOptions[selectedWeapon];
            SetChannelOptions(_weaponsMeshChannelMap[selectedWeapon].meshChannels);
        }

        private void SetupHullChannels() {
            SetChannelOptions(_hullMeshChannelMap.meshChannels);
        }

        private void SetChannelOptions(List<(List<GameObject>, int channelIdx)> meshChannels) {
            _channelOptions = new List<TMP_Dropdown.OptionData>();
            foreach ((List<GameObject>, int channelIdx) meshChannel in meshChannels) {
                _channelOptions.Add(new TMP_Dropdown.OptionData("Channel " + (meshChannel.channelIdx + 1)));
            }

            _channelDropdown.ClearOptions();
            _channelDropdown.options = _channelOptions;
            _channelDropdown.onValueChanged.RemoveAllListeners();
            _channelDropdown.onValueChanged.AddListener(SelectChannel);
            SelectChannel(0);
        }

        private void SelectChannel(int i) {
            _selectedChannel = _channelOptions[i];
        }

        public void SetColour(Color colour) {
            int channelIndex = _channelOptions.IndexOf(_selectedChannel);
            if (_selectedGroup == hullGroup) {
                (List<string> objectName, Color colour) colourChannel = _shipObjectHandler.ManagedShip.ShipHull.ColourChannelObjectMap[channelIndex];
                colourChannel.colour = colour;
                _shipObjectHandler.ManagedShip.ShipHull.ColourChannelObjectMap[channelIndex] = colourChannel;
                _shipObjectHandler.SetMappedMaterials(_shipObjectHandler.ManagedShip.ShipHull.MeshObjects, _shipObjectHandler.ManagedShip.ShipHull.ColourChannelObjectMap);
            }
            else {
                List<(GameObject mesh, int channelIdx)> meshObjects = new List<(GameObject mesh, int channelIdx)>();
                List<(List<string> objectName, Color colour)> colourChannelObjectMap = new List<(List<string> objectName, Color colour)>();
                if (_selectedGroup == thrusterGroup) {
                    int typeIndex = _thrusterOptions.IndexOf(_selectedType);
                    var thrusters = _mainThrustersMeshChannelMap[typeIndex].thrusters;
                    var shipMainThrusters = _shipObjectHandler.ManagedShip.ShipHull.MainThrusterComponents;
                    
                    foreach (MainThruster mainThruster in thrusters) {
                        int thrusterIdx = shipMainThrusters.Select(mt => mt.concreteComponent).ToList().IndexOf(mainThruster);
                        if (thrusterIdx >= 0) {
                            var colourChannel = shipMainThrusters[thrusterIdx].concreteComponent.ColourChannelObjectMap[channelIndex];
                            colourChannel.colour = colour;
                            shipMainThrusters[thrusterIdx].concreteComponent.ColourChannelObjectMap[channelIndex] = colourChannel;
                            meshObjects.AddRange(shipMainThrusters[thrusterIdx].concreteComponent.MeshObjects);
                            colourChannelObjectMap.AddRange(shipMainThrusters[thrusterIdx].concreteComponent.ColourChannelObjectMap);
                        }
                    }
                }
                else {
                    int typeIndex = _weaponOptions.IndexOf(_selectedType);
                    var weapons = _weaponsMeshChannelMap[typeIndex].weapons;
                    var shipWeapons = _shipObjectHandler.ManagedShip.ShipHull.WeaponComponents;

                    foreach (var weapon in weapons) {
                        int weaponIdx = shipWeapons.Select(w => w.concreteComponent).ToList().IndexOf(weapon);
                        if (weaponIdx >= 0) {
                            var colourChannel = shipWeapons[weaponIdx].concreteComponent.ColourChannelObjectMap[channelIndex];
                            colourChannel.colour = colour;
                            shipWeapons[weaponIdx].concreteComponent.ColourChannelObjectMap[channelIndex] = colourChannel;
                            meshObjects.AddRange(shipWeapons[weaponIdx].concreteComponent.MeshObjects);
                            colourChannelObjectMap.AddRange(shipWeapons[weaponIdx].concreteComponent.ColourChannelObjectMap);
                        }
                    }
                }

                _shipObjectHandler.SetMappedMaterials(meshObjects, colourChannelObjectMap);
            }
        }

        private void GroupAllComponents() {
            Hull hull = GameController.CurrentShip.ShipHull;
            _hullMeshChannelMap = (GroupHullChannels(hull), GameController.CurrentShip.ShipHull);
            _weaponsMeshChannelMap = GroupExternalChannels(GameController.CurrentShip.ShipHull.WeaponComponents.Select(wc => wc.concreteComponent).ToList());
            _mainThrustersMeshChannelMap = GroupExternalChannels(GameController.CurrentShip.ShipHull.MainThrusterComponents.Select(c => c.concreteComponent).ToList());
        }

        private List<(List<GameObject>, int channelIdx)> GroupMeshChannels(List<(GameObject mesh, int channelIdx)> meshObjects, List<Color> channelColours) {
            string groupedMeshes = "";
            List<(List<GameObject>, int channelIdx)> meshChannels = new List<(List<GameObject>, int channelIdx)>();
            for (int i = 0; i < channelColours.Count; i++) {
                List<GameObject> meshes = meshObjects.Where(mo => mo.channelIdx == i).Select(mo => mo.mesh).ToList();
                Color colour = channelColours[i];
                groupedMeshes += "Channel: " + i + " Colour: " + colour + " " + "Meshes:";
                foreach (var mesh in meshes) {
                    groupedMeshes += " " + mesh.name;
                }

                groupedMeshes += "\n";
                meshChannels.Add((meshes, i));
            }

            Debug.Log(groupedMeshes);
            return meshChannels;
        }

        private List<(List<GameObject>, int channelIdx)> GroupHullChannels(Hull hull) {
            List<(GameObject mesh, int channelIdx)> meshObjects = hull.MeshObjects;
            List<Color> channelColours = hull.ColourChannelObjectMap.Select(cc => cc.colour).ToList();
            return GroupMeshChannels(meshObjects, channelColours);
        }


        private List<(List<(List<GameObject>, int channelIdx)> meshChannels, List<T> externalComponents)> GroupExternalChannels<T>(List<T> components) where T : ExternalComponent {
            List<Type> externalTypes = new List<Type>();
            foreach (T component in components) {
                if (component != null) {
                    if (!externalTypes.Contains(component.GetType())) {
                        externalTypes.Add(component.GetType());
                    }
                }
            }

            List<(List<(List<GameObject>, int channelIdx)> meshChannels, List<T> externalComponents)> meshChannelMap = new List<(List<(List<GameObject>, int channelIdx)> meshChannels, List<T> externalComponents)>();
            foreach (Type type in externalTypes) {
                var externalOfSameType = components.Where(c => c !=null && c.GetType() == type).ToList();
                List<(GameObject mesh, int channelIdx)> meshObjects = new List<(GameObject mesh, int channelIdx)>();
                List<Color> channelColours = externalOfSameType[0].ColourChannelObjectMap.Select(cc => cc.colour).ToList();
                //get meshes of same type components
                foreach (T component in externalOfSameType) {
                    meshObjects.AddRange(component.MeshObjects);
                }

                Debug.Log("Type: " + type);

                List<(List<GameObject>, int channelIdx)> meshChannels = GroupMeshChannels(meshObjects, channelColours);
                meshChannelMap.Add((meshChannels, externalOfSameType));
            }

            return meshChannelMap;
        }
    }
}