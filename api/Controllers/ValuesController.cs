using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ValuesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<IDictionary<string, object>>> Get()
        {
            var query = "SELECT Id, Name, Quantity FROM Inventory";
            using (var cn = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                cn.Open();
                using (var a = new SqlDataAdapter(query, cn))
                {
                    var dt = new DataTable();
                    a.Fill(dt);

                    var data = new List<Dictionary<string, object>>();
                    foreach (DataRow row in dt.Rows)
                    {
                        var rowDict = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            rowDict.Add(col.ColumnName, row[col.ColumnName]);
                        }
                        data.Add(rowDict);
                    }
                    return data;
                }
            }
        }
    }
}
