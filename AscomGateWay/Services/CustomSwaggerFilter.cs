using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AscomPayPG.Services
{
    public class CustomSwaggerFilter : IDocumentFilter
    {

        private IConfiguration _configuration;

        public CustomSwaggerFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            bool isFilterEnabled = Convert.ToBoolean(_configuration["SwaggerRouteSetting:isFilterEnabled"]);
            var swagerfilteredPath = _configuration["SwaggerRouteSetting:FilteredPath"];

            if (isFilterEnabled)
            {
                List<string> pathList = swagerfilteredPath.Split(',').ToList();

                foreach (var Swaggerpath in pathList)
                {
                    var nonMobileRoutes = swaggerDoc.Paths.Where(x => !x.Key.ToLower().Contains(Swaggerpath)).ToList();

                    nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
                }
            }

        }
    }
}
