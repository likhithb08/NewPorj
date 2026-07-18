using LOCPS.Enums;
using LOCPS.Models;
using LOCPS.Constants;
using LOCPS.Repositories.Interfaces;
using LOCPS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LOCPS.Services.Implementations
{
    public class UserServices : IUserServices
    {
        public readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }
        public async Task<User> RegisterUserAsync(User user)
        {
            // Checking if the user objected recieved is null 
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            //input validation
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email Required");
            }
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Password is required");
            }

            //Check for user Existance
            var existingUser = await _userRepository.GetUserByEmailIdAsync(user.Email);
            //if the user already exists we will throw an exception
            if(existingUser != null)
            {
                throw new Exception("User Already Exists");
            }
            //if not we add the created date and active status of the user account
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            //Hashing the user password
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            //Create user using the object from the controller in database
            var createdUser = await _userRepository.CreateUserAsync(user);
            //Return the created user to the controller
            return createdUser;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            //Validating email
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new Exception("Email is required!");
            }
           
            //Getting the user from database
            var user = await  _userRepository.GetUserByEmailIdAsync(email);
            //Checking if he exist in the database
            if (user == null)
            {
                throw new Exception("User Does Not Exist!");
            }

            //Comparig the email and password recieved with the one existing in the databse 
            //Verifyng the Hashed Password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid password");
            }
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            //To check if the user id is above is greater than 0   
            if(userId <= 0)
            {
                throw new ArgumentException("Invalid user ID");
            }
            var user = await _userRepository.GetUserByIdAsync(userId);
            //Checking if the user is not null after finding him
            if(user == null)
            {
                throw new KeyNotFoundException($"No user exist with the user id {userId} provided");
            }
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()

        {
            //To get all all the users 
            var usersList = await _userRepository.GetAllAsync();
            return usersList;
        }

        public async Task<IEnumerable<User>> GetUserByRoleAsync(Roles Role)
        {
            //Validation to check if the roles sent exists
            if (!Enum.IsDefined(typeof(Roles), Role))
            {
                throw new ArgumentException("Invalid Role");
            }
            //Getting the users by thier role
            var usersList = await _userRepository.GetUsersByRoleAsync(Role);

            return usersList;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            //Check if the user is null if yes throw an exeption
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            var existingUser = await _userRepository.GetByIdAsync(user.UserId);
            if(existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            //Updating the user in database
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return updatedUser;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            //to validate the user id is not negative
            if(userId <= 0)
            {
                throw new KeyNotFoundException("User not found with the user id {userId}");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            //to check if the user is null or not 
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
                
            }
            //if the user is not null delete the user based on his id
            await _userRepository.DeleteUserByIdAsync(user.UserId);
            //return ture after delete
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassord, string newPassword)
        {
            //Validate the user id
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid userId");
            }
            //Check if the old password is empty or null
            if (string.IsNullOrWhiteSpace(oldPassord))
            {
                throw new ArgumentException("Old Password is Required");
            }
            //Check if the new password is empty or null
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("New Password is Required");
            }
            //Get the user details 
            var user = await _userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                throw new KeyNotFoundException("User not found with the user id {userId}");
            }
            //if user is not null check if the Old password matches the Existing password
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassord);
            if(result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Incorrect old Password");
            }

            //if the old password matches the existing password has the new password and save it
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdateUserAsync(user);
            return true;

        }

        public async Task<bool> AssignRoleAsync(int userId, int roleId)
        {
            //validate the user id
            if(userId <= 0)
            {
                throw new KeyNotFoundException("Invalid user id");
            }
            // Bug 8 fix: was Enum.IsDefined(typeof(Roles), roleId) which compared the int
            // directly to Roles enum ordinals (Customer=0,LoanOfficer=1,...) — mismatched DB IDs (1,2,3,4)
            // Now uses RoleConstants.GetRoleFromId to map correctly
            if (!Enum.IsDefined(typeof(Roles), RoleConstants.GetRoleFromId(roleId)))
            {
                throw new Exception("Invalid Role!");
            }
            //Check for the user
            var user = await _userRepository.GetByIdAsync(userId);
            //If User is null throw an exception
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.RoleId = roleId;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }

    }
}
