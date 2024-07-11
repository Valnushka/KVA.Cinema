using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Models.ViewModels.SubscriptionLevel;
using KVA.Cinema.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class SubscriptionLevelService : IService<SubscriptionLevelCreateViewModel, SubscriptionLevelDisplayViewModel, SubscriptionLevelEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MAX = 50;

        public CinemaContext Context { get; }

        public SubscriptionLevelService(CinemaContext db)
        {
            Context = db;
        }

        public IEnumerable<SubscriptionLevelCreateViewModel> Read()
        {
            return Context.SubscriptionLevels.Select(x => new SubscriptionLevelCreateViewModel()
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
        }

        public SubscriptionLevelDisplayViewModel Read(Guid subscriptionLevelId)
        {
            var subscriptionLevel = Context.SubscriptionLevels.FirstOrDefault(x => x.Id == subscriptionLevelId);

            if (subscriptionLevel == default)
            {
                throw new EntityNotFoundException($"Subscription level with id \"{subscriptionLevelId}\" not found");
            }

            return MapToDisplayViewModel(subscriptionLevel);
        }

        public IEnumerable<SubscriptionLevelDisplayViewModel> ReadAll()
        {
            return Context.SubscriptionLevels.Select(x => new SubscriptionLevelDisplayViewModel()
            {
                Id = x.Id,
                Title = x.Title
            }).ToList();
        }

        public void CreateAsync(SubscriptionLevelCreateViewModel subscriptionLevelData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionLevelData.Title))
            {
                throw new ArgumentNullException("Title has no value");
            }

            if (subscriptionLevelData.Title.Length < TITLE_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TITLE_LENGHT_MIN} symbols");
            }

            if (subscriptionLevelData.Title.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (Context.SubscriptionLevels.FirstOrDefault(x => x.Title == subscriptionLevelData.Title) != default)
            {
                throw new DuplicatedEntityException($"Subscription level with title \"{subscriptionLevelData.Title}\" is already exist");
            }

            SubscriptionLevel newSubscriptionLevel = new SubscriptionLevel()
            {
                Id = Guid.NewGuid(),
                Title = subscriptionLevelData.Title
            };

            Context.SubscriptionLevels.Add(newSubscriptionLevel);
            Context.SaveChanges();
        }

        public void Delete(Guid subscriptionLevelId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionLevelId))
            {
                throw new ArgumentNullException("Id has no value");
            }

            SubscriptionLevel subscriptionLevel = Context.SubscriptionLevels.FirstOrDefault(x => x.Id == subscriptionLevelId);

            if (subscriptionLevel == default)
            {
                throw new EntityNotFoundException($"Subscription level with Id \"{subscriptionLevelId}\" not found");
            }

            Context.SubscriptionLevels.Remove(subscriptionLevel);
            Context.SaveChanges();
        }

        public void Update(Guid subscriptionLevelId, SubscriptionLevelEditViewModel subscriptionLevelNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionLevelId, subscriptionLevelNewData.Title))
            {
                throw new ArgumentNullException("One or more parameters have no value");
            }

            SubscriptionLevel subscriptionLevel = Context.SubscriptionLevels.FirstOrDefault(x => x.Id == subscriptionLevelId);

            if (subscriptionLevel == default)
            {
                throw new EntityNotFoundException($"Subscription level with id \"{subscriptionLevelId}\" not found");
            }

            if (subscriptionLevelNewData.Title.Length < TITLE_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TITLE_LENGHT_MIN} symbols");
            }

            if (subscriptionLevelNewData.Title.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (Context.SubscriptionLevels.FirstOrDefault(x => x.Title == subscriptionLevelNewData.Title && x.Id != subscriptionLevelNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Subscription level with title \"{subscriptionLevelNewData.Title}\" is already exist");
            }

            subscriptionLevel.Title = subscriptionLevelNewData.Title;

            Context.SaveChanges();
        }

        private SubscriptionLevelDisplayViewModel MapToDisplayViewModel(SubscriptionLevel subscriptionLevel)
        {
            return new SubscriptionLevelDisplayViewModel()
            {
                Id = subscriptionLevel.Id,
                Title = subscriptionLevel.Title
            };
        }
    }
}
