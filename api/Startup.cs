using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void Preconfigure() {
            Console.WriteLine("Preparing to connect to sql");

            var sql = "SELECT Name FROM sys.Databases";

            using (var cn = new SqlConnection(Configuration.GetConnectionString("Master")))
            {
                cn.Open();
                using (var cmd = new SqlCommand(sql, cn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == "TestDB")
                                return;
                        }
                    }
                }

                var makeDatabase = "CREATE DATABASE TestDB";
                using (var cmd = new SqlCommand(makeDatabase, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            using (var cn = new SqlConnection(Configuration.GetConnectionString("Default")))
            {
                cn.Open();
                var makeTable = "CREATE TABLE Inventory(Id int identity(1,1) primary key, Name nvarchar(100) not null unique, Quantity decimal(19,4) not null)";
                using (var cmd = new SqlCommand(makeTable, cn))
                {
                    cmd.ExecuteNonQuery();
                }

                var insert = "INSERT INTO Inventory(Name, Quantity) VALUES(@Name, @Quantity)";
                var names = new string[] { "Burgers", "Cookies", "Milk" };
                var quantities = new decimal[] { 10.0M, 100M, 25.5M };

                for (var i = 0; i < 3; i++)
                {
                    using (var insertCmd = new SqlCommand(insert, cn))
                    {
                        insertCmd.Parameters.AddWithValue("Name", names[i]);
                        insertCmd.Parameters.AddWithValue("Quantity", quantities[i]);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Preconfigure();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
