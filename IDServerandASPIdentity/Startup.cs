using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IDServer.Data;
using IDServer.Models;
using IDServer.Services;
using IdentityServerEF;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IDServerandASPIdentity;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Services;
using IdentityServer4.Validation;

namespace IDServer
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration
    {
      get;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
      var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

      services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();
      services.AddTransient<IProfileService, ProfileService>();
      services.AddTransient<IRedirectUriValidator, RedirectUriValidator>();
      services.AddMvc();
      // configure identity server with in-memory stores, keys, clients and scopes
      services.AddIdentityServer()
         //  .AddDeveloperSigningCredential()
         .AddSigningCredential(new X509Certificate2(Configuration.GetValue<string>("Certificate:Path"), Configuration.GetValue<string>("Certificate:Password")))
         // this adds the config data from DB (clients, resources)
         .AddAspNetIdentity<ApplicationUser>()

         .AddConfigurationStore(options =>
         {
           options.ConfigureDbContext = builder =>
               builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                   sql => sql.MigrationsAssembly(migrationsAssembly));
         })
          // this adds the operational data from DB (codes, tokens, consents)
          .AddOperationalStore(options =>
          {
            options.ConfigureDbContext = builder =>
                builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(migrationsAssembly));

            // this enables automatic token cleanup. this is optional.
            options.EnableTokenCleanup = true;
            options.TokenCleanupInterval = 30;

          })
          .AddProfileService<ProfileService>()
          .AddRedirectUriValidator<RedirectUriValidator>(); //use for subDomain
         //Add Authentication for User APIs
    /*  services.AddAuthentication()
              .AddIdentityServerAuthentication("token", isAuth =>
              {
                isAuth.Authority = "base_address_of_identityserver";
                isAuth.ApiName = "name_of_api";
              });*/

      services.AddCors();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      app.UseStaticFiles();
      // app.UseMiddleware<LogRequestMiddleware>();

      app.UseIdentityServer();
     
      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
      //InitializeDatabase(app);
    }

    private void InitializeDatabase(IApplicationBuilder app)
    {
      using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
      {
        serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        context.Database.Migrate();
        if (!context.Clients.Any())
        {
          foreach (var client in Config.GetClients())
          {
            context.Clients.Add(client.ToEntity());
          }
          context.SaveChanges();
        }

        if (!context.IdentityResources.Any())
        {
          foreach (var resource in Config.GetIdentityResources())
          {
            context.IdentityResources.Add(resource.ToEntity());
          }
          context.SaveChanges();
        }

        if (!context.ApiResources.Any())
        {
          foreach (var resource in Config.GetApiResources())
          {
            context.ApiResources.Add(resource.ToEntity());
          }
          context.SaveChanges();
        }
      }
    }
  }
}
