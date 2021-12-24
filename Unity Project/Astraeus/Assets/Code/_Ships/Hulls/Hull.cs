using System.Collections.Generic;

namespace Code._Ships.Hulls {
    //ship blueprint for the components allowed on a particular hull 
    public class Hull {
        public Hull(List<(ShipComponentType, int maxSize, int maxNum)> hullComponents, int mass) {
            HullComponents = hullComponents;
            HullMass = mass;
        }

        public List<(ShipComponentType, int maxSize, int maxNum)> HullComponents;
        public float HullMass;
    }

    public class TestLightHull:Hull {
        private static List<(ShipComponentType, int maxSize, int maxNum)> hullComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.MainThruster, 1, 2),         //2 x size 1 Main Thruster
            (ShipComponentType.ManoeuvringThruster, 1, 4),  //4 x size 1 Manoeuvring Thruster
            (ShipComponentType.Internal, 2, 1),             //2 x size 1 Internal Bays
            (ShipComponentType.Internal, 2, 2),             //2 x size 2 Internal Bays
            (ShipComponentType.Weapon, 2, 2)                // 2 x size 2 weapons hardpoints
        };
        public TestLightHull() : base(hullComponents, 5000) {
        }
    }
    
    public  class TestLargeHull:Hull {
        private static List<(ShipComponentType, int maxSize, int maxNum)> hullComponents = new List<(ShipComponentType, int maxSize, int maxNum)>() {
            (ShipComponentType.MainThruster, 3, 4),
            (ShipComponentType.ManoeuvringThruster, 3, 8),
            (ShipComponentType.Internal, 2, 4),
            (ShipComponentType.Internal, 3, 4),
            (ShipComponentType.Internal, 5, 2),
            (ShipComponentType.Weapon, 5, 3),
            (ShipComponentType.Weapon, 3, 4)
        };
        public TestLargeHull() : base(hullComponents, 50000) {
        }
    }
    
    
}