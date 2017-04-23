using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApi.Exceptions
{
    public class FaceNotFoundException : Exception
    {
        public FaceNotFoundException()
             : base("Face not found")
        {
        }
        public FaceNotFoundException(string message)
             : base("Face not found")
        {
        }
        public FaceNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
