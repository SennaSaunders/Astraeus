using Code.TextureGen.NoiseGeneration;
using UnityEngine;

namespace Code.TextureGen {
    public class TextureTest : MonoBehaviour {
        MeshRenderer _meshRenderer;

        public int seed = 1337;

        public int size = 1024;
        // Start is called before the first frame update
        public void Start() {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial.mainTexture = PlanetGen.GenNoiseTex(NoiseGenerator.GetNoise(size, seed), size);
        }
    }
}
