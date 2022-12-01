using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Org.BouncyCastle.Utilities;
using PerrijosGatijos.Models.Class.MockMapModel;
using Plugin.Geolocator;
using System.Linq;
using Xamarin.Forms.Maps;

namespace PerrijosGatijos.ViewModels
{
    public class MapViewModel: BaseViewModel
    {
        #region Properties
        private Position _myPosition= new Position(19.3188895, -99.10986379999997);
        CoffeeShop cafe;

        private ObservableCollection<Position> _ruta = new ObservableCollection<Position>();

        public ObservableCollection<Position> Ruta
        {
            get { return _ruta; }
            set
            {
                _ruta = value;
                OnPropertyChanged("Ruta");
            }
        }

        private ObservableCollection<double> _longitudes = new ObservableCollection<double>();

        public ObservableCollection<double> Longitudes
        {
            get { return _longitudes; }
            set
            {
                _longitudes = value;
                OnPropertyChanged("Longitudes");
            }
        }

        private ObservableCollection<double> _latitudes = new ObservableCollection<double>();

        public ObservableCollection<double> Latitudes
        {
            get { return _latitudes; }
            set
            {
                _latitudes = value;
                OnPropertyChanged("Latitudes");
            }
        }

        public Position MyPosition
        {
            get
            {
                return _myPosition;
            }

            set
            {
                SetProperty(ref _myPosition, value);
                OnPropertyChanged("MyPosition");
            }
        }

        private Map _map;

        public Map Map
        {
            get
            {
                return _map;
            }

            set
            {
                SetProperty(ref _map, value);
                OnPropertyChanged("Map");
            }
        }

        private ObservableCollection<Pin> _allPines = new ObservableCollection<Pin>();

        public ObservableCollection<Pin> AllPines
        {
            get
            {
                return _allPines;
            }

            set
            {
                SetProperty(ref _allPines, value);
                OnPropertyChanged("AllPines");
            }
        }

        private ObservableCollection<CoffeeShop> _items;

        public ObservableCollection<CoffeeShop> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        #endregion

        public MapViewModel()
        {
            LoadMapData();
        }


        public async void LoadMapData()
        {
            IsBusy = true;
            var locator = await CrossGeolocator.Current.GetPositionAsync();
            MyPosition = new Position(locator.Latitude, locator.Longitude);
            GetVeterinians();
            //AllPines.Add(new Pin
            //{
            //    Position = new Position(locator.Latitude, locator.Longitude),
            //    Type = PinType.Place,
            //    Label = ""
            //});
            //Map.MoveToRegion(new MapSpan(MyPosition, 0.01, 0.01));
            IsBusy = false;
        }

        public void GetVeterinians()
        {
            //var pins = new List<Pin>
            //{
            //    new Pin
            //    {
            //         Type=PinType.Place,
            //         Label= "This is my home",
            //         Address = "Here",
            //         Position = new Position(-23.68, -46.87)
            //    },
            //    new Pin
            //    {
            //         Type=PinType.Place,
            //         Label= "This is my home",
            //         Address = "Here",
            //         Position = new Position(-23.68, -46.77)
            //    },
            //    new Pin
            //    {
            //         Type=PinType.Place,
            //         Label= "This is my home",
            //         Address = "Here",
            //         Position = new Position(-23.68, -46.97)
            //    },
            //};

            //foreach (var pin in pins)
            //{
            //    AllPines.Add(pin);
            //}

            Items = new ObservableCollection<CoffeeShop>
            {
                new CoffeeShop
                {
                    Id = 1,
                    Name = "Viva Espresso San Benito",
                    Description = "Centro Comercial El Hipodromo 503, San Benito",
                    Latitude = 13.6946923,
                    Longitude = -89.2414103,
                    Rate = 5
                },
                new CoffeeShop
                {
                    Id = 2,
                    Name = "The Coffee Cup Masferrer",
                    Description = "Rendondel Masferrer, Colonia Escalon",
                    Latitude = 13.703869,
                    Longitude = -89.248569,
                    Rate = 5
                },
                new CoffeeShop
                {
                    Id = 3,
                    Name = "El Cafecito Bistro",
                    Description = "Fundacion Emprendedores Por El Mundo, Paseo El Carmen",
                    Latitude = 13.67534,
                    Longitude = -89.2868771,
                    Rate = 5
                }
            };

        }
    }
}

