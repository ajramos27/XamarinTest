using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecordApp.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {

        private bool hasLocationPermissions = false;
        public MapPage()
        {
            InitializeComponent();
            GetPermissions();
        }

        private async void GetPermissions()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status == PermissionStatus.Granted)
                {
                    locationsMap.IsShowingUser = true;
                    hasLocationPermissions = true;
                    GetLocation();

                } else
                {
                    await DisplayAlert("Location denied", "Please allow access to location", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (hasLocationPermissions)
            {
                var locator = CrossGeolocator.Current;
                locator.PositionChanged += Locator_PositionChanged;
                await locator.StartListeningAsync(TimeSpan.Zero, 100);

                var position = await locator.GetPositionAsync();

                var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
                var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
                locationsMap.MoveToRegion(span);

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Post>(); //Nothing if it is already created
                    var posts = conn.Table<Post>().ToList(); //return table query object

                    DisplayInMap(posts);
                };
            }
            
            //GetLocation();
            
           
        }

        private void DisplayInMap(List<Post> posts)
        {

            foreach (var post in posts)
            {
                try
                {
                    var position = new Xamarin.Forms.Maps.Position(post.Latitude, post.Longitude);

                    var pin = new Xamarin.Forms.Maps.Pin()
                    {
                        Type = Xamarin.Forms.Maps.PinType.SavedPin,
                        Position = position,
                        Label = post.VenueName,
                        Address = post.Address
                    };

                    locationsMap.Pins.Add(pin);
                }
                catch (NullReferenceException nre) { }
                catch (Exception ex) { }
                
            }
            
            
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            //Stop and unsuscribed
            CrossGeolocator.Current.StopListeningAsync();
            CrossGeolocator.Current.PositionChanged -= Locator_PositionChanged;
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            MoveMap(e.Position);
        }

        private async void GetLocation()
        {
            if (hasLocationPermissions)
            {
                var locator = CrossGeolocator.Current;
                //var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                //var location = await Geolocation.GetLocationAsync(request);

                var position = await locator.GetPositionAsync();
                MoveMap(position);
                
            }
           
        }
        
        private void MoveMap(Position position)
        {
            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            locationsMap.MoveToRegion(new Xamarin.Forms.Maps.MapSpan(center, 2, 2));
        }
    }
}