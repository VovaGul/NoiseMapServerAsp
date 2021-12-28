using BAL.Models;
using DAL;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BAL
{
    public class MarkersSeed
    {
        /// <summary>
        /// порядок чисел у координаты после запятой
        /// </summary>
        private static readonly double OrderNumbers = Math.Pow(10, 6);
        private static readonly Random random = new();

        private readonly Square _bound; 
        private readonly ApplicationContext _applicationContext;

        public MarkersSeed(ApplicationContext applicationContext, Square bound)
        {
            _applicationContext = applicationContext;
            _bound = bound;
        }

        public void SetSeed(int markerCount)
        {
            var defaultMarkers = GetMarkers(markerCount);

            _applicationContext.Database.EnsureDeleted();
            _applicationContext.Database.EnsureCreated();
            _applicationContext.Markers.AddRange(defaultMarkers);
            _applicationContext.SaveChanges();
        }

        private List<Marker> GetMarkers(int markerCount)
        {
            return GetMarkerCoordinates(markerCount)
                .Select(markerCoordinate => new Marker
                {
                    X = markerCoordinate.X.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Y = markerCoordinate.Y.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    MarkerType = GetRandomMarkerType()
                })
                .ToList();
        }

        private List<Coordinate> GetMarkerCoordinates(int markerCount)
        {
            var minX = ToIntCoordinate(_bound.StartCoordinate.X);
            var maxX = ToIntCoordinate(_bound.EndCoordinate.X);

            var minY = ToIntCoordinate(_bound.EndCoordinate.Y);
            var maxY = ToIntCoordinate(_bound.StartCoordinate.Y);

            return Enumerable
                .Range(0, markerCount)
                .Select(_ => new Coordinate
                {
                    X = ToDoubleCoordinate(random.Next(minX, maxX)),
                    Y = ToDoubleCoordinate(random.Next(minY, maxY))
                })
                .ToList();
        }

        private static int ToIntCoordinate(double doubleCoordinate)
        {
            return (int)(doubleCoordinate * OrderNumbers);
        }

        private static double ToDoubleCoordinate(int intCoordinate)
        {
            return intCoordinate / OrderNumbers;
        }

        private static MarkerType GetRandomMarkerType()
        {
            return (MarkerType)random.Next(0, 3);
        }
    }
}
