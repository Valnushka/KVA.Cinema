using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using KVA.Cinema.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class SubscriptionService : BaseService<Subscription, SubscriptionCreateViewModel, SubscriptionDisplayViewModel, SubscriptionEditViewModel>
    {
        private const int DURATION_DAYS_MIN = 1;
        private const int DURATION_DAYS_MAX = 366;

        public SubscriptionService(CinemaContext context) : base(context) { }

        public override void Create(SubscriptionCreateViewModel subscriptionData)
        {
            ValidateInput(subscriptionData);
            ValidateEntity(subscriptionData);

            Subscription newSubscription = MapToEntity(subscriptionData);

            if (subscriptionData.VideoIds != null)
            {
                List<VideoInSubscription> videoInSubscriptionList = new List<VideoInSubscription>();

                foreach (var videoId in subscriptionData.VideoIds)
                {
                    videoInSubscriptionList.Add(new VideoInSubscription()
                    {
                        Id = Guid.NewGuid(),
                        SubscriptionId = newSubscription.Id,
                        VideoId = videoId
                    });
                }

                newSubscription.VideoInSubscriptions = videoInSubscriptionList;
                Context.VideoInSubscriptions.AddRange(videoInSubscriptionList);
            }

            Context.Subscriptions.Add(newSubscription);
            Context.SaveChanges();
        }

        public override void Update(Guid subscriptionId, SubscriptionEditViewModel subscriptionNewData)
        {
            if (subscriptionId == default)
            {
                throw new ArgumentException($"Subscription with id \"{subscriptionId}\" not found");
            }

            ValidateInput(subscriptionNewData);

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            if (subscription == default)
            {
                throw new EntityNotFoundException($"Subscription with id \"{subscriptionId}\" not found");
            }

            List<VideoInSubscription> videoInSubscriptionList = Context.VideoInSubscriptions.Where(x => x.SubscriptionId == subscriptionNewData.Id).ToList();

            var subscriptionNewDataVideoIds = subscriptionNewData.VideoIds;
            var contextVideoIds = videoInSubscriptionList.Select(x => x.VideoId);

            var recordsToDelete = videoInSubscriptionList.Where(x => !subscriptionNewDataVideoIds.Contains(x.VideoId));
            var videoIdsToAdd = subscriptionNewData.VideoIds.Where(x => !contextVideoIds.Contains(x));

            foreach (var record in recordsToDelete)
            {
                Context.VideoInSubscriptions.Remove(record);
            }

            foreach (var videoId in videoIdsToAdd)
            {
                Context.VideoInSubscriptions.Add(new VideoInSubscription()
                {
                    Id = Guid.NewGuid(),
                    SubscriptionId = subscription.Id,
                    VideoId = videoId
                });
            }

            Context.SaveChanges();
        }

        protected override void ValidateInput(SubscriptionCreateViewModel subscriptionData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionData.Title,
                                                        subscriptionData.Description,
                                                        subscriptionData.Cost,
                                                        subscriptionData.LevelId,
                                                        subscriptionData.ReleasedIn,
                                                        subscriptionData.Duration,
                                                        subscriptionData.AvailableUntil))
            {
                throw new ArgumentException("One or more parameters have no value");
            }
        }

        protected override void ValidateInput(SubscriptionEditViewModel subscriptionData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionData.Title,
                                                        subscriptionData.Description,
                                                        subscriptionData.Cost,
                                                        subscriptionData.LevelId,
                                                        subscriptionData.ReleasedIn,
                                                        subscriptionData.Duration,
                                                        subscriptionData.AvailableUntil))
            {
                throw new ArgumentException("One or more parameters have no value");
            }
        }

        protected override void ValidateEntity(SubscriptionCreateViewModel subscriptionData)
        {
            ValidateDuration(subscriptionData.Duration);

            if (Context.Subscriptions.FirstOrDefault(x => x.Title == subscriptionData.Title) != default)
            {
                throw new DuplicatedEntityException($"Subscription with title \"{subscriptionData.Title}\" is already exist");
            }
        }

        protected override void ValidateEntity(SubscriptionEditViewModel subscriptionNewData)
        {
            ValidateDuration(subscriptionNewData.Duration);

            if (Context.Subscriptions.FirstOrDefault(x => x.Title == subscriptionNewData.Title && x.Id != subscriptionNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Subscription with title \"{subscriptionNewData.Title}\" is already exist");
            }
        }

        protected override Subscription MapToEntity(SubscriptionCreateViewModel subscriptionData)
        {
            return new Subscription()
            {
                Id = Guid.NewGuid(),
                Title = subscriptionData.Title,
                Description = subscriptionData.Description,
                Cost = subscriptionData.Cost,
                LevelId = subscriptionData.LevelId,
                ReleasedIn = subscriptionData.ReleasedIn,
                Duration = subscriptionData.Duration,
                AvailableUntil = subscriptionData.AvailableUntil
            };
        }

        protected override SubscriptionDisplayViewModel MapToDisplayViewModel(Subscription subscription)
        {
            return new SubscriptionDisplayViewModel()
            {
                Id = subscription.Id,
                Title = subscription.Title,
                Description = subscription.Description,
                Cost = subscription.Cost,
                LevelId = subscription.LevelId,
                LevelName = subscription.Level.Title,
                ReleasedIn = subscription.ReleasedIn,
                Duration = subscription.Duration,
                AvailableUntil = subscription.AvailableUntil,
                VideosInSubscription = subscription.VideoInSubscriptions,
                VideoNames = subscription.VideoInSubscriptions.Select(y => y.Video.Title)
            };
        }

        protected override void UpdateFieldValues(Subscription subscription, SubscriptionEditViewModel subscriptionNewData)
        {
            subscription.Title = subscriptionNewData.Title;
            subscription.Description = subscriptionNewData.Description;
            subscription.Cost = subscriptionNewData.Cost;
            subscription.LevelId = subscriptionNewData.LevelId;
            subscription.ReleasedIn = subscriptionNewData.ReleasedIn;
            subscription.Duration = subscriptionNewData.Duration;
            subscription.AvailableUntil = subscriptionNewData.AvailableUntil;
        }

        private void ValidateDuration(int durationInDays)
        {
            if (durationInDays < DURATION_DAYS_MIN)
            {
                throw new ArgumentException($"Duration cannot be less than {DURATION_DAYS_MIN} day(s)");
            }

            if (durationInDays > DURATION_DAYS_MAX)
            {
                throw new ArgumentException($"Duration cannot be more than {DURATION_DAYS_MAX} day(s)");
            }
        }
    }
}
