using Code._Galaxy._Factions;
using Code._Ships.Hulls;
using UnityEngine;

namespace Code._Ships {
    public class Ship
    {
        public Ship(Hull shipHull) {
            ShipHull = shipHull;
        }

        public Hull ShipHull { get; set; }
        public GameObject ShipObject { get; set; }
        public Faction Faction;
        public static int ShipMarkerSize = 200;
    }
}