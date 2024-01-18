using WebActivatorEx;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Collections.Generic;
using Swagger;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Swagger
{
    public class AuthTokenOperation : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/GenerateToken", new PathItem
            {
                post = new Operation
                {
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            type = "string",
                            name = "grant_type",
                            required = true,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "username",
                            required = true,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "password",
                            required = true,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "source",
                            required = true,
                            @in = "formData"
                        },
                    }
                }
            });
        }
    }
}
