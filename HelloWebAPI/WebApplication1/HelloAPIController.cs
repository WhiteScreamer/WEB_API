using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WebApplication1
{
    public class HelloAPIController: ApiController
    {
        public string Get()
        {
            return "Helo" + DateTime.Now.ToString();
        }
    }
}