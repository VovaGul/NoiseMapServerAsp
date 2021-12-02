using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Marker> Markers { get; set; }

        //public ApplicationContext(DbContextOptions<ApplicationContext> options)
        //    : base(options)
        //{
        //}

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
                    MarkerType = MarkerType.Checked,
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
                    Title = "Cinema",
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
