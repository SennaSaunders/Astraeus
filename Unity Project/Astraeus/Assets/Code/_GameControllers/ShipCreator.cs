using System;
using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._Factions;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters.Types;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using Code._Ships.ShipComponents.InternalComponents.Power_Plants;
using Code._Ships.ShipComponents.InternalComponents.Shields;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using UnityEngine;
using Random = System.Random;

namespace Code._GameControllers {
    public class ShipCreator : MonoBehaviour {
        public ShipObjectHandler shipObjectHandler;

        private void Awake() {
            shipObjectHandler = gameObject.AddComponent<ShipObjectHandler>();
        }

        public enum ShipClass {
            Transport,
            Fighter
        }

        public Ship CreateDefaultShip(GameObject objectContainer) {
            Ship ship = objectContainer.AddComponent<Ship>();
            ship.ShipHull = new SmallFighterHull();
            shipObjectHandler.ManagedShip = ship;
            shipObjectHandler.CreateShip(objectContainer.transform);
            //manoeuvring thrusters
            shipObjectHandler.SetManoeuvringThrusterComponent(new ManoeuvringThruster(ShipComponentTier.T1));
            //main thrusters
            for (int i = 0; i< ship.ShipHull.MainThrusterComponents.Count; i++) {//DO NOT convert to foreach - causes enumerator error as loop is changing the list
                var thrusterComponent = ship.ShipHull.MainThrusterComponents[i]; 
                shipObjectHandler.SetMainThrusterComponent(thrusterComponent.parentTransformName, new PrimitiveThruster(ShipComponentTier.T1));
            }
            
            SetupBasicLoadout(shipObjectHandler.ManagedShip);
            return shipObjectHandler.ManagedShip;
        }

        private void SetupBasicLoadout(Ship ship) {
            //shield
            int slotIndex = GetBestEmptyInternalSlotIndex(ship);
            if (slotIndex >= 0) {
                var slot = ship.ShipHull.InternalComponents[slotIndex];
                shipObjectHandler.SetInternalComponent(slot.parentTransformName, new ShieldBalanced(slot.maxSize));
            }
            
            //power plant
            slotIndex = GetBestEmptyInternalSlotIndex(ship);
            if (slotIndex >= 0) {
                var slot = ship.ShipHull.InternalComponents[slotIndex];
                shipObjectHandler.SetInternalComponent(slot.parentTransformName, new PowerPlantBalanced(slot.maxSize));
            }
            //cargo
            do {
                slotIndex = GetBestEmptyInternalSlotIndex(ship);
                if (slotIndex >= 0) {
                    var slot = ship.ShipHull.InternalComponents[slotIndex]; 
                    shipObjectHandler.SetInternalComponent(slot.parentTransformName, new CargoBay(slot.maxSize));
                }
            } while (slotIndex >= 0);
        }

        public int GetBestEmptyInternalSlotIndex(Ship ship) {
            int bestIndex = -1;//if -1 is returned then there will be no empty slot
            ShipComponentTier bestTier = ShipComponentTier.T1;//smallest slot size by default
            for (int i = 0; i < ship.ShipHull.InternalComponents.Count; i++) {
                var currentSlot = ship.ShipHull.InternalComponents[i];
                if (currentSlot.concreteComponent == null) {//if slot is empty
                    if (bestIndex < 0) {//if slot hasn't been picked
                        bestIndex = i;
                        bestTier = currentSlot.maxSize;
                    }
                    else {
                        if (bestTier < currentSlot.maxSize) {//if slot has a higher tier than current best
                            bestIndex = i;
                            bestTier = currentSlot.maxSize;
                        }
                    }
                    
                }
            }
            return bestIndex;
        }

