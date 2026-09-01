using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FirstName { get; set; }=string.Empty;

        [Required, MaxLength(200)]
        public string LastName { get; set; }= string.Empty;

        [Required, MaxLength(200)]
        public string UserName { get; set; }= string.Empty;

        [Required]
        public string Email { get; set; }= string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Role { get; set; } = "Customer";


    }
}
