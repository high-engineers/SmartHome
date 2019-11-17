using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.API
{
    public class ValuesController
    {
        [HttpGet("api/values")]
        public IList<string> Get()
        {
            return new List<string> { "walju1", "walju2" };
        }
    }
}
