using Auth.Core;
using Auth.Core.Interfaces;
using Auth.Core.Services;
using Auth.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Auth
{
	public class Startup
    {
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			// Регистрируем настройки сервиса Auth
			services.AddOptions();
			services.Configure<AuthOptions>(Configuration.GetSection("AuthOptions"));

			// Регистрация строк подключения к БД
			services.AddDbContext<AuthDataContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("AuthData")));

			services.AddTransient<IAuthRepository, AuthRepository>();
	        services.AddTransient<ILogRepository, LogRepository>();
			services.AddTransient<ITokenController, TokenController>();

			services.AddTransient<IAdminService, AdminService>();
	        services.AddTransient<IUserService, UserService>();
	        services.AddTransient<IAuthenticationService, AuthenticationService>();

			services.AddMvc();

			// Swagger
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Auth API", Version = "v1" });
			});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
			});

			app.UseMvc();
		}
    }
}
