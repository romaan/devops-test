using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JHipsterNet.Config;
using DevOpsDeploy.Data;
using DevOpsDeploy.Models;
using DevOpsDeploy.Security;
using DevOpsDeploy.Security.Jwt;
using DevOpsDeploy.Service;
using DevOpsDeploy.Service.Mapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService = DevOpsDeploy.Service.AuthenticationService;
using IAuthenticationService = DevOpsDeploy.Service.IAuthenticationService;

namespace DevOpsDeploy.Infrastructure {
    public static class SecurityStartup {

        public const string UserNameClaimType = JwtRegisteredClaimNames.Sub;

        public static IServiceCollection AddSecurityModule(this IServiceCollection @this)
        {
            //TODO Retrieve the signing key properly (DRY with TokenProvider)
            var opt = @this.BuildServiceProvider().GetRequiredService<IOptions<JHipsterSettings>>();
            var jhipsterSettings = opt.Value;
            byte[] keyBytes;
            var secret = jhipsterSettings.Security.Authentication.Jwt.Secret;

            if (!string.IsNullOrWhiteSpace(secret)) {
                keyBytes = Encoding.ASCII.GetBytes(secret);
            }
            else {
                keyBytes = Convert.FromBase64String(jhipsterSettings.Security.Authentication.Jwt.Base64Secret);
            }

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            @this.AddIdentity<User, Role>(options => {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.ClaimsIdentity.UserNameClaimType = UserNameClaimType;
                })
                .AddEntityFrameworkStores<ApplicationDatabaseContext>()
                .AddUserStore<UserStore<User, Role, ApplicationDatabaseContext, string, IdentityUserClaim<string>,
                    UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
                .AddRoleStore<RoleStore<Role, ApplicationDatabaseContext, string, UserRole, IdentityRoleClaim<string>>
                >()
                .AddDefaultTokenProviders();


            @this
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg => {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                        ClockSkew = TimeSpan.Zero,/// remove delay of token when expire
                        NameClaimType = UserNameClaimType
                    };
                });

            @this.AddScoped<IAuthenticationService, AuthenticationService>();
            @this.AddScoped<ITokenProvider, TokenProvider>();
            @this.AddScoped<IUserService, UserService>();
            @this.AddScoped<UserMapper>();
            @this.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();
            @this.AddScoped<IClaimsTransformation, RoleClaimsTransformation>();
            @this.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();
            @this.AddSingleton<IMailService, MailService>();

            return @this;
        }

        public static IApplicationBuilder UseApplicationSecurity(this IApplicationBuilder @this,
            JHipsterSettings jhipsterSettings)
        {
            @this.UseCors(CorsPolicyBuilder(jhipsterSettings.Cors));
            @this.UseAuthentication();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            @this.UseHsts();
            // @this.UseHttpsRedirection();
            return @this;
        }

        private static Action<CorsPolicyBuilder> CorsPolicyBuilder(Cors config)
        {
            //TODO implement an url based cors policy rather than global or per controller
            return builder => {
                if (!config.AllowedOrigins.Equals("*"))
                {
                    if (config.AllowCredentials)
                    {
                        builder.AllowCredentials();
                    }
                    else
                    {
                        builder.DisallowCredentials();
                    }
                }

                builder.WithOrigins(config.AllowedOrigins)
                    .WithMethods(config.AllowedMethods)
                    .WithHeaders(config.AllowedHeaders)
                    .WithExposedHeaders(config.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(config.MaxAge));
            };
        }
    }
}
