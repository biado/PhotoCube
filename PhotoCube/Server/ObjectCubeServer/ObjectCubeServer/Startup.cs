using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ObjectCubeServer
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
            services.AddMvc(option => option.EnableEndpointRouting = false );
            
            // enable json input/output for controllers
            services.AddControllers()
                .AddNewtonsoftJson();
                //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
                
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoCube API", Version = "v1" });
            });

            /* CORS: To enable calls from other origins:
             * https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2 */
             /*
            services.AddCors(options =>
            {
                options.AddPolicy("AllowRequestsFromLocalhost",
                    builder => builder.WithOrigins(@"http://localhost", @"https://localhost", @"http://localhost:3000/"));
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "PhotoCube API V1");
                });
            }
            else
            {
                app.UseHsts();
            }

            // Shows UseCors with named policy.
            app.UseCors(builder => builder.WithOrigins(@"http://localhost:3000", @"https://localhost:3000").AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
