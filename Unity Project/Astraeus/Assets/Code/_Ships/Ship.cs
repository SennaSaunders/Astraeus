using Code._Ships.Hulls;
using UnityEngine;

namespace Code._Ships {
    public class Ship : MonoBehaviour {
        public Hull ShipHull { get; set; }
        public GameObject ShipObject { get; set; }
        public bool Active { get; set; } = false;
    }
}