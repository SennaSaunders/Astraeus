using Code.TextureGen;
using UnityEngine;

namespace Code._CelestialObjects.Planet {
    public class Planet : CelestialBody {
        public Planet(Body primary, BodyTier tier, PlanetGen planetGen) : base(primary, tier) {
            _planetGen = planetGen;
        }

        public const BodyTier MAXPlanetTier = BodyTier.T6;
        public const BodyTier MINPlanetTier = BodyTier.T1;

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