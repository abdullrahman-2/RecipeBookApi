using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecipeBookaApi.DA.Models;
using RecipeBookaApi.Dtos.User;
using RecipeBookApi.DA.Repositories;
using RecipeBookApi.Dtos.User;
using RecipeBookApi.Dtos.Auth;
using RecipeBookApi.Dtos.Recipe;


namespace RecipeBookApi.BL
{
    public class UserService : IUserService
    {
         protected readonly IMapper _mapper;
        protected readonly UserManager<TbUser> _userManager; 
        protected readonly SignInManager<TbUser> _signInManager;  
        protected readonly IRecipeRepository _recipeRepository;  
        protected readonly IRecipeService _recipeService;   

        public UserService(IMapper mapper,
            IRecipeService recipeService,
                           UserManager<TbUser> userManager, SignInManager<TbUser> signInManager,
                           IRecipeRepository recipeRepository)  
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _recipeRepository = recipeRepository;
            _recipeService = recipeService;
        }

 
        public async Task<AuthResultDto> RegisterUserAsync(RegisterUserDto model)
        {
             var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Email already registered." }
                };
            }




            var newUser = _mapper.Map<RegisterUserDto, TbUser>(model);
            newUser.CurrentState = 1; // 


            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {


                return new AuthResultDto
                {
                    Success = true,
                    Token = "",
                    RefreshToken = "",
                    User = newUser,
                };
            }
            else
            {

                return new AuthResultDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
        }

        public async Task<AuthResultDto> LoginUserAsync(LoginUserDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

 
            if (user == null || user.CurrentState != 1)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Invalid login credentials." }  
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
  
            if (result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = true,
                    Token = "",
                    RefreshToken = "",
                    User = user,
                };
            }
            else
            {
                 if (result.IsLockedOut)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Errors = new List<string> { "User account locked out. Please try again later." }
                    };
                }
                 
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new List<string> { "Invalid login credentials." }
                };
            }
        }

 


        public async Task<UserDto?> UpdateUserAsync(UpdateUserDto userDto)
        {
            var userToUpdate = await _userManager.FindByIdAsync(userDto.UserId.ToString()); // Identity بيحتاج ID كـ string

            if (userToUpdate == null)
            {
                return null; 
            }


            userToUpdate.UserName = userDto.UserName;
            userToUpdate.Email = userDto.Email;

        
          
            var updateResult = await _userManager.UpdateAsync(userToUpdate);

            if (updateResult.Succeeded)
            {
                return _mapper.Map<UserDto>(userToUpdate);
            }

            return null;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id.ToString());

            if (userToDelete == null)
            {
                return false;
            }

            userToDelete.CurrentState = 0;
            var result = await _userManager.UpdateAsync(userToDelete);  

            return result.Succeeded;
        }

         public async Task<bool> DeleteRecipesByUserIdAsync(int userId)
        {
             var allRecipes = await _recipeRepository.GetAll();

             var userRecipes = allRecipes.Where(r => r.UserId == userId).ToList();

            bool allSucceeded = true;
            foreach (var recipe in userRecipes)
            {
                 var result = await _recipeRepository.DeleteById(recipe.Id);
                if (!result)
                {
                    allSucceeded = false;  
                }
            }
            return allSucceeded;  
        }

        public async Task<TbUser> GetUserByIdAsync(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return null;

            return user;

        }

         public async Task<UserDto> GetUserInfoAsync(string userId)
        {
            var currentUser = await GetUserByIdAsync(userId);

            if (currentUser == null)

                return null;


            var userDto = _mapper.Map<TbUser, UserDto>(currentUser);


            var userRecipes = await _recipeService.GetUserRecipesAsync(userDto.UserId.ToString());
            if (userRecipes == null)
                userDto.vwRecipeDtos = new List<VwRecipeDto>();


            userDto.vwRecipeDtos = userRecipes;


            return userDto;
        }

    }
}