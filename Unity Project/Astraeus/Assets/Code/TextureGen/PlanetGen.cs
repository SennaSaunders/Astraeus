using System;
using System.Collections.Generic;
using System.Linq;
using Code.TextureGen.NoiseGeneration;
using UnityEngine;
using Random = System.Random;

namespace Code.TextureGen {
    public abstract class PlanetGen {
        public Color MapColour;
        private int _seed;
        private int _size;
        private int width;
        private int height;
        private List<LandColourMapping> _landColourMappings;
        internal float SeaLevel;
        private Color _lowSeaColour;
        private Color _highSeaColour;
        public Color[] colors;
        protected Random rng;

        protected PlanetGen(int seed, int size, Color mapColour) {
            _seed = seed;
            _size = size;
            width = _size * 2;
            height = _size;
            rng = new Random(_seed);
            MapColour = mapColour;
        }

        //sea colours list to choose from
        private readonly List<(int r, int g, int b)> _seaColours = new List<(int r, int g, int b)> {
            (0, 255, 230), //light blue
            (66, 161, 129), //turquoise
            (4, 108, 110), //dark turquoise
            (4, 80, 110), //dark blue
            (109, 62, 175), //light purple
            (131, 37, 0), //reddish orange
            (9, 22, 57), //deep blue
        };

        //land colours list to choose from
        protected readonly List<(int r, int g, int b)> _rockyColours = new List<(int r, int g, int b)> {
            (101, 133, 224), //blueish gray
            (255, 162, 0), //orange
            (255, 98, 0), //orange
            (255, 0, 0), //red
            (173, 29, 29), //dark red
            (59, 0, 107), // deep purple
            (140, 56, 209), //light purple
            (140, 12, 61), //reddish pink
            (40, 40, 40), //dark gray
            (37, 57, 59), //dark blue gray
            (94, 36, 36), //murky red
        };
        
        protected readonly List<(int r, int g, int b)> _earthLandColours = new List<(int r, int g, int b)> {
            (68,229,32),//light green
            (44, 105, 25), //mid green
            (20, 63, 5), //dark green)
            (63,56,5),//green/brown
            (174,204,8),//greenish yellow
            (217,221,93),//sand?
            (216,153,34)//pale orange
        };

        private float GetLuminance((int r, int g, int b) colourCode) {
            return 0.2126f * colourCode.r + 0.7152f * colourCode.g + 0.0722f * colourCode.b;
        }

        public Color RGBToColour((int r, int g, int b) colourCode) {
            return new Color((float)colourCode.r / 255, (float)colourCode.g / 255, (float)colourCode.b / 255);
        }

        private class LandColourMapping {
            public LandColourMapping(Color colour, float relativeHeight) {
                Colour = colour;
                RelativeHeight = relativeHeight;
            }

            public readonly Color Colour;
            public readonly float RelativeHeight;
        }

        protected abstract void Setup(float[] noise);


        protected void GenerateSeaColours() {
            int firstIndex = rng.Next(_seaColours.Count);
            int secondIndex = firstIndex;

            while (secondIndex == firstIndex) {
                secondIndex = rng.Next(_seaColours.Count);
            }

            (int r, int g, int b) firstRGB = _seaColours[firstIndex];
            (int r, int g, int b) secondRGB = _seaColours[secondIndex];

            _highSeaColour = GetLuminance(firstRGB) > GetLuminance(secondRGB) ? RGBToColour(firstRGB) : RGBToColour(secondRGB);
            _lowSeaColour = GetLuminance(firstRGB) > GetLuminance(secondRGB) ? RGBToColour(secondRGB) : RGBToColour(firstRGB);
        }

        protected void GenerateLandColourMapping(List<(int, int, int)> colourCodes) {
            _landColourMappings = new List<LandColourMapping>();
            List<double> heights = new List<double>();
            double relativeColourLeft = 1;
            int maxLandColours = 3;
            while (relativeColourLeft > 0 && heights.Count < maxLandColours) {
                float minRelHeight = .005f;
                float maxRelHeight = .4f;
                double rollPercentage = rng.NextDouble() * (maxRelHeight - minRelHeight) + minRelHeight;
                relativeColourLeft -= rollPercentage;
                if (relativeColourLeft > 0 && heights.Count + 1 < maxLandColours) {
                    heights.Add(1 - relativeColourLeft);
                }
                else {
                    heights.Add(1);
                }
            }

            int lastColourRoll = -1;
            for (int i = 0; i < heights.Count; i++) {
                int landColourRoll;
                do {
                    landColourRoll = rng.Next(colourCodes.Count);
                } while (lastColourRoll == landColourRoll);

                lastColourRoll = landColourRoll;
                _landColourMappings.Add(new LandColourMapping(RGBToColour(colourCodes[landColourRoll]), (float)heights[i]));
            }
        }

        private (Color low, Color high, float relHeight) GetLandColorFromHeight(float landHeight) {
            float seaRelativeHeight = (landHeight - SeaLevel) / (1 - SeaLevel);
            int colourIndex = 0;


            for (int i = 0; i < _landColourMappings.Count - 1; i++) { //find which colour band the height belongs to
                if (_landColourMappings[i].RelativeHeight < seaRelativeHeight) {
                    colourIndex = i;
                }
            }

            LandColourMapping high = _landColourMappings[colourIndex + 1];
            LandColourMapping low = _landColourMappings[colourIndex];

            float bandRelHeight = (seaRelativeHeight - low.RelativeHeight) / (high.RelativeHeight - low.RelativeHeight);
            return (low.Colour, high.Colour, bandRelHeight);
        }

