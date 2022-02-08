using System.Collections.Generic;
using System.Linq;
using Code._Galaxy._Factions;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using Code._Ships.ShipComponents.ExternalComponents.Thrusters;
using Code._Ships.ShipComponents.ExternalComponents.Weapons;
using Code._Ships.ShipComponents.InternalComponents;
using UnityEngine;
using Random = System.Random;

namespace Code._GameControllers {
    public class ShipCreator : MonoBehaviour {
        public ShipObjectHandler _shipObjectHandler;

        private void Awake() {
            _shipObjectHandler = gameObject.AddComponent<ShipObjectHandler>();
        }

        public enum ShipClass {
            Transport,
            Fighter
        }

        public Ship CreateDefaultShip(GameObject objectContainer) {
            Ship ship = objectContainer.AddComponent<Ship>();
            ship.ShipHull = new SmallFighterHull();
            _shipObjectHandler.ManagedShip = ship;
            ship.ShipObject = _shipObjectHandler.CreateShip(objectContainer.transform);
            return ship;
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
            _shipObjectHandler.ManagedShip = ship;
            _shipObjectHandler.CreateShip(objectContainer.transform);

            //choose thrusters
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)> mainThrusterSlots = _shipObjectHandler.ManagedShip.ShipHull.ThrusterComponents;

            mainThrusterSlots = mainThrusterSlots.OrderBy(t => r.Next()).ToList(); //randomize the slots so that different configurations will be chosen on each ship with multiple thrusters

            int slotsUsed = RollForSlotsUsed(loadoutEfficiency, mainThrusterSlots.Count, r);
            
            
            //thrusters

            List<int> assignedThrusterSlots = new List<int>();
            int slotIdx = 0;
            while (assignedThrusterSlots.Count < slotsUsed) {//while there are more slots ot choose
                while (assignedThrusterSlots.Contains(slotIdx)) {//while the current slot has already been chosen
                    slotIdx++;
                }
                
                var slot = mainThrusterSlots[slotIdx];
                
                List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)> currentTiedThrusters = new List<(ShipComponentType componentType, ShipComponentTier maxSize, Thruster concreteComponent, string parentTransformName, bool needsBracket)>();
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
                        _shipObjectHandler.SetMainThrusterComponent(slotName, chosenThruster);
                        assignedThrusterSlots.Add(mainThrusterSlots.IndexOf(thruster));
                    }
                }
                else {
                    _shipObjectHandler.SetMainThrusterComponent(slotName, chosenThruster);
                    assignedThrusterSlots.Add(slotIdx);
                }
            }


            int currentAssignedSlots = 0;
            //weapons
            List<(ShipComponentType componentType, ShipComponentTier maxSize, Weapon concreteComponent, string parentTransformName)> weaponSlots = _shipObjectHandler.ManagedShip.ShipHull.WeaponComponents.OrderBy(w => r.Next()).ToList();
            slotsUsed = RollForSlotsUsed(loadoutEfficiency, weaponSlots.Count, r);
            currentAssignedSlots = 0;

            for (int i = 0; i < weaponSlots.Count; i++) {
                ShipComponentTier slotTier = weaponSlots[i].maxSize;
                ShipComponentTier slotAdjustedMaxTier = maxComponentTier <= slotTier ? maxComponentTier : slotTier;
                Weapon chosenWeapon = RollForWeapon(loadoutEfficiency, slotAdjustedMaxTier, faction, r);
                string slotName = weaponSlots[i].parentTransformName;
                _shipObjectHandler.SetWeaponComponent(slotName, chosenWeapon);
                currentAssignedSlots++;
                if (currentAssignedSlots >= slotsUsed) {
                    break;
                }
            }

            List<(ShipComponentType componentType, ShipComponentTier maxSize, InternalComponent concreteComponent, string parentTransformName)> internalSlots = _shipObjectHandler.ManagedShip.ShipHull.InternalComponents;

            //choose a power plant
            //choose a shield

            return _shipObjectHandler.ManagedShip;
        }

        private MainThruster RollForMainThruster(float slotEfficiency, ShipComponentTier slotAdjustedMaxTier, Faction faction, Random r) {
            ShipComponentTier chosenTier = RollForSlotTier(slotEfficiency, slotAdjustedMaxTier, r);
            List<(MainThruster mainThruster, int spawnWeighting)> thrusters = faction.GetAllowedMainThrusters(chosenTier);

            int chosenThrusterIndex = RollForSpawnWeightedIndex(thrusters.Select(t => t.spawnWeighting).ToList(), r);
            MainThruster chosenThruster = thrusters[chosenThrusterIndex].mainThruster;

            return chosenThruster;
        }

        private Weapon RollForWeapon(float slotEfficiency, ShipComponentTier slotAdjustedMaxTier, Faction faction, Random r) {
            ShipComponentTier chosenTier = RollForSlotTier(slotEfficiency, slotAdjustedMaxTier, r);
            List<(Weapon weapon, int spawnWeighting)> weapons = faction.GetAllowedWeapons(chosenTier);

            int chosenWeaponIndex = RollForSpawnWeightedIndex(weapons.Select(t => t.spawnWeighting).ToList(), r);
            Weapon chosenWeapon = weapons[chosenWeaponIndex].weapon;

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
            return new List<Hull>() { new MedCargoHull() };
        }

        private List<Hull> GetFighterHulls() {
            return new List<Hull>() { new SmallFighterHull() };
        }
    }
}