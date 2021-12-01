using Code.TextureGen;
using UnityEngine;

namespace Code.CelestialObjects.Planet {
    public class Planet : CelestialBody {
        public Planet(Body primary, BodyTier tier, PlanetGen planetGen) : base(primary, tier) {
            _planetGen = planetGen;
        }

        public static BodyTier maxPlanetTier = BodyTier.T6;
        public static BodyTier minPlanetTier = BodyTier.T1;

        private PlanetGen _planetGen;
        private Texture _surfaceTexture;
        public override GameObject GetSystemObject() {
            GameObject sphere = base.GetSystemObject();
            //slap generated texture on the sphere
            //move noise generation to here
            sphere.GetComponent<MeshRenderer>().material.mainTexture = _surfaceTexture;
            return sphere;
        }
    }
}