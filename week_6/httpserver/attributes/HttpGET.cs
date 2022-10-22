using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpserver.attributes
{

    internal class HttpGET : Attribute
    {
        public string MethodURI;
        public HttpGET(string methodURI)
        {
            MethodURI = methodURI;
        }
    }
}

