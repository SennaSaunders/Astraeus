namespace Code.TextureGen.NoiseGeneration {
    public static class NoiseGenerator {
        private static FastNoise GetFastNoise() {
            FastNoise simplexNoise = new FastNoise("OpenSimplex2");
            FastNoise fractal = new FastNoise("FractalFBm");
            fractal.Set("Source", simplexNoise);
            fractal.Set("Gain", .6f);
            fractal.Set("WeightedStrength", .3f);
            fractal.Set("Octaves", 4);
            fractal.Set("Lacunarity", 2f);
            return fractal;
        }

        public static float[] GetNoise3D(int size, int seed) {
            FastNoise fastNoise = GetFastNoise();
            float[] noise = new float[size*size*size];
            float freq = 0.01f;
            FastNoise.OutputMinMax minMax = fastNoise.GenUniformGrid3D(noise, 0, 0, 0, size, size, size, freq, seed);
            noise = NormaliseNoise(noise, minMax);
            return noise;
        }

        private static float[] NormaliseNoise(float[] noise, FastNoise.OutputMinMax minMax) {
            for (int i = 0; i < noise.Length;i++) {
                float value = noise[i]; 
                noise[i] = (value - minMax.min) / (minMax.max - minMax.min);
            }
            return noise;
        }
    }
}