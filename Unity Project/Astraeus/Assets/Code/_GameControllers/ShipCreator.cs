using System.Collections.Generic;
using Code._Galaxy._Factions;
using Code._Ships;
using Code._Ships.Hulls;
using Code._Ships.ShipComponents;
using UnityEngine;

namespace Code._GameControllers {
    public class ShipCreator : MonoBehaviour {
        private ShipObjectHandler _shipObjectHandler;

        public enum ShipClass {
            Transport,
            Fighter
        }

        public Ship CreateShip(ShipClass shipClass, ShipComponentTier shipTier, Faction.FactionType factionType) {
            List<Hull> hulls;

            switch (shipClass) {
                case ShipClass.Transport:
                    hulls = GetTransportHulls();
                    break;
                case ShipClass.Fighter:
                    hulls = GetFighterHulls();
                    break;
            }
            
            //select a hull
            
            //depending on the ship tier kit it out with the necessary components
            
            //power plant
            //thrusters
            //weapons
            //cargo
            //

            return null;
        }


        public List<Hull> GetTransportHulls() {
            return new List<Hull>() { gameObject.AddComponent<MedCargoHull>() };
        }

        public List<Hull> GetFighterHulls() {
            return new List<Hull>() { gameObject.AddComponent<SmallFighterHull>() };
        }
    }
}