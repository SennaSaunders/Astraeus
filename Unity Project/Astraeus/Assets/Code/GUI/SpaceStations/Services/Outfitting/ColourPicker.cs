using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class ColourPicker : MonoBehaviour {
        private Color _selectedColour;
        public int spacing = 10;
        public int colourBoxSize = 70;
        private ShipColourGUIController _shipColourGUIController;
        private float _saturation;
        
        private int _startMod;
        void Start() {
            GenerateColourBoxes();
        }

        public void Setup(ShipColourGUIController shipColourGUIController, float saturation, bool trimStart) {
            _shipColourGUIController = shipColourGUIController;
            _saturation = saturation;
            _startMod = trimStart ? 1 : 0;
        }

        private void ChangeColour() {
            _shipColourGUIController.SetColour(_selectedColour);
        } 

        private void GenerateColourBoxes() {
            Vector2 areaSize = gameObject.GetComponent<RectTransform>().sizeDelta;
            
            int xCount = (int)((areaSize.x-spacing) / (colourBoxSize + spacing));
            int yCount = (int)((areaSize.y-spacing) / (colourBoxSize + spacing));
        
            float xRemainder = (spacing + (spacing + colourBoxSize) * xCount - areaSize.x);
            float yRemainder = (spacing + (spacing + colourBoxSize) * yCount - areaSize.y);

            // var colours = GetColoursHS(xCount, yCount);
            var colours = GetColoursHV(xCount, yCount, _saturation);
            
            for (int i = 0; i < xCount; i++) {
                float x = spacing + (spacing + colourBoxSize) * i - xRemainder/2;
                for (int j = 0; j < yCount; j++) {
                    float y = spacing + (spacing + colourBoxSize) * j - yRemainder/2;

                    Color colour = colours[i * yCount + j];

                    GameObject colourBoxObj = new GameObject("ColourBox");
                    colourBoxObj.transform.SetParent(gameObject.transform);
                    
                    ColourBox colourBox = colourBoxObj.AddComponent<ColourBox>();
                    colourBox.Setup(new Color(colour.r, colour.g, colour.b), colourBoxObj, new Vector3(x,y,0), new Vector2(colourBoxSize, colourBoxSize), this);
                }
            }
        }
        private List<Color> GetColoursHS(int x, int y, float brightness) {
            List<Color> colours = new List<Color>();
            float saturationMin = 0.5f;
            float saturationMax = 1;
            for (int i = x; i > 0; i--) {
                for (int j = y; j > 0; j--) {
                    float satRatio = (float)(i+_startMod) / x;
                    float saturation = (satRatio * (saturationMax- saturationMin))+saturationMin;
                    float hue = (float)(j+_startMod) / y;
                    colours.Add(Color.HSVToRGB(hue, saturation, brightness));
                }
            }
            return colours;
        }
        
        private List<Color> GetColoursHV(int x, int y, float saturation) {
            List<Color> colours = new List<Color>();
            
            for (int i = x; i > 0; i--) {
                for (int j = y; j > 0; j--) {
                    float hue = (float)(i+_startMod) / x;
                    float brightness = (float)(j+_startMod) / y;
                    colours.Add(Color.HSVToRGB(hue,saturation,  brightness));
                }
            }

            return colours;
        }

        private class ColourBox : MonoBehaviour, IPointerClickHandler {
            private ColourPicker _colourPicker;
            private Color _colour;
            public void  Setup(Color colour, GameObject parent, Vector3 position, Vector2 size, ColourPicker colourPicker) {
                parent.AddComponent<CanvasRenderer>();
                Image image = parent.AddComponent<Image>();
                image.color = colour;
                _colour = colour;
            
                RectTransform rectTransform = parent.GetComponent<RectTransform>();
                rectTransform.anchorMax = new Vector2();
                rectTransform.anchorMin = new Vector2();
                rectTransform.pivot = new Vector2();
                rectTransform.sizeDelta = size;
                rectTransform.anchoredPosition = position;
                rectTransform.localScale = new Vector3(1, 1, 1);
                _colourPicker = colourPicker;
            }

            public void OnPointerClick(PointerEventData eventData) {
                _colourPicker._selectedColour = _colour;
                _colourPicker.ChangeColour();
                
            }
        }
    }
}
