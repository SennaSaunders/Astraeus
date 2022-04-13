using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using UnityEngine;

namespace Code.TextureGen {
    public class TextureTest : MonoBehaviour {
        MeshRenderer _meshRenderer;

        public int seed = 1337;
        public Body.BodyTier tier = Body.BodyTier.T2;
        public PlanetGenType planetGenType;

        public enum PlanetGenType {
            Earth,
            Water,
            Rocky
        }

        // Start is called before the first frame update
        public void Start() {
            GenPlanetTexture();
        }

        public void GenPlanetTexture() {
            _meshRenderer = GetComponent<MeshRenderer>();
            Planet planet;
            if (planetGenType == PlanetGenType.Earth) {
                planet = new Planet(null, tier, new EarthWorldGen(seed, tier.TextureSize()));
            } else if (planetGenType == PlanetGenType.Water) {
                planet = new Planet(null, tier, new WaterWorldGen(seed, tier.TextureSize()));
            }
            else {
                planet = new Planet(null, tier, new RockyWorldGen(seed, tier.TextureSize()));
            }
            planet.PlanetGen.GenColors();
            _meshRenderer.material.mainTexture = planet.PlanetGen.GenTexture();
        }
    }
}
