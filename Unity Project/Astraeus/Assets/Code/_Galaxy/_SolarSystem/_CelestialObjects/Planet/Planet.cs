using Code.TextureGen;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Planet {
    public class Planet : CelestialBody {
        public Planet(Body primary, BodyTier tier, PlanetGen planetGen) : base(primary, tier,planetGen.MapColour) {
            PlanetGen = planetGen;
        }

        public const BodyTier MAXPlanetTier = BodyTier.T6;
        public const BodyTier MINPlanetTier = BodyTier.T1;

        public PlanetGen PlanetGen { get; }
        public Texture2D SurfaceTexture;
        public override GameObject GetSystemObject() {
            GameObject sphere = (GameObject)Resources.Load("Bodies/Celestial/Sphere/WarpSphere");;
            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.mainTexture = SurfaceTexture;
            meshRenderer.material = material;
            return sphere;
        }

        public void GeneratePlanetColours() {
            PlanetGen.GenColors();
        }

        public void GeneratePlanetTexture() {
            SurfaceTexture = PlanetGen.GenTexture();
        }
    }
}