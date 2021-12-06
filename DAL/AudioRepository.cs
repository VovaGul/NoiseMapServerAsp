using DAL.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AudioRepository
    {
        public Stream GetAudio(Marker mark)
        {
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "Audio", $"{mark.Id}.mp3");
            FileStream file = File.Open(path, FileMode.Open);
            return file;
        }
    }
}
