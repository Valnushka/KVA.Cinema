using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class SubscriptionLevelService : BaseService<SubscriptionLevel, SubscriptionLevelCreateViewModel, SubscriptionLevelDisplayViewModel, SubscriptionLevelEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for Title
        /// </summary>
        private const int TITLE_LENGHT_MAX = 50;

        public SubscriptionLevelService(CinemaContext context) : base(context) { }

        protected override SubscriptionLevelDisplayViewModel MapToDisplayViewModel(SubscriptionLevel subscriptionLevel)
        {
            return new SubscriptionLevelDisplayViewModel()
            {
                Id = subscriptionLevel.Id,
                Title = subscriptionLevel.Title
            };
        }

        protected override void ValidateInput(SubscriptionLevelCreateViewModel subscriptionLevelData)
        {
            if (string.IsNullOrWhiteSpace(subscriptionLevelData.Title))
            {
                throw new ArgumentException("No value", nameof(subscriptionLevelData.Title));
            }
        }

        protected override void ValidateInput(SubscriptionLevelEditViewModel subscriptionLevelNewData)
        {
            if (string.IsNullOrWhiteSpace(subscriptionLevelNewData.Title))
            {
                throw new ArgumentException("No value", nameof(subscriptionLevelNewData.Title));
            }
        }

        protected override void ValidateEntity(SubscriptionLevelCreateViewModel subscriptionLevelData)
        {
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
        }

        protected override void ValidateEntity(SubscriptionLevelEditViewModel subscriptionLevelNewData)
        {
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
        }

        protected override SubscriptionLevel MapToEntity(SubscriptionLevelCreateViewModel subscriptionLevelData)
        {
            return new SubscriptionLevel()
            {
                Id = Guid.NewGuid(),
                Title = subscriptionLevelData.Title
            };
        }

        protected override void UpdateFieldValues(SubscriptionLevel subscriptionLevel, SubscriptionLevelEditViewModel subscriptionLevelNewData)
        {
            subscriptionLevel.Title = subscriptionLevelNewData.Title;
        }
    }
}
