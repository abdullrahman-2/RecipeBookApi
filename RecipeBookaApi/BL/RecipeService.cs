using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecipeBookaApi.DA.Models;
using RecipeBookApi.DA.Repositories;
using RecipeBookApi.Dtos.Recipe;

namespace RecipeBookApi.BL
{
    public class RecipeService : IRecipeService
    {
        protected readonly IMapper _mapper;
        protected readonly IRecipeRepository _recipeRepository;
        protected readonly UserManager<TbUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;



        public RecipeService(IMapper mapper,
            IWebHostEnvironment hostingEnvironment,
                           UserManager<TbUser> userManager,
                           IRecipeRepository recipeRepository) 
        {
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

 
        // ...
        public async Task<RecipeDto?> CreateRecipeAsync(RecipeDto recipeDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
               
                return null; 
            }

            var newRecipeEntity = _mapper.Map<RecipeDto,TbRecipe>(recipeDto);

            newRecipeEntity.UserId = int.Parse(userId); 
            newRecipeEntity.CreationDate = DateTime.UtcNow;
            newRecipeEntity.CurrentState = 1; 
            newRecipeEntity.UserName = user.UserName;

            var createdRecipeEntity = await _recipeRepository.SaveRecipe(newRecipeEntity);

            if (createdRecipeEntity ==false)
            {
              
                return null;
            }

            return recipeDto;
        }
        // ...

        public async Task<bool> DeleteRecipeAsync(int id)
        {
          var result = await _recipeRepository.DeleteById(id);
        
            if(result==false)
        return false;

            return true;

        }

        public async Task<List<VwRecipeDto>> GetAllRecipesAsync()
        {
            List<TbRecipe> CurrentAllRecipes = await _recipeRepository.GetAll();
            List<TbRecipe> AllRecipes =   CurrentAllRecipes.Where(a => a.CurrentState == 1).ToList();
            var result = _mapper.Map<List<TbRecipe>, List<VwRecipeDto>>(AllRecipes);

            if (result.Count == 0)
                return new List<VwRecipeDto>();

            return result;
        }

        public async Task<VwRecipeDto> GetRecipeByIdAsync(int id)
        {
            var Recipe = await _recipeRepository.GetById(id);
            if (Recipe == null)
            {
                return null; 
            }

            var user = await _userManager.FindByIdAsync(Recipe.UserId.ToString());


            var newRecipe =    _mapper.Map<TbRecipe,VwRecipeDto>(Recipe);
            newRecipe.UserName = user.UserName;

            return newRecipe;
        }

       
        public async Task<List<VwRecipeDto>> GetUserRecipesAsync(string userId) 
        {
             var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            List<TbRecipe> allRecipes = await _recipeRepository.GetAll();

            List<TbRecipe> userRecipes = allRecipes.Where(a => a.UserId == int.Parse(userId) && a.CurrentState != 0).ToList(); // تأكد إن UserId في TbRecipe هو string

             if (userRecipes.Count == 0) 
                return new List<VwRecipeDto>() ;

            return _mapper.Map<List<TbRecipe>, List<VwRecipeDto>>(userRecipes);
        }

        public async Task<bool> UpdateRecipeAsync(RecipeDto recipeDto)
        {
             var existingRecipe = await _recipeRepository.GetById(recipeDto.id);  

            if (existingRecipe == null)
            {
                return false; 
            }

          
            var newRecipeEntity =  _mapper.Map(recipeDto, existingRecipe);

             newRecipeEntity.CreationDate = DateTime.UtcNow;
            newRecipeEntity.CurrentState = 1;
 
            var result = await _recipeRepository.UpdateRecipe(newRecipeEntity);

            return result;
        }

        public async Task<RecipeDto> MakeImage(RecipeDto recipe) {

            if (recipe.ImageFile != null)
            {
 
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                 if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                 var uniqueFileName = Guid.NewGuid().ToString() + "_" + recipe.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                 using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await recipe.ImageFile.CopyToAsync(fileStream);
                }

                 recipe.Img = $"/images/{uniqueFileName}";

            
            }
            return recipe;
        }


       
    }
}
