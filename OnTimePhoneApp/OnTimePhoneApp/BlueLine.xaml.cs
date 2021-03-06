﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace OnTimePhoneApp
{
    public static class CoordinateConverterBlue
    {
        public static GeoCoordinate ConvertGeocoordinateBlue(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }
    }

    public partial class BlueLine : PhoneApplicationPage
    {
        const string blue946 = "http://realtime.mbta.com/developer/api/v1/stopsbyroute?api_key=ePbnXz1wU0Oru5ApeL_mqA&route=946_";
        public BlueLine()
        {
            InitializeComponent();
            map.SetView(new GeoCoordinate(42.3797, -71.0230, 150), 13);
            Loaded += BlueLine_Loaded;
        }

        private async void ShowMyLocationOnTheMap()
        {
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = CoordinateConverterBlue.ConvertGeocoordinateBlue(myGeocoordinate);
            this.map.Center = myGeoCoordinate;
            this.map.ZoomLevel = 17;
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Red);
            myCircle.Height = 15;
            myCircle.Width = 15;
            myCircle.Opacity = 90;
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = myGeoCoordinate;
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);
            map.Layers.Add(myLocationLayer);
        }
        private void BlueLine_Loaded(object sender, RoutedEventArgs e)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri(blue946));
        }
        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var rootObject = JsonConvert.DeserializeObject<Rootobject>(e.Result);
            double lat, longitude;
            MapPolyline line = new MapPolyline();
            line.StrokeColor = Colors.Blue;
            line.StrokeThickness = 2;

            double[] coord = new double[2 * rootObject.direction[0].stop.Length];
            for (int i = 0; i < rootObject.direction[0].stop.Length; i++)
            {
                lat = Convert.ToDouble(rootObject.direction[0].stop[i].stop_lat);
                longitude = Convert.ToDouble(rootObject.direction[0].stop[i].stop_lon);

                line.Path.Add(new GeoCoordinate(lat, longitude));

                Ellipse myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Blue);
                myCircle.Height = 10;
                myCircle.Width = 10;
                myCircle.Opacity = 60;
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                myLocationOverlay.GeoCoordinate = new GeoCoordinate(lat, longitude, 200);
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);
                map.Layers.Add(myLocationLayer);
            }
            map.MapElements.Add(line);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowMyLocationOnTheMap();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (map.ZoomLevel < 19)
            {
                map.ZoomLevel++;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (map.ZoomLevel > 2)
            {
                map.ZoomLevel--;
            }
        }

    }
}