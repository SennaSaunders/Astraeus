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
        private GameObject _ownedContainer;

        private List<(Type productType, int quantity, int price)> _availableProducts;

        private List<ProductCard> _availableProductCards;
        private List<ProductCard> _ownedProductCards;
        private CargoController _cargoController;
        private Vector2 _indicatorBarDefaultSize;
        private GameObject notEnoughCreditsMsg;
        private float creditMsgTime = 3;
        private float creditMsgCountdown = 0;

        public void StartTradeGUI(TradeService tradeService, StationGUIController stationGUIController) {
            _tradeService = tradeService;
            _stationGUIController = stationGUIController;
            SetupGUI();
        }

        private void Update() {
            if (notEnoughCreditsMsg != null) {
                if (notEnoughCreditsMsg.activeSelf) {
                    creditMsgCountdown -= Time.deltaTime;
                    if (creditMsgCountdown <= 0) {
                        notEnoughCreditsMsg.SetActive(false);
                    }
                }
            }
        }

        private void SetupGUI() {
            _stationGUIController.stationGUI.SetActive(false);
            _guiGameObject = Instantiate((GameObject)Resources.Load(_tradeService.GUIPath));
            _cargoController = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController;
            notEnoughCreditsMsg = GameObjectHelper.FindChild(_guiGameObject, "NotEnoughCredits");
            notEnoughCreditsMsg.SetActive(false);
            SetupButtons();
            SetupProducts();
        }

        private void SetupAvailableProducts() {
            _availableProducts = new List<(Type productType, int quantity, int price)>();

            foreach ((Type productType, int quantity, int price) product in _tradeService.Products) {
                _availableProducts.Add(product);
            }
        }

        private void SetupProducts() {
            SetupAvailableProducts();
            SetupAvailableProductCards();
            SetupOwnedProductCards();
            UpdateCargoHoldBar();
            UpdateCredits();
        }

        private void UpdateCredits() {
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsCurrentValue", GameController.PlayerProfile._credits.ToString());
            GameObjectHelper.SetGUITextValue(_guiGameObject, "CreditsChangeValue", GetChangeInFunds().ToString());
        }

        private int GetChangeInFunds() {
            //cost of purchases
            int purchaseCost = 0;
            foreach (ProductCard productCard in _availableProductCards) {
                purchaseCost += productCard.GetCurrentValue() * productCard.Product.price;
            }

            //money made selling
            int saleValue = 0;
            foreach (ProductCard productCard in _ownedProductCards) {
                saleValue += productCard.GetCurrentValue() * productCard.Product.price;
            }

            return saleValue - purchaseCost;
        }

        private void SetupAvailableProductCards() {
            if (_availableContainer == null) {
                _availableContainer = GameObjectHelper.FindChild(_guiGameObject, "AvailableProducts");
            }

            _availableProductCards = new List<ProductCard>();

            foreach ((Type productType, int quantity, int price) product in _availableProducts) {
                if (product.quantity > 0) {
                    ProductCard productCard = _availableProductCards.Find(apc => product == apc.Product);
                    if (productCard == null) {
                        GameObject productCardObject = GetProductCardObject();
                        productCardObject.transform.SetParent(_availableContainer.transform, false);
                        productCard = productCardObject.AddComponent<ProductCard>();
                        productCard.SetupCard(product, this, false);
                        _availableProductCards.Add(productCard);
                    }
                }
            }
        }

        private void SetupOwnedProductCards() {
            if (_ownedContainer == null) {
                _ownedContainer = GameObjectHelper.FindChild(_guiGameObject, "CargoProducts");
            }

            _ownedProductCards = new List<ProductCard>();

            List<Type> cargoTypes = _cargoController.GetCargoTypes();
            foreach (Type cargoType in cargoTypes) {
                if (cargoType != typeof(Fuel)) {
                    GameObject productCardObject = GetProductCardObject();
                    productCardObject.transform.SetParent(_ownedContainer.transform, false);
                    ProductCard productCard = productCardObject.AddComponent<ProductCard>();
                    int quantity = _cargoController.GetCargoOfType(cargoType).Count;
                    int price = _availableProducts.Find(product => product.productType == cargoType).price;
                    productCard.SetupCard((cargoType, quantity, price), this, true);
                    _ownedProductCards.Add(productCard);
                }
            }
        }

        private GameObject GetProductCardObject() {
            return Instantiate((GameObject)Resources.Load("GUIPrefabs/Station/Services/Trade/ProductCard"));
        }

        private void SetupButtons() {
            Button exitBtn = GameObjectHelper.FindChild(_guiGameObject, "ExitBtn").GetComponent<Button>();
            exitBtn.onClick.AddListener(Exit);

            Button resetBtn = GameObjectHelper.FindChild(_guiGameObject, "ResetButton").GetComponent<Button>();
            resetBtn.onClick.AddListener(ResetTrade);

            Button purchaseBtn = GameObjectHelper.FindChild(_guiGameObject, "PurchaseButton").GetComponent<Button>();
            purchaseBtn.onClick.AddListener(PurchaseBtnClick);
        }

        private void ResetTrade() {
            RecreateCards();
        }

        private void Exit() {
            _stationGUIController.stationGUI.SetActive(true);
            Destroy(_guiGameObject);
            Destroy(this);
        }

        private void UpdateCargoHoldBar() {
            int fuel = _cargoController.GetCargoOfType(typeof(Fuel)).Count;
            RectTransform fuelBarTransform = GameObjectHelper.FindChild(_guiGameObject, "FuelIndicator").GetComponent<RectTransform>();

            int stored = _cargoController.GetUsedCargoSpace() - fuel;
            int selling = GetProductCardsCargoCount(_ownedProductCards);
            RectTransform storedBarTransform = GameObjectHelper.FindChild(_guiGameObject, "StoredIndicator").GetComponent<RectTransform>();

            int buying = GetProductCardsCargoCount(_availableProductCards);
            RectTransform purchasingTransform = GameObjectHelper.FindChild(_guiGameObject, "PurchaseIndicator").GetComponent<RectTransform>();
            int totalCapacity = _cargoController.GetMaxCargoSpace();
            RectTransform freeTransform = GameObjectHelper.FindChild(_guiGameObject, "FreeIndicator").GetComponent<RectTransform>();

            if (_indicatorBarDefaultSize == new Vector2(0, 0)) {
                _indicatorBarDefaultSize = freeTransform.sizeDelta;
            }

            int barX = fuel;
            fuelBarTransform.sizeDelta = new Vector2((float)barX / totalCapacity * _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
            barX += stored - selling;
            storedBarTransform.sizeDelta = new Vector2((float)barX / totalCapacity * _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
            barX += buying;
            purchasingTransform.sizeDelta = new Vector2((float)barX / totalCapacity * _indicatorBarDefaultSize.x, _indicatorBarDefaultSize.y);
        }

        private int GetProductCardsCargoCount(List<ProductCard> productCards) {
            int cargoCount = 0;

            foreach (ProductCard productCard in productCards) {
                cargoCount += productCard.GetCurrentValue();
            }

            return cargoCount;
        }

        private void PurchaseBtnClick() {
            if (GameController.PlayerProfile.AddCredits(GetChangeInFunds())) {
                List<Cargo> newCargo = new List<Cargo>();
                List<Cargo> cargoToSell = new List<Cargo>();

                foreach (ProductCard productCard in _availableProductCards) {
                    int cargoCount = productCard.GetCurrentValue();
                    for (int i = 0; i < cargoCount; i++) {
                        Commodity commodity = (Commodity)Activator.CreateInstance(productCard.Product.productType);
                        newCargo.Add(commodity);
                    }

                    var product = _tradeService.Products.Find(type => type.productType == productCard.Product.productType);
                    int indexOfProduct = _tradeService.Products.IndexOf(product);

                    productCard.Product.quantity -= cargoCount;
                    _tradeService.Products[indexOfProduct] = productCard.Product;
                }


                foreach (ProductCard productCard in _ownedProductCards) {
                    int cargoCount = productCard.GetCurrentValue();
                    cargoToSell.AddRange(_cargoController.GetCargoOfType(productCard.Product.productType, cargoCount));

                    var productIndex = _tradeService.Products.FindIndex(product => product.productType == productCard.Product.productType);
                    var product = _availableProducts[productIndex];
                    product.quantity += cargoCount;
                    _tradeService.Products[productIndex] = product;
                }

                _cargoController.RemoveCargo(cargoToSell);
                _cargoController.AddCargo(newCargo);

                RecreateCards();
            }
            else {
                NotEnoughCredits();
            }
        }

        private void NotEnoughCredits() {
            notEnoughCreditsMsg.SetActive(true);
            creditMsgCountdown = creditMsgTime;
        }

        private void RecreateCards() {
            for (int i = _availableProductCards.Count-1; i >= 0;i--) {
                Destroy(_availableProductCards[i].gameObject);
                Destroy(_availableProductCards[i]);
            }

            for (int i = _ownedProductCards.Count-1;i >= 0;i--) {
                Destroy(_ownedProductCards[i].gameObject);
            }

            SetupProducts();
        }
        
        

        public class ProductCard : MonoBehaviour {
            public (Type productType, int quantity, int price) Product;
            private TradeGUIController _tradeGUIController;
            private TMP_InputField _txtIntInput;
            private bool _isSellCard;


            public void SetupCard((Type productType, int quantity, int price) product, TradeGUIController tradeGUIController, bool isSellCard) { //split into 2 classes inheriting from this for ship cargo with List of products instead of Type and quantity
                Product = product;
                _tradeGUIController = tradeGUIController;
                _isSellCard = isSellCard;
                Commodity commodity = (Commodity)Activator.CreateInstance(product.productType);
                GameObjectHelper.SetGUITextValue(gameObject, "ProductName", commodity.Name);
                GameObjectHelper.SetGUITextValue(gameObject, "PriceValue", product.price + " Cr");
                UpdateAvailableValue();

                _txtIntInput = GameObjectHelper.FindChild(gameObject, "PurchaseAmountInput").GetComponent<TMP_InputField>();
                _txtIntInput.onValueChanged.AddListener(ParseTxtInput);
                SetupButtons();
            }

            private void UpdateAvailableValue() {
                GameObjectHelper.SetGUITextValue(gameObject, "AvailableValue", Product.quantity.ToString());
            }

            private void SetupButtons() {
                Button plusBtn = GameObjectHelper.FindChild(gameObject, "PlusBtn").GetComponent<Button>();
                plusBtn.onClick.AddListener(delegate { PlusBtnClick(); });

                Button minusBtn = GameObjectHelper.FindChild(gameObject, "MinusBtn").GetComponent<Button>();
                minusBtn.onClick.AddListener(delegate { MinusBtnClick(); });
            }

            private void ParseTxtInput(string text) {
                int qtyPurchaseProducts = _tradeGUIController.GetProductCardsCargoCount(_tradeGUIController._availableProductCards);
                int qtySoldProducts = _tradeGUIController.GetProductCardsCargoCount(_tradeGUIController._ownedProductCards);
                
                //ensures value is not too large or small upon changing
                int value = Int32.Parse(text);
                value = value < 0 ? 0 : value > Product.quantity ? Product.quantity : value;//stops negative values && values larger than quantity of products

                int freeCargoSpace = GameController.CurrentShip.ShipObject.GetComponent<ShipController>().CargoController.GetFreeCargoSpace();
                
                //ensures cargo won't be overfilled when buying
                if (!_isSellCard) {
                    // int qtyOtherPurchaseProducts = qtyPurchaseProducts-GetCurrentValue();
                    int qtyOtherPurchaseProducts = qtyPurchaseProducts-GetCurrentValue();
                    value = freeCargoSpace - qtyOtherPurchaseProducts - value + qtySoldProducts >= 0 ? value : freeCargoSpace - qtyOtherPurchaseProducts + qtySoldProducts;//ensures purchase fits in cargo bays
                }
                else {
                    int qtyOtherSoldProducts = qtySoldProducts -GetCurrentValue();
                    value = freeCargoSpace - qtyPurchaseProducts + qtyOtherSoldProducts + value >= 0 ? value : -(freeCargoSpace - qtyPurchaseProducts + qtyOtherSoldProducts);
                }

                _txtIntInput.text = value.ToString();
                _tradeGUIController.UpdateCargoHoldBar();
                _tradeGUIController.UpdateCredits();
            }

            public int GetCurrentValue() {
                return Int32.Parse(_txtIntInput.text);
            }

            private void PlusBtnClick() {
                //get new value
                //shift to increase by 50 - Ctrl to add all - regular click to inc by 1
                int newValue = Input.GetKey(KeyCode.LeftShift) ? GetCurrentValue() + 50 : Input.GetKey(KeyCode.LeftControl) ? Product.quantity : GetCurrentValue() + 1;
                _txtIntInput.text = newValue.ToString();
            }

            private void MinusBtnClick() {
                //get new value
                //shift to decrease by 50 - Ctrl to remove all - regular click to dec by 1
                int newValue = Input.GetKey(KeyCode.LeftShift) ? GetCurrentValue() - 50 : Input.GetKey(KeyCode.LeftControl) ? 0 : GetCurrentValue() - 1;
                _txtIntInput.text = newValue.ToString();
            }
        }
    }
}