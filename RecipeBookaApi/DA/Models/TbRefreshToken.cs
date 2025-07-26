using System.ComponentModel.DataAnnotations;
using RecipeBookaApi.DA.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBookApi.DA.Models
{
    public class TbRefreshToken
    {
        public int RefreshID { get; set; }

        [Required]
        public string Token { get; set; } = null!; 

        public DateTime ExpiryTime { get; set; } 

        [Required]
        public int UserId { get; set; } 


        public bool IsRevoked { get; set; } = false; 
    }
}

