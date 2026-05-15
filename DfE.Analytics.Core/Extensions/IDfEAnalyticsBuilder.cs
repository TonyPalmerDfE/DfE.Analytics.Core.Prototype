using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DfE.Analytics.Core.Extensions
{
    public interface IDfEAnalyticsBuilder
    {
        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }

    internal class DfEAnalyticsBuilder : IDfEAnalyticsBuilder
    {
        public IServiceCollection Services { get; }
        public IConfiguration Configuration { get; }
        
        public DfEAnalyticsBuilder(IServiceCollection services, IConfiguration configuration)
        {
            Services = services;
            Configuration = configuration;
        }
    }


}

