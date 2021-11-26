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
            var path = String.Format("D:\\Projects\\CSharp\\Audio\\{0}.mp3", mark.Id);
            FileStream file = File.Open(path, FileMode.Open);
            return file;
        }
    }
}
