using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code._Galaxy.GalaxyComponents {
    public class Tile {
        private readonly float _x1, _x2;
        private readonly float _y1, _y2;
        public readonly int XIndex, YIndex;

        public Tile(int xIndex, int yIndex, float x1, float x2, float y1, float y2) {
            XIndex = xIndex;
            YIndex = yIndex;
            _x1 = x1;
            _x2 = x2;
            _y1 = y1;
            _y2 = y2;
        }

        public Vector2 GetRandomPoint() {
            float x = (float)GalaxyGenerator.Rng.NextDouble() * (_x2 - _x1) + _x1;
            float y = (float)GalaxyGenerator.Rng.NextDouble() * (_y2 - _y1) + _y1;
            return new Vector2(x, y);
        }

        public bool IsInsideTile(Vector2 pos) {
            return pos.x >= _x1 && pos.x < _x2 && pos.y >= _y1 && pos.y < _y2;
        }

        public List<(int, int)> GetSurroundingIndexes() {
            List<(int, int)> surroundingIndexes = new List<(int, int)> {
                (XIndex - 1, YIndex - 1), //top left
                (XIndex, YIndex - 1), //top middle
                (XIndex + 1, YIndex - 1), //top right
                (XIndex - 1, YIndex), //middle left
                (XIndex + 1, YIndex), //middle right
                (XIndex - 1, YIndex + 1), //bottom left
                (XIndex, YIndex + 1), //bottom middle
                (XIndex + 1, YIndex + 1) //bottom right
            };

            return surroundingIndexes;
        }
            
        public static bool InsideMapTiles((int x, int y) pos, int maxX, int maxY) {
            bool xValid = pos.x >= 0 && pos.x <= maxX;
            bool yValid = pos.y >= 0 && pos.y <= maxY;
            return xValid && yValid;
        }

        public float GetDistanceToTile(Tile otherTile) {
            float a = otherTile.XIndex - XIndex;
            float b = otherTile.YIndex - YIndex;
            float c = (float)Math.Sqrt(a * a + b * b);

            return c;
        }
    }
}