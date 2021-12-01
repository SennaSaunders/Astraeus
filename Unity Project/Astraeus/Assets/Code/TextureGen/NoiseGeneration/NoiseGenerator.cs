using System;

namespace Code.TextureGen.NoiseGeneration {
    public class NoiseGenerator {
        public static float[] GetNoise(int size, int seed) {
            float[] noiseData = new float[size * size];
            FastNoise fastNoiseTree = FastNoise.FromEncodedNodeTree("JgANAAQAAAAAAABACQAAmpkZPwCamZk+AAAAAAA=");

            int x1 = 0;
            int y1 = 0;
            int x2 = 1;
            int y2 = 1;
            float[] noiseTest = new float[size * size];
            //var test = fastNoiseTree.GenUniformGrid4D(noiseTest, 0, 0, 0, 0, size, size, size, size, 1, seed);
            
            float min = float.MaxValue;
            float max = float.MinValue;
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    float s = (float)x / size;
                    float t = (float)y / size;
                    float dx = x2 - x1;
                    float dy = y2 - y1;

                    float nx = (float)(x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    float ny = (float)(y1 + Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI));
                    float nz = (float)(x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI));
                    float nw = (float)(y1 + Math.Sin(t * 2 * Math.PI) * dy / (2 * Math.PI));
                    
                    float value =  fastNoiseTree.GenSingle4D(nx, ny, nz, nw, seed);
                    // float value =  fastNoiseTree.GenSingle2D(x, y, seed);
                    
                     
                    //float value =  fastNoiseTree.GenSingle2D(x, y, seed);
                    min = value < min ? value : min;
                    max = value > max ? value : max;
                    
                    noiseData[x * size + y] = value;
                }
            }

            for (int x = 0; x < size; x++) {//scales values from 0 - 1 
                for (int y = 0; y < size; y++) {
                    float value = noiseData[x * size + y];
                    noiseData[x * size + y] = (value - min) / (max - min);
                }
            }

            return noiseData;
        }
    }
}