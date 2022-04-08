using Code._Galaxy._SolarSystem._CelestialObjects;
using Code._Galaxy._SolarSystem._CelestialObjects.Planet;
using UnityEngine;

namespace Code.TextureGen {
    public class TextureTest : MonoBehaviour {
        MeshRenderer _meshRenderer;

        public int seed = 1337;
        public Body.BodyTier tier = Body.BodyTier.T2; 

        
        // Start is called before the first frame update
        public void Start() {
            GenPlanetTexture();
        }

        public void GenPlanetTexture() {
            _meshRenderer = GetComponent<MeshRenderer>();
            Planet planet = new Planet(null, tier, new RockyWorldGen(seed, tier.TextureSize())); 
            _meshRenderer.sharedMaterial.mainTexture = planet.PlanetGen.GenTexture();
        }
    }
}
