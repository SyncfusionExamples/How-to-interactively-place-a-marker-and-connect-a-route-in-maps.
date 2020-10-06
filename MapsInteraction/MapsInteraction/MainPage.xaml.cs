using Syncfusion.SfMaps.XForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MapsInteraction
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private int markerSelectedIndex;
        private int zoomLevel;
        public MainPage()
        {
            InitializeComponent();
            markerSelectedIndex = -1;
            zoomLevel = 1;
        }
        private void maps_Tapped(object sender, Syncfusion.SfMaps.XForms.MapTappedEventArgs e)
        {
            if (!editing.IsToggled)
            {
                var screenPoint = e.Position;
                var geoPoint = layer.GetLatLonFromPoint(screenPoint);
                MapMarker marker = new MapMarker();
                marker.Latitude = geoPoint.Y.ToString();
                marker.Longitude = geoPoint.X.ToString();
                layer.Markers.Add(marker);
                latitudeEntry.Text = Math.Round(geoPoint.Y, 2).ToString();
                longitudeEntry.Text = Math.Round(geoPoint.X, 2).ToString();
                subLayer.Points.Add(new Point(geoPoint.Y, geoPoint.X));
            }
        }

        private void maps_Panning(object sender, Syncfusion.SfMaps.XForms.MapPanUpdatedEventArgs e)
        {
            if (maps.EnablePanning) return;
            var screenPoint = e.Position;
            var geoPoint = layer.GetLatLonFromPoint(screenPoint);
            latitudeEntry.Text = Math.Round(geoPoint.Y, 2).ToString();
            longitudeEntry.Text = Math.Round(geoPoint.X, 2).ToString();

            if (e.Started && editing.IsToggled)
            {
                markerSelectedIndex = CalculateNearestMarker(geoPoint, screenPoint);
            }

            if (editing.IsToggled && markerSelectedIndex != -1 && !e.Started && !e.Completed)
            {
                if (layer.Markers.Count > markerSelectedIndex)
                {
                    layer.Markers[markerSelectedIndex].Latitude = geoPoint.Y.ToString();
                    layer.Markers[markerSelectedIndex].Longitude = geoPoint.X.ToString();
                }
                if (subLayer.Points.Count > markerSelectedIndex)
                {
                    subLayer.Points[markerSelectedIndex] = new Point(geoPoint.Y, geoPoint.X);
                }
            }
            if (e.Completed)
            {
                markerSelectedIndex = -1;
            }
        }

        private int CalculateNearestMarker(Point geoPoint, Point screenPoint)
        {
            int markerIndex = -1;
            var x = geoPoint.Y;
            var y = geoPoint.X;

            foreach (var marker in layer.Markers)
            {
                int extraBounds = (10 / zoomLevel);
                var x1 = Convert.ToDouble(marker.Latitude) - extraBounds;
                var x2 = Convert.ToDouble(marker.Latitude) + extraBounds;

                var y1 = Convert.ToDouble(marker.Longitude) - extraBounds;
                var y2 = Convert.ToDouble(marker.Longitude) + extraBounds;

                if (x1 < x && x < x2 && y1 < y && y < y2)
                {
                    markerIndex = layer.Markers.IndexOf(marker);
                }
            }

            return markerIndex;
        }

        private void layer_ZoomLevelChanging(object sender, ZoomLevelChangingEventArgs e)
        {
            zoomLevel = e.CurrentLevel;
        }
    }
}
