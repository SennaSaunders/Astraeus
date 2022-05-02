using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Star {
    public class Star : CelestialBody {
        public enum StarType {
            O,
            B,
            A,
            F,
            G,
            K,
            M 
        }

        public StarType StarClass { get; }
        private static Color starColor = new Color(1,1,0); 

        //prefab
        public Star(Body primary, Vector2 coordinate, StarType starClass) : base(primary, coordinate, TierFromType(starClass),starColor) {
            StarClass = starClass;
        }

        public Star(Body primary, StarType starClass) : base(primary, TierFromType(starClass),starColor) {
            StarClass = starClass;
        }

        public override GameObject GetSystemObject() {
            GameObject starObject = (GameObject)Resources.Load(StarClass.GetStarObjectPath());
            return starObject;
        }

        

        private static BodyTier TierFromType(StarType type) {
            return type switch {
                StarType.O => BodyTier.T9,
                StarType.B => BodyTier.T8,
                _ => BodyTier.T7
            };
        }
    }

    public static class StarTypeExtension {
        public static string GetStarObjectPath(this Star.StarType type) {
            string basePath = "Bodies/Celestial/Star/Prefabs/";
            switch (type) {
                case Star.StarType.O:
                    return basePath + "StarO"; //super blue
                case Star.StarType.B:
                    return basePath + "StarB"; //light blue
                case Star.StarType.A: 
                    return basePath + "StarA"; //blue tinge 
                case Star.StarType.F: 
                    return basePath + "StarF"; //white 
                case Star.StarType.G:
                    return basePath + "StarG"; //yellow
                case Star.StarType.K:
                    return basePath + "StarK"; //red
                default:
                    return basePath + "StarM"; //orange
            }
        }
    }
}