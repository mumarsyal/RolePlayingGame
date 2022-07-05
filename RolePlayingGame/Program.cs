using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RolePlayingGame.Data;
using RolePlayingGame.Profiles;
using RolePlayingGame.Services.CharacterService;

namespace RolePlayingGame
{
    public class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigurationManager configuration = builder.Configuration;

            // AutoMapper Configs
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = config.CreateMapper();

            // Add services to the container.
            builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            /*if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }*/

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}