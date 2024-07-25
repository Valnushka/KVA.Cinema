using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class VideoService : BaseService<Video, VideoCreateViewModel, VideoDisplayViewModel, VideoEditViewModel>
    {
        /// <summary>
        /// Maximum length allowed for title
        /// </summary>
        private const int TITLE_LENGHT_MAX = 128;

        private const string POSTER_UPLOAD_PATH = "upload/videoPreview";

        /// <summary>
        /// Maximum size allowed for preview in bytes
        /// </summary>
        private const int MAX_PREVIEW_SIZE = 25_000_000; // 25 MB

        private IWebHostEnvironment HostEnvironment { get; }

        public VideoService(CinemaContext context, IWebHostEnvironment hostEnvironment) : base(context)
        {
            HostEnvironment = hostEnvironment;
        }

        public override void Create(VideoCreateViewModel videoData)
        {
            if (videoData == default)
            {
                throw new ArgumentNullException(nameof(videoData), "No value");
            }

            ValidateInput(videoData);
            ValidateEntity(videoData);

            string previewNewName = null;

            if (videoData.Preview != null)
            {
                if (videoData.Preview.Length > MAX_PREVIEW_SIZE)
                {
                    throw new ArgumentOutOfRangeException($"File is too big");
                }

                string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);

                previewNewName = SaveFile(videoData.Preview, uploadsFolder);
            }

            Video newVideo = MapToEntity(videoData);

            newVideo.Preview = previewNewName;

            Context.Videos.Add(newVideo);
            Context.SaveChanges();
        }

        public override void Delete(Guid videoId)
        {
            if (videoId == default)
            {
                throw new ArgumentException("No value", nameof(videoId));
            }

            Video video = Context.Videos.FirstOrDefault(x => x.Id == videoId);

            if (video == default)
            {
                throw new EntityNotFoundException($"Video with Id \"{videoId}\" not found");
            }

            var preview = video.Preview;

            Context.Videos.Remove(video);
            Context.SaveChanges();

            if (preview != null)
            {
                DeletePreview(preview);
            }
        }

        public override void Update(Guid videoId, VideoEditViewModel newVideoData)
        {
            if (videoId == default)
            {
                throw new ArgumentException("No value", nameof(videoId));
            }

            if (newVideoData == default)
            {
                throw new ArgumentException("No value", nameof(newVideoData));
            }

            ValidateInput(newVideoData);

            Video video = Context.Videos.FirstOrDefault(x => x.Id == videoId);

            if (video == default)
            {
                throw new EntityNotFoundException($"Video with id \"{videoId}\" not found");
            }

            string newPreviewName = null;

            if (newVideoData.Preview != null)
            {
                if (newVideoData.Preview.Length > MAX_PREVIEW_SIZE)
                {
                    throw new ArgumentOutOfRangeException($"File is too big");
                }

                string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);

                newPreviewName = SaveFile(newVideoData.Preview, uploadsFolder);
            }

            var oldPreview = video.Preview;

            video.Preview = oldPreview == null || newVideoData.IsResetPreviewButtonClicked || newPreviewName != null ? newPreviewName : oldPreview;


            UpdateFieldValues(video, newVideoData);

            Context.SaveChanges();

            if ((newVideoData.IsResetPreviewButtonClicked || newPreviewName != null) && oldPreview != null)
            {
                DeletePreview(oldPreview);
            }
        }

        private string SaveFile(IFormFile file, string destinationFolderName)
        {
            if (file == null || string.IsNullOrWhiteSpace(destinationFolderName))
            {
                throw new ArgumentException("Null or empty argument");
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(destinationFolderName);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            string fileNewName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            string pathToFile = Path.Combine(destinationFolderName, fileNewName);

            using (var stream = new FileStream(pathToFile, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileNewName;
        }

        protected override void ValidateInput(VideoCreateViewModel videoData)
        {
            if (string.IsNullOrWhiteSpace(videoData.Name))
            {
                throw new ArgumentException("Invalid argument", nameof(videoData.Name));
            }

            Guid[] guidFields = new Guid[]
            {
                videoData.CountryId,
                videoData.PegiId,
                videoData.LanguageId,
                videoData.DirectorId,

            };

            foreach (var id in guidFields)
            {
                if (id == default)
                {
                    throw new ArgumentException("Argument has no value");
                }
            }

            foreach (var id in videoData.GenreIds)
            {
                if (id == default)
                {
                    throw new ArgumentException("Argument has no value");
                }
            }

            if (videoData.ReleasedIn == default)
            {
                throw new ArgumentException("Invalid argument", nameof(videoData.ReleasedIn));
            }
        }

        protected override void ValidateInput(VideoEditViewModel videoNewData)
        {
            if (string.IsNullOrWhiteSpace(videoNewData.Name))
            {
                throw new ArgumentException("Invalid argument", nameof(videoNewData.Name));
            }

            Guid[] guidFields = new Guid[]
            {
                videoNewData.CountryId,
                videoNewData.PegiId,
                videoNewData.LanguageId,
                videoNewData.DirectorId,

            };

            foreach (var id in guidFields)
            {
                if (id == default)
                {
                    throw new ArgumentException("Argument has no value");
                }
            }

            foreach (var id in videoNewData.GenreIds)
            {
                if (id == default)
                {
                    throw new ArgumentException("Argument has no value");
                }
            }

            if (videoNewData.ReleasedIn == default)
            {
                throw new ArgumentException("Invalid argument", nameof(videoNewData.ReleasedIn));
            }
        }

        protected override void ValidateEntity(VideoCreateViewModel videoData)
        {
            if (videoData.Name.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Title length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (videoData.ReleasedIn.ToUniversalTime() > DateTime.UtcNow)
            {
                throw new ArgumentException($"Only released video can be uploaded");
            }

            if (Context.Videos.FirstOrDefault(x => x.Title == videoData.Name && x.DirectorId == videoData.DirectorId) != default)
            {
                throw new DuplicatedEntityException($"Video with title \"{videoData.Name}\" by this director is already exist");
            }
        }

        protected override void ValidateEntity(VideoEditViewModel newVideoData)
        {
            if (newVideoData.Name.Length > TITLE_LENGHT_MAX)
            {
                throw new ArgumentException($"Title length cannot be more than {TITLE_LENGHT_MAX} symbols");
            }

            if (newVideoData.ReleasedIn.ToUniversalTime() > DateTime.UtcNow)
            {
                throw new ArgumentException($"Only released video can be uploaded");
            }

            Video duplicate = Context.Videos.FirstOrDefault(x =>
                                                               x.Title == newVideoData.Name &&
                                                               x.DirectorId == newVideoData.DirectorId &&
                                                               x.Id != newVideoData.Id);
            if (duplicate != default)
            {
                throw new DuplicatedEntityException($"Video with title \"{newVideoData.Name}\" by this director is already exist");
            }
        }

        protected override Video MapToEntity(VideoCreateViewModel videoData)
        {
            return new Video()
            {
                Id = Guid.NewGuid(),
                Title = videoData.Name,
                Description = videoData.Description,
                Length = 1,
                CountryId = videoData.CountryId,
                ReleasedIn = videoData.ReleasedIn.ToUniversalTime(),
                Views = 0,
                PegiId = videoData.PegiId,
                LanguageId = videoData.LanguageId,
                DirectorId = videoData.DirectorId,
                Genres = Context.Genres.Where(x => videoData.GenreIds.Contains(x.Id)).ToList(),
                Tags = videoData.TagIds != null ? Context.Tags.Where(x => videoData.TagIds.Contains(x.Id)).ToList() : null
            };
        }

        protected override VideoDisplayViewModel MapToDisplayViewModel(Video video)
        {
            return new VideoDisplayViewModel()
            {
                Id = video.Id,
                Name = video.Title,
                Description = video.Description,
                Length = video.Length,
                CountryId = video.CountryId,
                ReleasedIn = video.ReleasedIn,
                Views = video.Views,
                PreviewFileName = video.Preview,
                PegiId = video.PegiId,
                LanguageId = video.LanguageId,
                DirectorId = video.DirectorId,
                CountryName = video.Country.Name,
                PegiName = video.Pegi.Type.ToString() + "+",
                LanguageName = video.Language.Name,
                DirectorName = video.Director.Name,
                Genres = video.Genres,
                Tags = video.Tags,
                TagViewModels = video.Tags.Select(x => new TagDisplayViewModel() { Text = x.Text, Color = x.Color })
            };
        }

        protected override void UpdateFieldValues(Video video, VideoEditViewModel newVideoData)
        {
            video.Title = newVideoData.Name;
            video.Description = newVideoData.Description;
            video.Length = 1;
            video.CountryId = newVideoData.CountryId;
            video.ReleasedIn = newVideoData.ReleasedIn.ToUniversalTime();
            video.Views = video.Views;
            video.PegiId = newVideoData.PegiId;
            video.LanguageId = newVideoData.LanguageId;
            video.DirectorId = newVideoData.DirectorId;

            UpdateRelatedGenres(video, newVideoData);
            UpdateRelatedTags(video, newVideoData);
        }

        private void UpdateRelatedGenres(Video video, VideoEditViewModel newVideoData)
        {
            newVideoData.GenreIds ??= Enumerable.Empty<Guid>();

            List<Genre> genresOldList = video.Genres.ToList();

            var genreRecordsToDelete = genresOldList.Where(x => !newVideoData.GenreIds.Contains(x.Id));

            foreach (var record in genreRecordsToDelete)
            {
                video.Genres.Remove(record);
            }

            var contextGenreIds = genresOldList.Select(x => x.Id);

            var genreIdsToAdd = newVideoData.GenreIds.Where(x => !contextGenreIds.Contains(x));

            IEnumerable<Genre> genresToAdd = Context.Genres.Where(x => genreIdsToAdd.Contains(x.Id));

            foreach (var genre in genresToAdd)
            {
                video.Genres.Add(genre);
            }
        }

        private void UpdateRelatedTags(Video video, VideoEditViewModel newVideoData)
        {
            newVideoData.TagIds ??= Enumerable.Empty<Guid>();

            List<Tag> tagsOldList = video.Tags.ToList();

            var contextTagIds = tagsOldList.Select(x => x.Id);
            var tagRecordsToDelete = tagsOldList.Where(x => !newVideoData.TagIds.Contains(x.Id));
            var tagIdsToAdd = newVideoData.TagIds.Where(x => !contextTagIds.Contains(x));

            IEnumerable<Tag> tagsToAdd = Context.Tags.Where(x => tagIdsToAdd.Contains(x.Id));

            foreach (var record in tagRecordsToDelete)
            {
                video.Tags.Remove(record);
            }

            foreach (var tag in tagsToAdd)
            {
                video.Tags.Add(tag);
            }
        }

        private void DeletePreview(string preview)
        {
            var previewFolderPath = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);
            var previewFullPath = previewFolderPath + "\\" + preview;

            File.Delete(previewFullPath);
        }
    }
}
