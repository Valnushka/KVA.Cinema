namespace KVA.Cinema.Services
{
    using KVA.Cinema.Exceptions;
    using KVA.Cinema.Models;
    using KVA.Cinema.Models.Entities;
    using KVA.Cinema.Models.ViewModels.Video;
    using KVA.Cinema.Utilities;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class VideoService : IService<VideoCreateViewModel, VideoDisplayViewModel, VideoEditViewModel>
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

        private CinemaContext Context { get; set; }

        private IWebHostEnvironment HostEnvironment { get; }

        public VideoService(CinemaContext db, IWebHostEnvironment hostEnvironment)
        {
            Context = db;
            HostEnvironment = hostEnvironment;
        }

        public IEnumerable<VideoCreateViewModel> Read()
        {
            List<Video> videos = Context.Videos.ToList();

            return videos.Select(x => new VideoCreateViewModel()
            {
                Name = x.Title,
                Description = x.Description,
                Length = x.Length,
                CountryId = x.CountryId,
                ReleasedIn = x.ReleasedIn,
                Views = x.Views,
                PegiId = x.PegiId,
                LanguageId = x.LanguageId,
                DirectorId = x.DirectorId,
                GenreIds = x.Genres.Select(x => x.Id),
                TagIds = x.Tags.Select(x => x.Id),
                TagsViewModels = x.Tags.Select(x => new Models.ViewModels.Tag.TagDisplayViewModel() { Text = x.Text, Color = x.Color })
            });
        }

        public IEnumerable<VideoDisplayViewModel> ReadAll()
        {
            return Context.Videos.Select(x => new VideoDisplayViewModel()
            {
                Id = x.Id,
                Name = x.Title,
                Description = x.Description,
                Length = x.Length,
                CountryId = x.CountryId,
                ReleasedIn = x.ReleasedIn,
                Views = x.Views,
                PreviewFileName = x.Preview,
                PegiId = x.PegiId,
                LanguageId = x.LanguageId,
                DirectorId = x.DirectorId,
                CountryName = x.Country.Name,
                PegiName = x.Pegi.Type.ToString() + "+",
                LanguageName = x.Language.Name,
                DirectorName = x.Director.Name,
                Genres = x.Genres,
                Tags = x.Tags,
                TagViewModels = x.Tags.Select(x => new Models.ViewModels.Tag.TagDisplayViewModel() { Text = x.Text, Color = x.Color })
            }).ToList();
        }

        public void CreateAsync(VideoCreateViewModel videoData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(videoData,
                                                        videoData.Name,
                                                        videoData.CountryId,
                                                        videoData.ReleasedIn,
                                                        videoData.PegiId,
                                                        videoData.LanguageId,
                                                        videoData.DirectorId,
                                                        videoData.GenreIds))
            {
                throw new ArgumentNullException("One or more required fields have no value");
            }

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

            Guid videoId = Guid.NewGuid();

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

            var genres = Context.Genres.Where(x => videoData.GenreIds.Contains(x.Id)).ToList();

            var tags = new List<Tag>();

            if (videoData.TagIds != null)
            {
                tags = Context.Tags.Where(x => videoData.TagIds.Contains(x.Id)).ToList();
            }

            Video newVideo = new Video()
            {
                Id = videoId,
                Title = videoData.Name,
                Description = videoData.Description,
                Length = 1,
                CountryId = videoData.CountryId,
                ReleasedIn = videoData.ReleasedIn.ToUniversalTime(),
                Views = 0,
                Preview = previewNewName,
                PegiId = videoData.PegiId,
                LanguageId = videoData.LanguageId,
                DirectorId = videoData.DirectorId,
                Genres = genres,
                Tags = tags
            };

            Context.Videos.Add(newVideo);
            Context.SaveChanges();
        }

        public void Delete(Guid videoId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(videoId))
            {
                throw new ArgumentNullException("Video Id has no value");
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
                var previewFolderPath = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);
                var previewFullPath = previewFolderPath + "\\" + preview;

                File.Delete(previewFullPath);
            }
        }

        public void Update(Guid videoId, VideoEditViewModel newVideoData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(videoId,
                                                        newVideoData,
                                                        newVideoData.Name,
                                                        newVideoData.CountryId,
                                                        newVideoData.ReleasedIn,
                                                        newVideoData.PegiId,
                                                        newVideoData.LanguageId,
                                                        newVideoData.DirectorId,
                                                        newVideoData.GenreIds))
            {
                throw new ArgumentNullException("One or more required fields have no value");
            }

            Video video = Context.Videos.FirstOrDefault(x => x.Id == videoId);

            if (video == default)
            {
                throw new EntityNotFoundException($"Video with id \"{videoId}\" not found");
            }

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

            if (oldPreview == null || newVideoData.IsResetPreviewButtonClicked || newPreviewName != null)
            {
                video.Preview = newPreviewName;
            }
            else
            {
                video.Preview = oldPreview;
            }

            newVideoData.GenreIds ??= Enumerable.Empty<Guid>();
            newVideoData.TagIds ??= Enumerable.Empty<Guid>();

            List<Genre> genresOldList = video.Genres.ToList();

            var contextGenreIds = genresOldList.Select(x => x.Id);
            var genreRecordsToDelete = genresOldList.Where(x => !newVideoData.GenreIds.Contains(x.Id));
            var genreIdsToAdd = newVideoData.GenreIds.Where(x => !contextGenreIds.Contains(x));

            IEnumerable<Genre> genresToAdd = Context.Genres.Where(x => genreIdsToAdd.Contains(x.Id));

            foreach (var record in genreRecordsToDelete)
            {
                video.Genres.Remove(record);
            }

            foreach (var genre in genresToAdd)
            {
                video.Genres.Add(genre);
            }

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

            video.Title = newVideoData.Name;
            video.Description = newVideoData.Description;
            video.Length = 1;
            video.CountryId = newVideoData.CountryId;
            video.ReleasedIn = newVideoData.ReleasedIn.ToUniversalTime();
            video.Views = video.Views;
            video.PegiId = newVideoData.PegiId;
            video.LanguageId = newVideoData.LanguageId;
            video.DirectorId = newVideoData.DirectorId;

            Context.SaveChanges();

            if ((newVideoData.IsResetPreviewButtonClicked || newPreviewName != null) && oldPreview != null)
            {
                var previewFolderPath = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);
                var oldPreviewFullPath = previewFolderPath + "\\" + oldPreview;

                File.Delete(oldPreviewFullPath);
            }
        }

        public bool IsEntityExist(Guid videoId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(videoId))
            {
                return false;
            }

            Video video = Context.Videos.FirstOrDefault(x => x.Id == videoId);

            return video != default;
        }

        private string SaveFile(IFormFile file, string destinationFolderName)
        {
            if (file == null || string.IsNullOrWhiteSpace(destinationFolderName))
            {
                throw new ArgumentNullException("Invalid argument");
            }

            DirectoryInfo drInfo = new DirectoryInfo(destinationFolderName);

            if (!drInfo.Exists)
            {
                drInfo.Create();
            }

            string fileNewName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            string pathToFile = Path.Combine(destinationFolderName, fileNewName);

            using (var stream = new FileStream(pathToFile, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileNewName;
        }
    }
}
