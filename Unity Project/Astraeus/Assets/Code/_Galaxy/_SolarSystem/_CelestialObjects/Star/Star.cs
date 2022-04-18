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

        //prefab
        public Star(Body primary, Vector2 coordinate, StarType starClass) : base(primary, coordinate, TierFromType(starClass)) {
            StarClass = starClass;
        }

        public Star(Body primary, StarType starClass) : base(primary, TierFromType(starClass)) {
            StarClass = starClass;
        }

        public override GameObject GetSystemObject() {
            GameObject sphere = base.GetSystemObject();
            //slap generated texture on the sphere
            //move noise generation to here

            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.material = (Material)Resources.Load("Materials/Star/"+StarClass.GetStarShader());
            meshRenderer.material.shader = Shader.Find("Shader Graphs/StarShaderGraph");
            return sphere;
        }

        public override GameObject GetMapObject() {
            
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.material = (Material)Resources.Load("Materials/Star/"+StarClass.GetStarShader());
            meshRenderer.material.shader = Shader.Find("Shader Graphs/StarShaderGraph");
            
            float scale = Tier.MapScale();
            sphere.transform.localScale = new Vector3(scale, scale, scale);
            return sphere;
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
        public static string GetStarShader(this Star.StarType type) {
            switch (type) {
                case Star.StarType.O:
                    return "StarO"; //super blue
                case Star.StarType.B:
                    return "StarB"; //light blue
                case Star.StarType.A: 
                    return "StarA"; //blue tinge 
                case Star.StarType.F: 
                    return "StarF"; //white 
                case Star.StarType.G:
                    return "StarG"; //yellow
                case Star.StarType.K:
                    return "StarK"; //red
                default:
                    return "StarM"; //orange
            }
        }
    }
}