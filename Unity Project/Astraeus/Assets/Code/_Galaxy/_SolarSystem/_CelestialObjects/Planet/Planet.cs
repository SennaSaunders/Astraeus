using Code.TextureGen;
using UnityEngine;

namespace Code._Galaxy._SolarSystem._CelestialObjects.Planet {
    public class Planet : CelestialBody {
        public Planet(Body primary, BodyTier tier, PlanetGen planetGen) : base(primary, tier) {
            PlanetGen = planetGen;
        }

        public const BodyTier MAXPlanetTier = BodyTier.T6;
        public const BodyTier MINPlanetTier = BodyTier.T1;

        public PlanetGen PlanetGen { get; }
        public Texture2D SurfaceTexture;
        public override GameObject GetSystemObject() {
            GameObject sphere = base.GetSystemObject();
            //slap generated texture on the sphere
            //move noise generation to here
            
            MeshRenderer meshRenderer = sphere.GetComponent<MeshRenderer>();
            meshRenderer.material.mainTexture = SurfaceTexture;
            meshRenderer.material.shader = Shader.Find("Unlit/Texture");
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