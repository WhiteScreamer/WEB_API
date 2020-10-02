using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebMVCDemo.Controllers
{
    public class ValuesController : ApiController
    {
        static List<string> data = new List<string>() {"val1", "val2" };
        // GET api/values
        public IEnumerable<string> Get()
        {
            return data;
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            if (id < data.Count)
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, data[id]);
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "item not found");
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody]string value)
        {
            data.Add(value);
            var msg = Request.CreateResponse(HttpStatusCode.Created);
            msg.Headers.Location = new Uri(Request.RequestUri + (data.Count - 1).ToString());
            return msg;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            data[id] = value;
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            data.RemoveAt(id);
        }
    }
}
