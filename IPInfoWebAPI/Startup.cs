using AutoMapper;
using IPInfoProviderLibrary;
using IPInfoWebAPI.Mappings;
using IPInfoWebAPI.Models;
using IPInfoWebAPI.Repositories;
using IPInfoWebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IPInfoWebAPI
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
            // register my database connection
            services.AddDbContext<IpInformationsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IpInformationsDatabase")));
            // register IPInfoProvider DLL
            services.AddSingleton<IIPInfoProvider, IPInfoProvider>(); 
            // resolve all the repositories and services
            services.AddScoped<IIPDetailsRepository, IPDetailsRepository>();
            services.AddScoped<IIPRequestHandlerService, IPRequestHandlerService>();

            // AutoMapper configuration
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapping());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMemoryCache();

            // make Newtonsoft to configure the ReferenceLoopHandling
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