        public Ship CreateFactionShip(ShipClass shipClass, ShipComponentTier maxComponentTier, float loadoutEfficiency, Faction faction, GameObject objectContainer) { // slotEfficiency should define how likely a slot is to be fully-utilised
            List<Hull> hulls = new List<Hull>();

            switch (shipClass) {
                case ShipClass.Transport:
                    hulls = GetTransportHulls();
                    break;
                case ShipClass.Fighter:
                    hulls = GetFighterHulls();
                    break;
            }

            //pick a hull
            Random r = new Random();

            int hullIdx = r.Next(0, hulls.Count);
            Hull chosenHull = hulls[hullIdx];

            Ship ship = objectContainer.AddComponent<Ship>();
            ship.ShipHull = chosenHull;
            shipObjectHandler.ManagedShip = ship;
            shipObjectHandler.CreateShip(objectContainer.transform);

            //choose thrusters
            List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> mainThrusterSlots = shipObjectHandler.ManagedShip.ShipHull.MainThrusterComponents;

            mainThrusterSlots = mainThrusterSlots.OrderBy(t => r.Next()).ToList(); //randomize the slots so that different configurations will be chosen on each ship with multiple thrusters

            int slotsUsed = RollForSlotsUsed(loadoutEfficiency, mainThrusterSlots.Count, r);

            //thrusters

            List<int> assignedMainThrusterSlots = new List<int>();
            int slotIdx = 0;
            while (assignedMainThrusterSlots.Count < slotsUsed) {//while there are more slots ot choose
                while (assignedMainThrusterSlots.Contains(slotIdx)) {//while the current slot has already been chosen
                    slotIdx++;
                }
                
                var slot = mainThrusterSlots[slotIdx];
                
                List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)> currentTiedThrusters = new List<(ShipComponentType componentType, ShipComponentTier maxSize, MainThruster concreteComponent, string parentTransformName, bool needsBracket)>();
                foreach (var tiedThrusters in ship.ShipHull.TiedThrustersSets) {//for all tied thruster sets
                    if (tiedThrusters.Contains(slot)) {//if the current slot is a tied thruster
                        currentTiedThrusters = tiedThrusters;
                        break;
                    }
                }
                
                ShipComponentTier slotTier = slot.maxSize;
                ShipComponentTier slotAdjustedMaxTier = maxComponentTier <= slotTier ? maxComponentTier : slotTier;
                MainThruster chosenThruster = RollForMainThruster(loadoutEfficiency, slotAdjustedMaxTier, faction, r);
                string slotName = slot.parentTransformName;
                
                if (currentTiedThrusters.Count>0) {
                    //set all thrusters to the chosen thruster type
                    foreach (var thruster in currentTiedThrusters) {
                        slotName = thruster.parentTransformName;
                        shipObjectHandler.SetMainThrusterComponent(slotName, chosenThruster);
                        assignedMainThrusterSlots.Add(mainThrusterSlots.IndexOf(thruster));
                    }
                }
                else {
                    shipObjectHandler.SetMainThrusterComponent(slotName, chosenThruster);
                    assignedMainThrusterSlots.Add(slotIdx);
                }
            }

            var manoeuvringThrusterSlot = shipObjectHandler.ManagedShip.ShipHull.ManoeuvringThrusterComponents;
            ShipComponentTier manoeuvringThrusterTier = RollForSlotTier(loadoutEfficiency, maxComponentTier, r);
            var maxSize = manoeuvringThrusterSlot.maxSize;
            manoeuvringThrusterTier = manoeuvringThrusterTier <= maxSize ? manoeuvringThrusterTier : maxSize; 
            ManoeuvringThruster manoeuvringThruster = new ManoeuvringThruster(manoeuvringThrusterTier);
            shipObjectHandler.SetManoeuvringThrusterComponent(manoeuvringThruster);

            int currentAssignedSlots = 0;
            //weapons
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> weaponSlots = shipObjectHandler.ManagedShip.ShipHull.WeaponComponents.OrderBy(w => r.Next()).ToList();
            slotsUsed = RollForSlotsUsed(loadoutEfficiency, weaponSlots.Count, r);
            currentAssignedSlots = 0;

