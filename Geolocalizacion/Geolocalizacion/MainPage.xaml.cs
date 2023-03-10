using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Geolocalizacion
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource cts;
        ObservableCollection<Geo> geoList= new ObservableCollection<Geo>();
        public MainPage()
        {
            InitializeComponent();
            Lista.ItemsSource = geoList;
        }

        public ObservableCollection<Geo> GeoList { get { return geoList; } }

        async Task GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    latitud.Text = location.Latitude.ToString();
                    longitud.Text = location.Longitude.ToString();
                    geoList.Add(new Geo { latitud=location.Latitude, longitud=location.Longitude });
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await DisplayAlert("Error",fnsEx.Message,"Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                await DisplayAlert("Error", fneEx.Message, "Ok");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                await DisplayAlert("Error", pEx.Message, "Ok");
            }
            catch (Exception ex)
            {
                // Unable to get location
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await GetCurrentLocation();
        }

        private void Clean_Clicked(object sender, EventArgs e)
        {
            latitud.Text = "";
            longitud.Text = "";
            geoList.Clear();
        }
    }
}
