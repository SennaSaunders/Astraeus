using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class ColourPicker : MonoBehaviour {
        private Color selectedColour;
        public int spacing = 10;
        public int colourBoxSize = 70;
        void Start() {
            GenerateColourBoxes();
        }

        public void ChangeBackgroundColour() {
            GameObject.Find("Background").GetComponent<Image>().color = selectedColour;
        } 

        private void GenerateColourBoxes() {
            Vector2 areaSize = gameObject.GetComponent<RectTransform>().sizeDelta;
            
            int xCount = (int)((areaSize.x-spacing) / (colourBoxSize + spacing));
            int yCount = (int)((areaSize.y-spacing) / (colourBoxSize + spacing));
        
            float xRemainder = (spacing + (spacing + colourBoxSize) * xCount - areaSize.x);
            float yRemainder = (spacing + (spacing + colourBoxSize) * yCount - areaSize.y);

            var colours = GetColours(xCount, yCount);
            
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
        private List<Color> GetColours(int x, int y) {
            List<Color> colours = new List<Color>();
            float saturationMin = 0.5f;
            float saturationMax = 1;
            for (int i = x; i > 0; i--) {
                for (int j = y; j > 0; j--) {
                    float satRatio = (float)(i) / x;
                    
                    float saturation = (satRatio * (saturationMax- saturationMin))+saturationMin;
                    
                    
                    float hue = (float)(j) / y;
                    colours.Add(Color.HSVToRGB(hue, saturation, 1));
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
                _colourPicker.selectedColour = _colour;
                _colourPicker.ChangeBackgroundColour();
                
            }
        }
    }
}
