using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Net;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using API.AutoFac;
using House.Prediction;
using Grpc.Core;
using CsvHelper;
using API.Entities;

namespace API
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; private set; }

        public IConfiguration Configuration { get; }

        public const string CorsPolicy = "Cors";

        public const string ArangoConnectionId = "_system";

        public const string SwaggerApiName = "job-api";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The return type IServiceProvider is very important to get AutoFac working. Without this returned IServiceProvider, AutoFac will fail.</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            AutoFacContainer autoFacContainer = new AutoFacContainer();

            ContainerBuilder builder = autoFacContainer.ContainerBuilder;

            Channel channel = new Channel("127.0.0.1:51666", ChannelCredentials.Insecure);

            var client = new PredictionService.PredictionServiceClient(channel);
            // regiseter the client for API usage
            builder.RegisterInstance<PredictionService.PredictionServiceClient>(client);

            // read the house indices file:

            var houseIndicesFile = autoFacContainer.Configuration.GetSection("HouseIndices").Get<string>();

            Dictionary<int, HouseIndex> houseIndices = new Dictionary<int, HouseIndex>();

            using (System.IO.StreamReader houseIndicesReader = new System.IO.StreamReader(houseIndicesFile))
            {
                using (CsvReader houseIndicesCsv = new CsvReader(houseIndicesReader))
                {
                    houseIndicesCsv.Read();
                    while (houseIndicesCsv.Read())
                    {
                        int index = int.Parse(houseIndicesCsv[0]);
                        string key = houseIndicesCsv[1];
                        string postcode = houseIndicesCsv[2];
                        houseIndices.Add(index, new HouseIndex()
                        {
                            Index = index,
                            Key = key,
                            Postcode = postcode
                        });
                    }
                }
            }

            builder.RegisterInstance<Dictionary<int, HouseIndex>>(houseIndices);

            services.AddCors(options =>
                    options.AddPolicy(
                        CorsPolicy,
                        corsBuilder =>
                            corsBuilder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                        )
                );

            services.AddMvc().AddJsonOptions(json =>
            {
                json.SerializerSettings.Error = OnJsonError;
                json.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddSwaggerGen(
                setup =>
                    setup.SwaggerDoc(SwaggerApiName,
                    new Info
                    {
                        Version = "1",
                        Title = "House Prediction API Server",
                        Description = "House Prediction API Server",
                        TermsOfService = "N/A"
                    })
                );

            builder.Populate(services);
            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);

        }

        public void OnJsonError(object source, ErrorEventArgs error)
        {
            Debugger.Break();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(CorsPolicy);
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(setup => setup.SwaggerEndpoint($"/swagger/{SwaggerApiName}/swagger.json", "House Prediction API Server"));
            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute("spaFallback", new { controller = "Home", action = "Spa" });
            });
        }
    }

}