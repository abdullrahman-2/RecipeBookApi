//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore;
//using RecipeBookaApi.DA.Models;

//namespace RecipeBookApi.DA.Models
//{
//    public class Design
//    {
//        public class RecipeBookContextFactory : IDesignTimeDbContextFactory<RecipeBookContext>
//        {
//            public RecipeBookContext CreateDbContext(string[] args)
//            {
//                var configuration = new ConfigurationBuilder()
//                    .SetBasePath(Directory.GetCurrentDirectory())
//                    .AddJsonFile("appsettings.json")
//                    .Build();

//                var optionsBuilder = new DbContextOptionsBuilder<RecipeBookContext>();

//                var connectionString = configuration.GetConnectionString("DefaultConnection");

//                optionsBuilder.UseSqlServer(connectionString);

//                return new RecipeBookContext(optionsBuilder.Options);
//            }
//        }
//    }
//}
