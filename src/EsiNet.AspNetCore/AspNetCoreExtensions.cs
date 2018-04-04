﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public static class AspNetCoreExtensions
    {
        public static IServiceCollection AddEsi(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var parsers = new Dictionary<string, IEsiParser>();

            var bodyParser = new EsiBodyParser(parsers);
            var httpClient = new HttpClient();
            var includeParser = new EsiIncludeParser(bodyParser, httpClient);
            var ignoreParser = new EsiIgnoreParser();
            var textParser = new EsiTextParser();

            parsers.Add("esi:include", includeParser);
            parsers.Add("esi:remove", ignoreParser);
            parsers.Add("esi:comment", ignoreParser);
            parsers.Add("esi:text", textParser);

            services.AddSingleton(bodyParser);

            return services;
        }

        public static IApplicationBuilder UseEsi(this IApplicationBuilder app)
        {
            app.UseMiddleware<EsiMiddleware>();
            return app;
        }
    }
}