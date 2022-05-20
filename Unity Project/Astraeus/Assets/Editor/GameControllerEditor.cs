using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._Factions;
using Code._GameControllers;
using Code._Ships;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents;
using UnityEditor;
using UnityEngine;

namespace Editor {
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : UnityEditor.Editor {
        private static GameController gameController;
        
        public static Vector2 spawnLocation, destination, velocity;
        public List<Faction> spawnableFactions;
        public List<ShipCreator.ShipClass> ShipClasses; 
        private static int selectedFactionIndex, selectedShipClassIndex, selectedTierIndex, selectedNPCShip;
        private float loadoutEfficiency = 0.5f;
        public static GameObject destinationObject;

        public override void OnInspectorGUI() {
            gameController = (GameController)target;

            if (destinationObject == null) {
                destinationObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                destinationObject.transform.localScale = new Vector3(5, 5, 5);
                destinationObject.name = "Destination Marker";
                destinationObject.GetComponent<Renderer>().material = null;
            }

            
            
            //faction selection
            List<Faction> potentialFactions = gameController.GetFactions();
            spawnableFactions = new List<Faction>();
            spawnableFactions.Add(gameController.GetCurrentSolarSystem().OwnerFaction);

            var factionTypeValues = Enum.GetValues(typeof(Faction.FactionType));
            foreach (var typeValue in factionTypeValues) {
                if (!spawnableFactions.Select(f => f.factionType).Contains((Faction.FactionType)typeValue)) {
                    Faction newFactionToPick = potentialFactions.Find(f => f.factionType == (Faction.FactionType)typeValue);
                    spawnableFactions.Add(newFactionToPick);
                }
            }

            List<string> factionNames = new List<string>();
            foreach (Faction faction in spawnableFactions) {
                factionNames.Add(faction.factionType.ToString());
            }

            selectedFactionIndex = EditorGUILayout.Popup(selectedFactionIndex, factionNames.ToArray());
            
            //ship class selection
            string[] shipClassNames = Enum.GetNames(typeof(ShipCreator.ShipClass));
            selectedShipClassIndex = EditorGUILayout.Popup(selectedShipClassIndex, shipClassNames);
            
            //max component tier
            string[] tierNames = Enum.GetNames(typeof(ShipComponentTier));
            selectedTierIndex = EditorGUILayout.Popup(selectedTierIndex, tierNames);
            
            //loadout efficiency
            EditorGUILayout.BeginVertical();
            loadoutEfficiency = EditorGUILayout.Slider(loadoutEfficiency, 0, 1);
            EditorGUILayout.EndVertical();

            
            //spawn location
            EditorGUILayout.BeginVertical();
            spawnLocation = EditorGUILayout.Vector2Field("Spawn Location", spawnLocation);
            EditorGUILayout.EndVertical();
            
            //spawn ship button
            if (GUILayout.Button("Spawn Ship")) {
                if (Application.isPlaying) {
                    GameObject.FindWithTag("GameController").GetComponent<GameController>().CreateNPC(spawnableFactions[selectedFactionIndex], (ShipCreator.ShipClass)selectedShipClassIndex, (ShipComponentTier)selectedTierIndex, loadoutEfficiency, spawnLocation);
                }
            }

            if (GUILayout.Button("Clear NPCs")) {
                foreach (Ship npcShip in gameController.npcShips) {
                    Destroy(npcShip.ShipObject);
                }
            }

            //select npc ship
            List<string> shipNameList = new List<string>();
            int shipCount = 0;
            foreach (Ship gameControllerNpcShip in gameController.npcShips) {
                shipNameList.Add(shipCount + " - " + gameControllerNpcShip.ShipHull.HullName);
                shipCount++;
            }

            selectedNPCShip = EditorGUILayout.Popup(selectedNPCShip, shipNameList.ToArray());
            
            //set destination
            EditorGUILayout.BeginVertical();
            destination = EditorGUILayout.Vector2Field("Destination", destination);
            EditorGUILayout.EndVertical();
            
            
            if (GUILayout.Button("Set destination")) {
                if (Application.isPlaying) {
                    destinationObject.transform.position = destination;
                }
            }

            EditorGUILayout.BeginVertical();
            velocity = EditorGUILayout.Vector2Field("New Velocity", velocity);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Set Velocity")) {
                if (Application.isPlaying) {
                    gameController.npcShips[selectedNPCShip].ShipObject.GetComponent<ShipController>().ThrusterController.Velocity = velocity;
                }
            }
            
            DrawDefaultInspector();
        }
    }
}