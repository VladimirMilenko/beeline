using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApi.Exceptions
{
    public class FacesApiException : Exception
    {
        public FacesApiException()
             : base("Something went wrong")
        {
        }
        public FacesApiException(string message)
             : base("Something went wrong")
        {
        }
        public FacesApiException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
