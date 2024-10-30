using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscomPayPG.Helpers
{
    public class ConfigurationHelper
    {
        private static IConfiguration _configuration;
        public static void InstantaiteConfiguration(IConfiguration configuration) => _configuration = configuration;
        public static IConfiguration GetConfigurationInstance() => _configuration;

        public static string GetAppSettingSectionSingleProperty(string sectionNameAndProperty)
            => _configuration.GetSection(key: sectionNameAndProperty).Value;

    }
}
