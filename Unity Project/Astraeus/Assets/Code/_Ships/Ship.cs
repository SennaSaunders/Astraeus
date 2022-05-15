using Code._Galaxy._Factions;
using Code._Ships.Hulls;
using Newtonsoft.Json;
using UnityEngine;

namespace Code._Ships {
    public class Ship
    {
        public Ship(Hull shipHull) {
            ShipHull = shipHull;
        }

        public Hull ShipHull { get; set; }
        [JsonIgnore]
        public GameObject ShipObject { get; set; }
        public Faction Faction;
    }
}