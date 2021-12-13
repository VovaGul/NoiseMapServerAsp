using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Marker> Markers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=NoiseMap.db");
        }

        public void SetDefaultSeed()
        {
            var defaultMarkers = new List<Marker>
            {
                new Marker
                {
                    MarkerType = MarkerType.Unchecked,
                    X = "-122.09702256272297",
                    Y = "37.40292274053587",
                    Title = "Panda Express",
                    AudioStatus = AudioStatus.Recorded
                },
                new Marker
                {
                    MarkerType = MarkerType.Empty,
                    X = "-121.09702256272297",
                    Y = "37.40292274053587",
                    Title = "Cinema1",
                    AudioStatus = AudioStatus.Unrecorded
                },
                new Marker
                {
                    MarkerType = MarkerType.Checked,
                    X = "-121.09702256272297",
                    Y = "30.40292274053587",
                    Title = "Cinema2",
                    AudioStatus = AudioStatus.Unrecorded
                }
            };

            foreach (var defaultMarker in defaultMarkers)
            {
                Markers.Add(defaultMarker);
                SaveChanges();
            }
        }
    }
}
