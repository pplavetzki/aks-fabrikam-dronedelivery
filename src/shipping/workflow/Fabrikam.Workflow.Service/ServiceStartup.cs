﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fabrikam.Workflow.Service.RequestProcessing;
using Fabrikam.Workflow.Service.Services;

namespace Fabrikam.Workflow.Service
{
    public static class ServiceStartup
    {
        private const string AppInsightsInstrumentationKey = "ApplicationInsights-InstrumentationKey";

        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddOptions();

            // Configure AppInsights
            services.AddApplicationInsightsKubernetesEnricher();
            services.AddApplicationInsightsTelemetry(
                       context.Configuration[AppInsightsInstrumentationKey]);

            services.Configure<WorkflowServiceOptions>(context.Configuration);
            services.AddHostedService<WorkflowService>();

            services.AddTransient<IRequestProcessor, RequestProcessor>();

            services.AddHttpClient<IPackageServiceCaller, PackageServiceCaller>(c =>
            {
                c.BaseAddress = new Uri(context.Configuration["SERVICE_URI_PACKAGE"]);
            });

            services.AddHttpClient<IDroneSchedulerServiceCaller, DroneSchedulerServiceCaller>(c =>
            {
                c.BaseAddress = new Uri(context.Configuration["SERVICE_URI_DRONE"]);
            });

            services.AddHttpClient<IDeliveryServiceCaller, DeliveryServiceCaller>(c =>
            {
                c.BaseAddress = new Uri(context.Configuration["SERVICE_URI_DELIVERY"]);
            });
        }
    }
}
