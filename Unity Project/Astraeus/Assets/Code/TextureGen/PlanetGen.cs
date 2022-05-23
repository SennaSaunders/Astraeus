using System;
using System.Collections.Generic;
using System.Linq;
using Code.TextureGen.NoiseGeneration;
using UnityEngine;
using Random = System.Random;

namespace Code.TextureGen {
    public abstract class PlanetGen {
        public Color MapColour;
        private int _seed, _size, width, height;
        internal float SeaLevel;
        private List<LandColourMapping> _landColourMappings = new List<LandColourMapping>();
        private Color _lowSeaColour, _highSeaColour;
        internal Color[] Colors;
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
            (3, 191, 172), //light blue
            (66, 161, 129), //turquoise
            (144, 229, 216), //sea green?
            (4, 108, 110), //dark turquoise
            (4, 80, 110), //dark blue
            (143, 102, 201), //light purple
            (9, 22, 57), //deep blue
            (4, 55, 109), //deep blue
        };

        //land colours list to choose from
        protected readonly List<(int r, int g, int b)> _rockyColours = new List<(int r, int g, int b)> {
            (255, 162, 0), //orange
            (131, 37, 0), //reddish orange
            (229, 99, 48), //orange
            (255, 98, 0), //traffic cone orange
            (255, 0, 0), //red
            (196, 36, 11), //burnt red
            (173, 29, 29), //dark red
            (140, 12, 61), //reddish pink
            (40, 40, 40), //dark gray
            (37, 57, 59), //dark blue gray
            (94, 36, 36), //murky red
        };

        protected readonly List<(int r, int g, int b)> _earthLandColours = new List<(int r, int g, int b)> {
            (68, 229, 32), //light green
            (93, 211, 25), //light green
            (44, 105, 25), //mid green
            (59, 145, 10), //mid green
            (20, 63, 5), //dark green
            (5, 66, 9), //dark green
            (5, 63, 34), //dark green
            (91, 127, 109), //blueish green
            (216, 201, 39), //sand?
            (216, 153, 34), //pale orange
            (229, 186, 139) //paler orange
        };

        private float GetLuminance((int r, int g, int b) colourCode) {
            return 0.2126f * colourCode.r + 0.7152f * colourCode.g + 0.0722f * colourCode.b;
        }

        public Color RGBToColour((int r, int g, int b) colourCode) {
            return new Color((float)colourCode.r / 255, (float)colourCode.g / 255, (float)colourCode.b / 255);
        }

        private class LandColourMapping {
            public LandColourMapping(Color colour, float fullColourHeight) {
                Colour = colour;
                FullColourHeight = fullColourHeight;
            }

            public readonly Color Colour;
            public readonly float FullColourHeight;
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

            if (GetLuminance(firstRGB) > GetLuminance(secondRGB)) {
                _highSeaColour = RGBToColour(firstRGB);
                _lowSeaColour = RGBToColour(secondRGB);
            }
            else {
                _highSeaColour = RGBToColour(secondRGB);
                _lowSeaColour = RGBToColour(firstRGB);
            }
        }

        protected void GenerateLandColourMapping(List<(int, int, int)> colourCodes) {
            float currentHeight = SeaLevel;
            int maxLandColours = 3;

            float colourLeft = 1 - currentHeight;
            float colourBandMod = .3f;
            float avgHeightPerBand = (colourLeft / maxLandColours);
            float minColourBandSize = avgHeightPerBand * (1 - colourBandMod);
            float maxColourBandSize = avgHeightPerBand * (1 + colourBandMod);
            //pick first colour
            _landColourMappings.Add(new LandColourMapping(RGBToColour(colourCodes[rng.Next(colourCodes.Count)]), currentHeight));
            //pick the rest of the colours
            while (currentHeight < 1) {
                float bandHeight = (float)(rng.NextDouble() * (maxColourBandSize - minColourBandSize) + minColourBandSize); //picks how tall the current colour band will be
                currentHeight += bandHeight;
                currentHeight = currentHeight > 1 ? 1 : maxLandColours - 1 == _landColourMappings.Count ? 1 : currentHeight; //if height over 1 then set height to 1, if one less than max colours has been picked set height to 1 - else keep current value

                Color newColour;
                do {
                    newColour = RGBToColour(colourCodes[rng.Next(colourCodes.Count)]); //pick a random colour 
                } while (newColour == _landColourMappings[_landColourMappings.Count - 1].Colour); //keep picking if it selects the same as the previous colour

                _landColourMappings.Add(new LandColourMapping(newColour, currentHeight));
            }
        }

        private Color GetLandColorFromHeight(float landHeight) {
            //find the upper of the colour band that the height belongs to
            int colourIndex = 0;
            while (colourIndex < _landColourMappings.Count-1) {
                if (_landColourMappings[colourIndex].FullColourHeight > landHeight) {
                    break;
                }
                colourIndex++;
            }

            LandColourMapping lowColourMap = _landColourMappings[colourIndex - 1];
            LandColourMapping highColourMap = _landColourMappings[colourIndex];

            float bandSize = highColourMap.FullColourHeight - lowColourMap.FullColourHeight;
            float heightInBand = landHeight - lowColourMap.FullColourHeight;
            float relativeHeight = heightInBand / bandSize;
            return Color.Lerp(lowColourMap.Colour, highColourMap.Colour, relativeHeight);
        }

        private Color[] GenLandOnly(float[] noise) {
            Color[] colors = new Color [width * height];

            for (int currentHeight = 0; currentHeight < height; currentHeight++) {
                for (int currentWidth = 0; currentWidth < width; currentWidth++) {
                    colors[currentHeight * width + currentWidth] = GetLandColorFromHeight(noise[currentWidth + currentHeight * width]);
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
                        colors[currentHeight * width + currentWidth] = GetLandColorFromHeight(noise[currentWidth + currentHeight * width]);
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
                Colors = GenLandOnly(noise);
            }
            else if (SeaLevel > 1 - 0.1) { //all sea
                Colors = GenSeaOnly(noise);
            }
            else { //land & sea
                Colors = GenLandAndSea(noise);
            }
        }

        public Texture2D GenTexture() {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(Colors);
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
            double lowestSea = .5;
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