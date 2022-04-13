using System;
using System.Collections.Generic;
using Code._Cargo;
using Code._Cargo.ProductTypes.Commodity;
using Code._Cargo.ProductTypes.Ships;
using Code._Galaxy._SolarSystem._CelestialObjects.Stations.StationServices;
using Code._GameControllers;
using Code._Ships.Controllers;
using Code._Ships.ShipComponents.InternalComponents.Storage;
using Code._Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GUI.SpaceStations.Services {
    public class TradeGUIController : MonoBehaviour {
        private TradeService _tradeService;
        private StationGUIController _stationGUIController;
        private GameObject _guiGameObject;
        private GameObject _availableContainer;
        public List<ProductCard> _productCards;
        private CargoController _cargoController;
        private Vector2 _indicatorBarDefaultSize; 

        public void StartTradeGUI(TradeService tradeService, StationGUIController stationGUIController) {
            _tradeService = tradeService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab(_tradeService.GUIPath));
            SetupButtons();
            SetupAvailableProducts();
            UpdateCargoHoldBar();
        }

        private void SetupButtons() {
            Button homeBtn = FindChildGameObject.FindChild(_guiGameObject, "HomeBtn").GetComponent<Button>();
            homeBtn.onClick.AddListener(Exit);
            
            Button purchaseBtn = FindChildGameObject.FindChild(_guiGameObject, "PurchaseButton").GetComponent<Button>();
            purchaseBtn.onClick.AddListener(PurchaseBtnClick);
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }
        
        private void UpdatePriceChange() {
            
        }

        private void UpdateCargoHoldBar() {
            _cargoController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController;
            int fuel = _cargoController.GetCargoOfType<Fuel>().Count;
            RectTransform fuelBarTransform = FindChildGameObject.FindChild(_guiGameObject, "FuelIndicator").GetComponent<RectTransform>();
            
            int stored = _cargoController.GetUsedCargoSpace() - fuel;
            RectTransform storedBarTransform = FindChildGameObject.FindChild(_guiGameObject, "StoredIndicator").GetComponent<RectTransform>();
            int buying = GetPurchasingCargoCount();
            RectTransform purchasingTransform = FindChildGameObject.FindChild(_guiGameObject, "PurchaseIndicator").GetComponent<RectTransform>();
            int totalCapacity = _cargoController.GetMaxCargoSpace();
            RectTransform freeTransform = FindChildGameObject.FindChild(_guiGameObject, "FreeIndicator").GetComponent<RectTransform>();

            if (_indicatorBarDefaultSize == new Vector2(0,0)) {
                _indicatorBarDefaultSize = freeTransform.sizeDelta;
            }

            fuelBarTransform.sizeDelta = new Vector2((float)fuel / totalCapacity* _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
            storedBarTransform.sizeDelta = new Vector2((float)(fuel + stored) / totalCapacity * _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
            purchasingTransform.sizeDelta = new Vector2((float)(fuel + stored + buying) / totalCapacity* _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
        }

        private int GetPurchasingCargoCount() {
            int cargoCount = 0;

            foreach (ProductCard productCard in _productCards) {
                cargoCount += productCard.GetCurrentValue();
            }

            return cargoCount;
        }

        private void PurchaseBtnClick() {
            List<Cargo> newCargo = new List<Cargo>();

            foreach (ProductCard productCard in _productCards) {
                int cargoCount = productCard.GetCurrentValue();
                for (int i = 0; i < cargoCount; i++) {
                    Commodity commodity = (Commodity)Activator.CreateInstance(productCard.Product.productType);
                    newCargo.Add(commodity);
                }

                var product = _tradeService.Products.Find(type => type.productType == productCard.Product.productType);
                int indexOfProduct = _tradeService.Products.IndexOf(product);
                productCard.Product.quantity -= cargoCount;
                _tradeService.Products[indexOfProduct] = productCard.Product;
                productCard.ResetCard();
            }

            _cargoController.AddCargo(newCargo);
            UpdateCargoHoldBar();
        }

        private void SetupAvailableProducts() {
            if (_availableContainer == null) {
                _availableContainer = FindChildGameObject.FindChild(_guiGameObject, "AvailableProducts");
            }

            if (_productCards == null) {
                _productCards = new List<ProductCard>();
            }

            foreach ((Type productType, int quantity, int price) product in _tradeService.Products) {
                GameObject productCardObject = GetProductCard();
                productCardObject.transform.SetParent(_availableContainer.transform);
                ProductCard productCard = productCardObject.AddComponent<ProductCard>();
                productCard.SetupCard(product, this);
                _productCards.Add(productCard);
            }
        }

        private GameObject GetProductCard() {
            return GameController._prefabHandler.InstantiateObject(GameController._prefabHandler.LoadPrefab("GUIPrefabs/Station/Services/Trade/ProductCard"));
        }

        public class ProductCard : MonoBehaviour {
            public (Type productType, int quantity, int price) Product;
            private TradeGUIController _tradeGUIController;
            private TMP_InputField _txtIntInput;
            

            public void SetupCard((Type productType, int quantity, int price) product, TradeGUIController tradeGUIController) {//split into 2 classes inheriting from this for ship cargo with List of products instead of Type and quantity
                Product = product;
                _tradeGUIController = tradeGUIController;
                
                Commodity commodity = (Commodity)Activator.CreateInstance(product.productType);
                
                TextMeshProUGUI nameText = FindChildGameObject.FindChild(gameObject, "ProductName").GetComponent<TextMeshProUGUI>();
                nameText.text = commodity.Name;
                
                UpdateAvailable();
                
                TextMeshProUGUI priceText = FindChildGameObject.FindChild(gameObject, "PriceValue").GetComponent<TextMeshProUGUI>();
                priceText.text = product.price + " Cr";

                _txtIntInput = FindChildGameObject.FindChild(gameObject, "PurchaseAmountInput").GetComponent<TMP_InputField>();
                _txtIntInput.onValueChanged.AddListener(ParseTxtInput);
                SetupButtons();
            }

            private void UpdateAvailable() {
                TextMeshProUGUI availableText = FindChildGameObject.FindChild(gameObject, "AvailableValue").GetComponent<TextMeshProUGUI>();
                availableText.text = Product.quantity.ToString();
            }

            private void SetupButtons() {
                Button plusBtn = FindChildGameObject.FindChild(gameObject, "PlusBtn").GetComponent<Button>();
                plusBtn.onClick.AddListener(delegate { PlusBtnClick(); });

                Button minusBtn = FindChildGameObject.FindChild(gameObject, "MinusBtn").GetComponent<Button>();
                minusBtn.onClick.AddListener(delegate { MinusBtnClick(); });
            }

            private void ParseTxtInput(string text) {
                //ensures value is not too large or small upon changing
                int value = Int32.Parse(text);
                value = value < 0 ? 0 : value > Product.quantity ? Product.quantity : value;//if less than 0 set to 0 - if larger than quantity of products available set to quantity of products
                
                //check if there is enough cargo space & if other purchases will also fit
                int freeCargoSpace = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController.GetFreeCargoSpace();
                int qtyOtherProductsPurchase = 0;
                List<ProductCard> productCards = _tradeGUIController._productCards; 
                for (int i = 0; i < productCards.Count; i++) {
                    if (productCards[i] != this) {
                        qtyOtherProductsPurchase += productCards[i].GetCurrentValue();
                    }
                }

                value = freeCargoSpace - qtyOtherProductsPurchase - value > 0 ? value : freeCargoSpace - qtyOtherProductsPurchase;//ensures the cargo won't be overfilled
                
                _txtIntInput.text = value.ToString();
                _tradeGUIController.UpdateCargoHoldBar();
            }

            public int GetCurrentValue() {
                return Int32.Parse(_txtIntInput.text);
            }

            public void ResetCard() {
                UpdateAvailable();
                _txtIntInput.text = 0.ToString();
            }

            public void PlusBtnClick() {
                //get new value
                //shift to increase by 50 - Ctrl to add all - regular click to inc by 1
                int newValue = Input.GetKey(KeyCode.LeftShift) ? GetCurrentValue() + 50 : Input.GetKey(KeyCode.LeftControl) ? Product.quantity : GetCurrentValue() + 1;
                _txtIntInput.text = newValue.ToString();
            }

            public void MinusBtnClick() {
                //get new value
                //shift to decrease by 50 - Ctrl to remove all - regular click to dec by 1
                int newValue = Input.GetKey(KeyCode.LeftShift) ? GetCurrentValue() - 50 : Input.GetKey(KeyCode.LeftControl) ? 0 : GetCurrentValue() - 1;
                _txtIntInput.text = newValue.ToString();
            }
        }
    }
}