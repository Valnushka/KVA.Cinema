using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KVA.Cinema.Services
{
    public class UserService : IService<UserCreateViewModel, UserDisplayViewModel, UserEditViewModel>
    {
        /// <summary>
        /// Minimum age allowed to use app
        /// </summary>
        private const int AGE_MIN = 14;

        /// <summary>
        /// Maximum age allowed to use app
        /// </summary>
        private const int AGE_MAX = 120;

        /// <summary>
        /// Pattern to check email
        /// </summary>
        private const string EMAIL_PATTERN = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        /// <summary>
        /// Minimum length allowed for Last Name and First Name
        /// </summary>
        private const int NAME_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Last Name and First Name
        /// </summary>
        private const int NAME_LENGHT_MAX = 30;

        /// <summary>
        /// Maximum length allowed for Nickname
        /// </summary>
        private const int NICKNAME_LENGHT_MIN = 3;

        /// <summary>
        /// Maximum length allowed for Nickname
        /// </summary>
        private const int NICKNAME_LENGHT_MAX = 20;

        /// <summary>
        /// Minimum password length
        /// </summary>
        private const int PASSWORD_LENGHT_MIN = 8;

        /// <summary>
        /// Maximum password length
        /// </summary>
        private const int PASSWORD_LENGHT_MAX = 120;

        /// <summary>
        /// Last Name, First Name and Nickname
        /// </summary>
        private string[] names;

        /// <summary>
        /// User management service
        /// </summary>
        public UserManager<User> UserManager { get; }

        /// <summary>
        /// User authentication service
        /// </summary>
        public SignInManager<User> SignInManager { get; }

        public EmailSender EmailSender { get; }

        private CinemaContext Context { get; }

        public UserService(CinemaContext db,
                           UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           EmailSender emailSender)
        {
            Context = db;
            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
        }

        public IEnumerable<UserDisplayViewModel> ReadAll()
        {
            return Context.Users
                .Select(x => new UserDisplayViewModel()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Nickname = x.Nickname,
                    BirthDate = x.BirthDate,
                    Email = x.Email,
                    SubscriptionIds = x.UserSubscriptions.Select(x => x.SubscriptionId)
                }).ToList();
        }

        public IEnumerable<UserCreateViewModel> Read()
        {
            return Context.Users.Select(x => new UserCreateViewModel()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Nickname = x.Nickname,
                BirthDate = x.BirthDate,
                Email = x.Email
            }).ToList();
        }
        
        public UserDisplayViewModel Read(string nickname)
        {
            var user = Context.Users.FirstOrDefault(x => x.Nickname == nickname);

            if (user == default)
            {
                throw new EntityNotFoundException($"User with nickname \"{nickname}\" not found");
            }

            return MapToDisplayViewModel(user);
        }

        public UserDisplayViewModel Read(Guid userId)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == default)
            {
                throw new EntityNotFoundException($"User with id \"{userId}\" not found");
            }

            return MapToDisplayViewModel(user);
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="userData"></param>
        /// <exception cref="DuplicatedEntityException">Nickname is not unique</exception>
        /// <exception cref="FailedToCreateEntityException">Failed to create user</exception>
        /// <returns></returns>
        public async Task CreateAsync(UserCreateViewModel userData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(userData.FirstName,
                                                        userData.LastName,
                                                        userData.Nickname,
                                                        userData.BirthDate,
                                                        userData.Email,
                                                        userData.Password,
                                                        userData.PasswordConfirm))
            {
                throw new ArgumentNullException("One or more required fields have no value");
            }

            if (userData.BirthDate > DateTime.UtcNow.AddYears(-AGE_MIN) || userData.BirthDate < DateTime.UtcNow.AddYears(-AGE_MAX))
            {
                throw new ArgumentException($"Age must be in {AGE_MIN}-{AGE_MAX}");
            }

            if (!new Regex(EMAIL_PATTERN).IsMatch(userData.Email))
            {
                throw new ArgumentException("Incorrect email format");
            }

            names = new string[] { userData.FirstName, userData.LastName };

            foreach (var item in names)
            {
                if (item.Length < NAME_LENGHT_MIN)
                {
                    throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN}");
                }

                if (item.Length > NAME_LENGHT_MAX)
                {
                    throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
                }
            }

            if (userData.Nickname.Length < NICKNAME_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {NICKNAME_LENGHT_MIN}");
            }

            if (userData.Nickname.Length > NICKNAME_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {NICKNAME_LENGHT_MAX} symbols");
            }

            if (userData.Password.Length is < PASSWORD_LENGHT_MIN or > PASSWORD_LENGHT_MAX)
            {
                throw new ArgumentException($"Password length must be in {PASSWORD_LENGHT_MIN}-{PASSWORD_LENGHT_MAX} symbols");
            }

            if (userData.Password != userData.PasswordConfirm)
            {
                throw new ArgumentException("Passwords are not equal");
            }

            if (Context.Users.FirstOrDefault(x => x.Nickname == userData.Nickname) != default)
            {
                throw new DuplicatedEntityException($"User with nickname \"{userData.Nickname}\" is already exist");
            }

            if (Context.Users.FirstOrDefault(x => x.Email == userData.Email) != default)
            {
                throw new DuplicatedEntityException($"User with email \"{userData.Email}\" is already exist");
            }

            User newUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = userData.Nickname,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Nickname = userData.Nickname,
                BirthDate = userData.BirthDate,
                Email = userData.Email,
                RegisteredOn = DateTime.UtcNow,
                LastVisit = DateTime.UtcNow,
                IsActive = false
            };

            var result = await UserManager.CreateAsync(newUser, userData.Password);

            if (result.Succeeded)
            {
                string userToken = await UserManager.GenerateEmailConfirmationTokenAsync(newUser);
                await EmailSender.SendActivateAccountLinkAsync(newUser.Email, newUser.Id.ToString(), newUser.Nickname, userToken);
                //await SignInManager.SignInAsync(newUser, true); - автоматический вход после создания акка (без подтверждения)
                Context.SaveChanges();
            }
            else
            {
                throw new FailedToCreateEntityException(result.Errors);
            }
        }

        [Obsolete("This method is obsolete. Call async Task CreateAsync instead.", true)]
        void IService<UserCreateViewModel, UserDisplayViewModel, UserEditViewModel>.Create(UserCreateViewModel entityData)
        {
            return;
        }

        public async Task<IdentityResult> ActivateAccountAsync(string userId, string token)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(userId, token))
            {
                throw new ArgumentNullException("One or more required fields have no value");
            }

            if (!Guid.TryParse(userId, out Guid userGuidId))
            {
                throw new ArgumentException("Invalid user id format");
            }

            User user = Context.Users.FirstOrDefault(x => x.Id == userGuidId);

            if (user == default)
            {
                throw new EntityNotFoundException($"User with id \"{userId}\" not found");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                user.IsActive = true;
                Context.SaveChanges();
            }

            return result;
        }

        public void Update(Guid userId, UserEditViewModel userNewData)
        {
            User user = Context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == default)
            {
                throw new EntityNotFoundException($"User with id \"{userId}\" not found");
            }

            if (CheckUtilities.ContainsNullOrEmptyValue(userId,
                                                        userNewData.LastName,
                                                        userNewData.FirstName,
                                                        userNewData.Nickname,
                                                        userNewData.BirthDate,
                                                        userNewData.Email))
            {
                throw new ArgumentNullException("One or more user's parameters have no value");
            }

            if (userNewData.BirthDate > DateTime.UtcNow.AddYears(-AGE_MIN))
            {
                throw new ArgumentException($"Age must not be less than {AGE_MIN}");
            }

            if (!new Regex(EMAIL_PATTERN).IsMatch(userNewData.Email))
            {
                throw new ArgumentException("Incorrect email format");
            }

            names = new string[] { userNewData.FirstName, userNewData.LastName, userNewData.Nickname };

            foreach (var item in names)
            {
                if (item.Length < NAME_LENGHT_MIN)
                {
                    throw new ArgumentException($"Length cannot be less than {NAME_LENGHT_MIN}");
                }

                if (item.Length > NAME_LENGHT_MAX)
                {
                    throw new ArgumentException($"Length cannot be more than {NAME_LENGHT_MAX} symbols");
                }
            }

            if (Context.Users.FirstOrDefault(x => x.Nickname == userNewData.Nickname && x.Id != userNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"User with nickname \"{userNewData.Nickname}\" is already exist");
            }

            if (Context.Users.FirstOrDefault(x => x.Email == userNewData.Email && x.Id != userNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"User with email \"{userNewData.Email}\" is already exist");
            }

            user.FirstName = userNewData.FirstName;
            user.LastName = userNewData.LastName;
            user.Nickname = userNewData.Nickname;
            user.BirthDate = userNewData.BirthDate;
            user.Email = userNewData.Email;

            // TO THINK: А если мы ввели те же самые данные пользователя - то имеет ли тут смысл вызывать сохранение бд?
            Context.SaveChanges();
        }

        public void Delete(Guid userId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(userId))
            {
                throw new ArgumentNullException("User Id has no value");
            }

            User user = Context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == default)
            {
                throw new EntityNotFoundException($"User with Id \"{userId}\" not found");
            }

            Context.Users.Remove(user);
            Context.SaveChanges();
        }

        public void AddSubscription(string nickname, Guid subscriptionId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(nickname, subscriptionId))
            {
                throw new ArgumentNullException("Id has no value");
            }

            User user = Context.Users.FirstOrDefault(x => x.UserName == nickname);

            if (user == default)
            {
                throw new EntityNotFoundException($"User \"{nickname}\" not found");
            }

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            if (subscription == default)
            {
                throw new EntityNotFoundException($"Subscription with Id \"{subscriptionId}\" not found");
            }

            if (user.UserSubscriptions.Any(x => x.SubscriptionId == subscription.Id))
            {
                throw new DuplicatedEntityException("This subscription is already bought");
            }

            var activatedOn = DateTime.UtcNow; //TODO: использовать часовой пояс пользователя
            var lastUntil = activatedOn.Date.AddDays(subscription.Duration + 1);

            Context.UserSubscriptions.Add(new UserSubscription
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                UserId = user.Id,
                ActivatedOn = activatedOn,
                LastUntil = lastUntil
            });

            Context.SaveChanges();
        }

        public void RemoveSubscription(string nickname, Guid subscriptionId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(nickname, subscriptionId))
            {
                throw new ArgumentNullException("One or more required fields have no value");
            }

            User user = Context.Users.FirstOrDefault(x => x.UserName == nickname);

            if (user == default)
            {
                throw new EntityNotFoundException($"User \"{nickname}\" not found");
            }

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            if (subscription == default)
            {
                throw new EntityNotFoundException($"Subscription with Id \"{subscriptionId}\" not found");
            }

            if (!user.UserSubscriptions.Any(x => x.SubscriptionId == subscription.Id))
            {
                throw new EntityNotFoundException("User doesn't have this subscription");
            }

            var entity = user.UserSubscriptions.FirstOrDefault(x => x.SubscriptionId == subscription.Id);

            Context.UserSubscriptions.Remove(entity);

            Context.SaveChanges();// TODO
        }

        private UserDisplayViewModel MapToDisplayViewModel(User user)
        {
            var subscriptionIds = user.UserSubscriptions.Select(x => x.SubscriptionId);
            var subs = Context.Subscriptions.Where(x => subscriptionIds.Contains(x.Id));

            return new UserDisplayViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nickname = user.Nickname,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Subscriptions = Context.Subscriptions.Where(x => subscriptionIds.Contains(x.Id)),
                UserSubscriptions = user.UserSubscriptions,
            };
        }
    }
}
