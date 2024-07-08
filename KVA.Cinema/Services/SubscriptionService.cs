namespace KVA.Cinema.Services
{
    using KVA.Cinema.Exceptions;
    using KVA.Cinema.Models;
    using KVA.Cinema.Models.Entities;
    using KVA.Cinema.Models.ViewModels.Subscription;
    using KVA.Cinema.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SubscriptionService : IService<SubscriptionCreateViewModel, SubscriptionDisplayViewModel, SubscriptionEditViewModel>
    {
        private const int DURATION_DAYS_MIN = 1;
        private const int DURATION_DAYS_MAX = 366;

        public CinemaContext Context { get; }

        public SubscriptionService(CinemaContext db)
        {
            Context = db;
        }

        public IEnumerable<SubscriptionCreateViewModel> Read()
        {
            return Context.Subscriptions.Select(x => new SubscriptionCreateViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Cost = x.Cost,
                LevelId = x.LevelId,
                ReleasedIn = x.ReleasedIn,
                Duration = x.Duration,
                AvailableUntil = x.AvailableUntil,
            }).ToList();
        } //TODO: remove

        public SubscriptionDisplayViewModel Read(Guid subscriptionId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionId))
            {
                throw new ArgumentNullException("Id has no value");
            }

            return Context.Subscriptions.Where(x => x.Id == subscriptionId).Select(x => new SubscriptionDisplayViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Cost = x.Cost,
                LevelId = x.LevelId,
                ReleasedIn = x.ReleasedIn,
                Duration = x.Duration,
                AvailableUntil = x.AvailableUntil,
                LevelName = x.Level.Title,
                VideoNames = x.VideoInSubscriptions.Select(y => y.Video.Title)
            }).FirstOrDefault();
        }

        public IEnumerable<SubscriptionDisplayViewModel> ReadAll()
        {
            return Context.Subscriptions.Select(x => new SubscriptionDisplayViewModel()  //AutoMapper
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Cost = x.Cost,
                LevelId = x.LevelId,
                LevelName = x.Level.Title,
                ReleasedIn = x.ReleasedIn,
                Duration = x.Duration,
                AvailableUntil = x.AvailableUntil,
                VideosInSubscription = x.VideoInSubscriptions,
                VideoNames = x.VideoInSubscriptions.Select(y => y.Video.Title)
            }).ToList();
        }

        public void CreateAsync(SubscriptionCreateViewModel subscriptionData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionData.Title,
                                                        subscriptionData.Description,
                                                        subscriptionData.Cost,
                                                        subscriptionData.LevelId,
                                                        subscriptionData.ReleasedIn,
                                                        subscriptionData.Duration,
                                                        subscriptionData.AvailableUntil))
            {
                throw new ArgumentNullException("One or more parameters have no value");
            }

            if (subscriptionData.Duration < DURATION_DAYS_MIN)
            {
                throw new ArgumentException($"Duration cannot be less than {DURATION_DAYS_MIN} day(s)");
            }

            if (subscriptionData.Duration > DURATION_DAYS_MAX)
            {
                throw new ArgumentException($"Duration cannot be more than {DURATION_DAYS_MAX} day(s)");
            }

            if (Context.Subscriptions.FirstOrDefault(x => x.Title == subscriptionData.Title) != default)
            {
                throw new DuplicatedEntityException($"Subscription with title \"{subscriptionData.Title}\" is already exist");
            }

            Subscription newSubscription = new Subscription()
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

        public void Delete(Guid subscriptionId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionId))
            {
                throw new ArgumentNullException("Id has no value");
            }

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            if (subscription == default)
            {
                throw new EntityNotFoundException($"Subscription with Id \"{subscriptionId}\" not found");
            }

            Context.Subscriptions.Remove(subscription);
            Context.SaveChanges();
        }

        public void Update(Guid subscriptionId, SubscriptionEditViewModel subscriptionNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionId,
                                                        subscriptionNewData.Title,
                                                        subscriptionNewData.Description,
                                                        subscriptionNewData.Cost,
                                                        subscriptionNewData.LevelId,
                                                        subscriptionNewData.ReleasedIn,
                                                        subscriptionNewData.Duration,
                                                        subscriptionNewData.AvailableUntil))
            {

                throw new ArgumentNullException("One or more parameters have no value");
            }

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            if (subscription == default)
            {
                throw new EntityNotFoundException($"Subscription with id \"{subscriptionId}\" not found");
            }

            if (subscriptionNewData.Duration < DURATION_DAYS_MIN)
            {
                throw new ArgumentException($"Duration cannot be less than {DURATION_DAYS_MIN} day(s)");
            }

            if (subscriptionNewData.Duration > DURATION_DAYS_MAX)
            {
                throw new ArgumentException($"Duration cannot be more than {DURATION_DAYS_MAX} day(s)");
            }

            if (Context.Subscriptions.FirstOrDefault(x => x.Title == subscriptionNewData.Title && x.Id != subscriptionNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"Subscription with title \"{subscriptionNewData.Title}\" is already exist");
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

            subscription.Title = subscriptionNewData.Title;
            subscription.Description = subscriptionNewData.Description;
            subscription.Cost = subscriptionNewData.Cost;
            subscription.LevelId = subscriptionNewData.LevelId;
            subscription.ReleasedIn = subscriptionNewData.ReleasedIn;
            subscription.Duration = subscriptionNewData.Duration;
            subscription.AvailableUntil = subscriptionNewData.AvailableUntil;

            Context.SaveChanges();
        }

        public bool IsEntityExist(Guid subscriptionId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(subscriptionId))
            {
                return false;
            }

            Subscription subscription = Context.Subscriptions.FirstOrDefault(x => x.Id == subscriptionId);

            return subscription != default;
        }
    }
}