            for (int i = 0; i < weaponSlots.Count; i++) {
                ShipComponentTier slotTier = weaponSlots[i].maxSize;
                ShipComponentTier slotAdjustedMaxTier = maxComponentTier <= slotTier ? maxComponentTier : slotTier;
                Weapon chosenWeapon = RollForWeapon(loadoutEfficiency, slotAdjustedMaxTier, faction, r);
                string slotName = weaponSlots[i].parentTransformName;
                shipObjectHandler.SetWeaponComponent(slotName, chosenWeapon);
                currentAssignedSlots++;
                if (currentAssignedSlots >= slotsUsed) {
                    break;
                }
            }

            SetupBasicLoadout(shipObjectHandler.ManagedShip);

            return shipObjectHandler.ManagedShip;
        }

        private MainThruster RollForMainThruster(float slotEfficiency, ShipComponentTier slotAdjustedMaxTier, Faction faction, Random r) {
            ShipComponentTier chosenTier = RollForSlotTier(slotEfficiency, slotAdjustedMaxTier, r);
            List<(Type mainThrusterType, int spawnWeighting)> thrusters = faction.GetAllowedMainThrusters();

            int chosenThrusterIndex = RollForSpawnWeightedIndex(thrusters.Select(t => t.spawnWeighting).ToList(), r);
            Type chosenType = thrusters[chosenThrusterIndex].mainThrusterType;
            MainThruster chosenThruster = (MainThruster)Activator.CreateInstance(chosenType, chosenTier);

            return chosenThruster;
        }

        private Weapon RollForWeapon(float slotEfficiency, ShipComponentTier slotAdjustedMaxTier, Faction faction, Random r) {
            ShipComponentTier chosenTier = RollForSlotTier(slotEfficiency, slotAdjustedMaxTier, r);
            List<(Type weaponType, int spawnWeighting)> weapons = faction.GetAllowedWeapons();

            int chosenWeaponIndex = RollForSpawnWeightedIndex(weapons.Select(t => t.spawnWeighting).ToList(), r);
            Type chosenType = weapons[chosenWeaponIndex].weaponType;
            Weapon chosenWeapon = (Weapon)Activator.CreateInstance(chosenType, chosenTier);

            return chosenWeapon;
        }

        private int RollForSpawnWeightedIndex(List<int> weights, Random r) {
            int sum = 0;
            List<int> cutoffs = new List<int>();
            foreach (int weight in weights) {
                sum += weight;
                cutoffs.Add(sum);
            }

            int roll = r.Next(sum);

            int index = cutoffs.Count - 1; //selects the last index by default so it is initialised
            for (int i = 0; i < cutoffs.Count; i++) {
                if (roll <= cutoffs[i]) { //if roll is below or equal to the cutoff then i is the selected index
                    index = i;
                    break;
                }
            }

            return index;
        }

        private int RollForSlotsUsed(float slotEfficiency, int numOfSlots, Random r) {
            bool notChosen = true;
            int currentSlots = numOfSlots;

            while (notChosen) {
                notChosen = (float)r.NextDouble() > slotEfficiency;
                if (currentSlots > 1 && notChosen) {
                    currentSlots--;
                }
                else {
                    break;
                }
            }

            return currentSlots;
        }

        private ShipComponentTier RollForSlotTier(float slotEfficiency, ShipComponentTier maxComponentTier, Random r) {
            bool notChosen = true;
            int currentTier = (int)maxComponentTier;

            while (notChosen) {
                notChosen = (float)r.NextDouble() > slotEfficiency;
                if (currentTier > 0 && notChosen) {
                    currentTier--;
                }
                else {
                    break;
                }
            }

            return (ShipComponentTier)currentTier;
        }

        private List<Hull> GetTransportHulls() {
            return new List<Hull>() { new SmallCargoHull() };
        }

        private List<Hull> GetFighterHulls() {
            return new List<Hull>() { new SmallFighterHull() };
        }
    }
}