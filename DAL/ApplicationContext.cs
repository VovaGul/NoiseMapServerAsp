using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Marker> Markers { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            SetDefaultSeed();
        }

        private void SetDefaultSeed()
        {
            var defaultMarkers = new List<Marker>
            {
                new Marker
                {
                    MarkerType = MarkerType.Checked,
                    Сoordinate = new Coordinate
                    { 
                        X = "-122.09702256272297",
                        Y = "37.40292274053587"
                    }
                },
                new Marker
                {
                    MarkerType = MarkerType.Empty,
                    Сoordinate = new Coordinate
                    {
                        X = "-110.09702256272297",
                        Y = "37.40292274053587"
                    }
                }
            };

            foreach (var defaultMarker in defaultMarkers)
            {
                Markers.Add(defaultMarker);
                SaveChanges();
            }

            var a = Markers.ToList();
        }
    }
}