        private Color[] GenLandOnly(float[] noise) {
            Color[] colors = new Color [width * height];

            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    (Color low, Color high, float relativeHeight) colours = GetLandColorFromHeight(noise[currentWidth + currentHeight * width]);
                    colors[currentHeight * width + currentWidth] = Color.Lerp(colours.low, colours.high, colours.relativeHeight);
                }
            }

            return colors;
        }

        private Color[] GenLandAndSea(float[] noise) {
            Color[] colors = new Color [width * height];

            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    if (noise[currentWidth + currentHeight * width] < SeaLevel) {
                        colors[currentHeight * width + currentWidth] = Color.Lerp(_lowSeaColour, _highSeaColour, noise[currentWidth + currentHeight * width] / SeaLevel);
                    }
                    else {
                        (Color low, Color high, float relativeHeight) colours = GetLandColorFromHeight(noise[currentWidth + currentHeight * width]);
                        Color colour = Color.Lerp(colours.low, colours.high, colours.relativeHeight);
                        colors[currentHeight * width + currentWidth] = colour;
                    }
                }
            }

            return colors;
        }

        private Color[] GenSeaOnly(float[] noise) {
            Color[] colors = new Color [width * height];

            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    colors[currentHeight * width + currentWidth] = Color.Lerp(_lowSeaColour, _highSeaColour, noise[currentHeight * width + currentWidth]);
                }
            }

            return colors;
        }

        public void GenColors() {
            float[] noise = Get3DNoiseTo2D();
            Setup(noise);
            if (SeaLevel == 0) { //no sea
                colors = GenLandOnly(noise);
            }
            else if (SeaLevel > 1 - 0.1) { //all sea
                colors = GenSeaOnly(noise);
            }
            else { //land & sea
                colors = GenLandAndSea(noise);
            }
        }

        public Texture2D GenTexture() {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }

        public Texture2D GenNoiseTex() {
            float[] noise2D = Get3DNoiseTo2D();


            Color[] colours = new Color[width * height];

            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    float value = noise2D[currentHeight * width + currentWidth];
                    int index = currentWidth + width * currentHeight;
                    colours[index] = new Color(value, value, value);
                }
            }

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(colours);
            texture.Apply();
            return texture;
        }

        private float[] Get3DNoiseTo2D() {
            float[] noise2D = new float[width * height];
            float[] noise3DFlat = NoiseGenerator.GetNoise3D(_size, _seed);


            float sphereRadius = (float)_size / 2;
            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    float d = currentHeight < sphereRadius ? sphereRadius - currentHeight : currentHeight - sphereRadius;
                    float circleRadius = (float)Math.Sqrt(sphereRadius * sphereRadius - d * d);
                    float thetaRadians = (float)(currentWidth / (float)(_size * 2) * 2 * Math.PI);

                    int x = (int)(circleRadius * Math.Sin((thetaRadians)) + sphereRadius);
                    int y = (int)(circleRadius * Math.Cos((thetaRadians)) + sphereRadius);
                    int z = currentHeight;

                    float value = noise3DFlat[(z * _size * _size) + (y * _size) + x];
                    noise2D[currentWidth + currentHeight * width] = value;
                }
            }

            return noise2D;
        }

        protected float GetSeaLevel(float[] noise, float seaPercentage) {
            //how much of the noise is above the relative sealevel
            var orderedNoise = noise.ToList().OrderBy(n => n).ToList();

            for (int i = 0; i < orderedNoise.Count; i++) {
                float currentSeaPercentage = (float)i / orderedNoise.Count;
                if (currentSeaPercentage > seaPercentage) {
                    return noise[i];
                }
            }

            return 1;
        }
    }

    public class WaterWorldGen : PlanetGen { //very high sea level/only sea
        private static readonly Color waterMapColour = new Color(.4f, .9f, .9f);

        public WaterWorldGen(int seed, int size) : base(seed, size, waterMapColour) {
        }

        protected override void Setup(float[] noise) {
            SeaLevel = 1;
            GenerateSeaColours();
        }
    }


    public class EarthWorldGen : PlanetGen { //medium sea level
        private static readonly Color earthMapColour = new Color(.2f, 1f, 0);

        public EarthWorldGen(int seed, int size) : base(seed, size, earthMapColour) {
        }

        protected override void Setup(float[] noise) {
            double lowestSea = .4;
            double highestSea = .8;
            double seaDiff = highestSea - lowestSea;
            float seaPercentage = (float)(rng.NextDouble() * seaDiff + lowestSea);
            SeaLevel = GetSeaLevel(noise, seaPercentage);
            GenerateSeaColours();
            GenerateLandColourMapping(_earthLandColours);
        }
    }

    public class RockyWorldGen : PlanetGen { //no sea
        private static readonly Color rockyMapColour = new Color(.4f, .3f, 0);

        public RockyWorldGen(int seed, int size) : base(seed, size, rockyMapColour) {
        }

        protected override void Setup(float[] noise) {
            SeaLevel = 0;
            GenerateLandColourMapping(_rockyColours);
        }
    }
}