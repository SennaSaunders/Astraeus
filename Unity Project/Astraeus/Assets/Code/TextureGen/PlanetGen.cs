﻿using System.Collections.Generic;
using Code.Galaxy;
using UnityEngine;

namespace Code.TextureGen {
    public abstract class PlanetGen {
        //sea colours list to choose from
        private List<(int r, int g, int b)> _seaColours = new List<(int r, int g, int b)>() {
            (0, 255, 230),
            (105, 255, 205),
            (66, 161, 129),
            (4, 108, 110),
            (4, 80, 110),
            (0, 152, 212),
            (0, 48, 168),
            (0, 84, 173),
            (0, 118, 245)
        };

        //land colours list to choose from
        private List<(int r, int g, int b)> _landColours = new List<(int r, int g, int b)>() {
            (77, 191, 10), //light green
            (42, 112, 1), //dark green
            (42, 51, 2), //jungle green
            (255, 255, 0), //yellow
            (240, 242, 92), //sand
            (110, 110, 69), //washed out yellow
            (115, 87, 9), //brown
            (199, 155, 34), //light brown
            (255, 162, 0), //orange
            (255, 98, 0), //orange
            (255, 0, 0), //red
            (173, 29, 29), //dark red
            (59, 0, 107), // deep purple
            (140, 56, 209), //light purple
            (235, 0, 231), //bright pink
            (242, 92, 240), //light pink
            (105, 105, 105), //gray
            (65, 65, 65), //dark gray
            (40, 40, 40), //darkest gray
            (37, 57, 59), //dark blue gray
            (56, 36, 36) //murky red brown
        };

        public float GetLuminance((int r, int g, int b) colourCode) {
            return 0.2126f * colourCode.r + 0.7152f * colourCode.g + 0.0722f * colourCode.b;
        }

        public Color RGBToColour((int r, int g, int b) colourCode) {
            return new Color((float)colourCode.r / 255, (float)colourCode.g / 255, (float)colourCode.b / 255);
        }

        internal class LandColourMapping {
            public LandColourMapping(Color colour, float height) {
                this.Colour = colour;
                this.Height = height;
            }

            public Color Colour;
            public float Height; // treat like flex box in CSS - sea height will condense the total region that is available for these colour mappings
        }

        public abstract void Setup();

        private List<LandColourMapping> _landColourMappings;
        internal float SeaLevel;
        private Color _lowSeaColour;
        private Color _highSeaColour;
        public Texture2D PlanetTexture;

        public void GenerateSeaColours() {
            int firstIndex = GalaxyGenerator.Rng.Next(_seaColours.Count);
            int secondIndex = firstIndex;

            while (secondIndex == firstIndex) {
                secondIndex = GalaxyGenerator.Rng.Next(_seaColours.Count);
            }

            (int r, int g, int b) firstRGB = _seaColours[firstIndex];
            (int r, int g, int b) secondRGB = _seaColours[secondIndex];
            
            _highSeaColour = GetLuminance(firstRGB) > GetLuminance(secondRGB) ? RGBToColour(firstRGB) : RGBToColour(secondRGB);
            _lowSeaColour = GetLuminance(firstRGB) > GetLuminance(secondRGB) ? RGBToColour(secondRGB) : RGBToColour(firstRGB);
        }
        
        public void GenerateLandColourMapping() {
            // how to select main colour band?
            //do we always want it to be the lowest one?
            //no
            //so we need to pick a random one
            //roll from 0 - 1
            _landColourMappings = new List<LandColourMapping>();
            List<double> heights = new List<double>();
            double relativeColourLeft = 1;
            while (relativeColourLeft > 0 && heights.Count < 2) {
                double rollPercentage = GalaxyGenerator.Rng.NextDouble();
                relativeColourLeft -= rollPercentage;
                if (relativeColourLeft > 0) {
                    heights.Add(rollPercentage);
                }
                else {
                    heights.Add(rollPercentage + relativeColourLeft);
                }
            }

            int lastColourRoll = -1;
            for (int i = 0; i < heights.Count; i++) {
                int landColourRoll = -1;
                do {
                    lastColourRoll = GalaxyGenerator.Rng.Next(_landColours.Count);
                } while (lastColourRoll == landColourRoll);

                lastColourRoll = landColourRoll;
                var rgb = _landColours[landColourRoll];
                _landColourMappings.Add(new LandColourMapping(new Color(rgb.r, rgb.g, rgb.b), (float)heights[i]));
            }
        }

