using System.Collections;
using System.Collections.Generic;
using Code.TextureGen;
using Code.TextureGen.NoiseGeneration;
using UnityEngine;
using UnityEngine.Serialization;

public class TextureTest : MonoBehaviour {
    MeshRenderer meshRenderer;

    public int seed = 1337;

    public int size = 1024;
    // Start is called before the first frame update
    public void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.mainTexture = PlanetGen.GenNoiseTex(NoiseGenerator.GetNoise(size, seed), size);
    }
}
