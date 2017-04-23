using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApi
{
    public interface IImageHandler
    {
        byte[] GetLastImageFromStream(int index);
    }
}