        public (Color low, Color high, float relHeight) GetLandColorFromHeight(float height) {
            int colourIndex = 0;
            for (int i = 0; i < _landColourMappings.Count-1; i++) {   //find which colour the height belongs to
                if (height < _landColourMappings[i].Height) {
                    colourIndex = i;
                }
                else {
                    break;
                }
            }
            
            //lerp from the colour it belongs to the above colour
            
            
            Color high = _landColourMappings[colourIndex].Colour;
            Color low = _landColourMappings[colourIndex+1].Colour;
            float heightDiff = _landColourMappings[colourIndex + 1].Height - _landColourMappings[colourIndex].Height;
            float relativeHeight = (height - _landColourMappings[colourIndex].Height) / heightDiff;
            return (high, low, relativeHeight);
        }

        private Color[] GenLandOnly(float[,] noise) {
            int x = noise.GetLength(0);
            int y = noise.GetLength(1);
            Color[] colors = new Color [x * y];

            for (int i = 0; i < x; i++) {
                for (int j = 0; j < x; j++) {
                    (Color low, Color high, float relativeHeight) colours = GetLandColorFromHeight(noise[i,j]);
                    colors[i * y + j] = Color.Lerp(colours.low, colours.high, colours.relativeHeight);
                }
            }
            
            return colors;
        }
        
        private Color[] GenLandAndSea(float[,] noise) {
            int x = noise.GetLength(0);
            int y = noise.GetLength(1);
            Color[] colors = new Color [x * y];

            for (int i = 0; i < x; i++) {
                for (int j = 0; j < x; j++) {
                    if (noise[i, j] < SeaLevel) {
                        colors[i * y + j] = Color.Lerp(_lowSeaColour, _highSeaColour, noise[i, j]);
                    }
                    else {
                        (Color low, Color high, float relativeHeight) colours = GetLandColorFromHeight(noise[i,j]);
                        colors[i * y + j] = Color.Lerp(colours.low, colours.high, colours.relativeHeight);
                    }
                }
            }
            
            return colors;
        }
        
        private Color[] GenSeaOnly(float[,] noise) {
            int x = noise.GetLength(0);
            int y = noise.GetLength(1);
            Color[] colors = new Color [x * y];

            for (int i = 0; i < x; i++) {
                for (int j = 0; j < x; j++) {
                    colors[i * y + j] = Color.Lerp(_lowSeaColour, _highSeaColour, noise[i, j]);
                }
            }
            
            return colors;
        }

        public Texture2D GenTexture(float[,] noise) {
            Texture2D texture = new Texture2D(noise.Length, noise.Length);
            Color[] colors = new Color[]{};
            if (SeaLevel == 0) { //no sea
                colors = GenLandOnly(noise);
            } else if (SeaLevel > 1 - 0.1) { //all sea
                colors = GenSeaOnly(noise);
            }
            else { //land & sea
                colors = GenLandAndSea(noise);
            }
            texture.SetPixels(colors);
            return texture;
        }
    }

    public class WaterWorldGen : PlanetGen { //very high sea level/only sea
        //
        public override void Setup() {
            //roll for all sea or high sea
            double allSeaChance = 0.9;
            if (GalaxyGenerator.Rng.NextDouble() > allSeaChance) { //some land
                double highestHighNotAllSea = .97;
                double lowestHighNotAllSea = .9;
                double seaDiff = highestHighNotAllSea - lowestHighNotAllSea;
                SeaLevel = (float)(GalaxyGenerator.Rng.NextDouble() * seaDiff + lowestHighNotAllSea);
                GenerateLandColourMapping();
            }
            else { //all sea
                SeaLevel = 1;
            }
            GenerateSeaColours();
        }
    }

    public class RockyWorldGen : PlanetGen { //no sea
        public override void Setup() {
            GenerateLandColourMapping();
            SeaLevel = 0;
        }
    }

    public class EarthWorldGen : PlanetGen { //medium sea level
        public override void Setup() {
            double lowestSea = .4;
            double highestSea = .8;
            double seaDiff = highestSea - lowestSea;
            SeaLevel = (float)(GalaxyGenerator.Rng.NextDouble() * seaDiff + lowestSea);
            GenerateSeaColours();
            GenerateLandColourMapping();
        }
    }
}