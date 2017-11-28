﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Threading.Tasks;
using Coddee.AspNet;
using Coddee.Loggers;
using Coddee.Security;
using HR.Data.LinqToSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HR.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "..\\", "..\\", "..\\", "..\\", "HR.Clients.WPF", "DB"));
            AppDomain.CurrentDomain.SetData("DataDirectory", dbLocation);

            services.AddLogger(LoggerTypes.DebugOutput, LogRecordTypes.Debug);
            services.AddILObjectMapper();
            services.AddLinqRepositoryManager<HRDBManager>(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30", "HR.Data.LinqToSQL");


            services.AddCoddeeControllers(config =>
            {
                config.RegisterController<AuthController>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCoddeeDynamicApi();
        }
    }

    public class AuthController
    {
        [ApiAction("Auth/Authenticate")]
        public async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            return new AuthenticationResponse { Status = AuthenticationStatus.Successfull };
        }
    }
}