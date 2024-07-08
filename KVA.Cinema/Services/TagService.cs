using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using KVA.Cinema.Models.ViewModels.Tag;
using KVA.Cinema.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class TagService : IService<TagCreateViewModel, TagDisplayViewModel, TagEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for tag text
        /// </summary>
        private const int TEXT_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for tag text
        /// </summary>
        private const int TEXT_LENGHT_MAX = 20;

        private CinemaContext Context { get; }

        public TagService(CinemaContext db)
        {
            Context = db;
        }

        public IEnumerable<TagCreateViewModel> Read()
        {
            List<Tag> tags = Context.Tags.ToList(); //TODO: перенести ToList в return

            return tags.Select(x => new TagCreateViewModel()
            {
                Text = x.Text,
                Color = x.Color
            });
        }

        public IEnumerable<TagDisplayViewModel> ReadAll()
        {
            List<Tag> tags = Context.Tags.ToList(); //TODO: перенести ToList в return

            return tags.Select(x => new TagDisplayViewModel()
            {
                Id = x.Id,
                Text = x.Text,
                Color = x.Color
            });
        }

        public void CreateAsync(TagCreateViewModel tagData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(tagData.Text, tagData.Color))
            {
                throw new ArgumentNullException("One or more parameter has no value");
            }

            if (tagData.Text.Length < TEXT_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TEXT_LENGHT_MIN} symbols");
            }

            if (tagData.Text.Length > TEXT_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TEXT_LENGHT_MAX} symbols");
            }

            if (Context.Tags.FirstOrDefault(x => x.Text == tagData.Text) != default)
            {
                throw new DuplicatedEntityException($"Tag \"{tagData.Text}\" is already exist");
            }

            Tag newTag = new Tag()
            {
                Id = Guid.NewGuid(),
                Text = tagData.Text,
                Color = tagData.Color
            };

            Context.Tags.Add(newTag);
            Context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(id))
            {
                throw new ArgumentNullException("Tag Id has no value");
            }

            Tag tag = Context.Tags.FirstOrDefault(x => x.Id == id);

            if (tag == default)
            {
                throw new EntityNotFoundException($"Tag with Id \"{id}\" not found");
            }

            Context.Tags.Remove(tag);
            Context.SaveChanges();
        }

        public void Update(Guid id, TagEditViewModel newTagData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(id, newTagData, newTagData.Text, newTagData.Color))
            {
                throw new ArgumentNullException("One or more parameter has no value");
            }

            Tag tag = Context.Tags.FirstOrDefault(x => x.Id == id);

            if (tag == default)
            {
                throw new EntityNotFoundException($"Tag with id \"{id}\" not found");
            }

            if (newTagData.Text.Length < TEXT_LENGHT_MIN)
            {
                throw new ArgumentException($"Length cannot be less than {TEXT_LENGHT_MIN} symbols");
            }

            if (newTagData.Text.Length > TEXT_LENGHT_MAX)
            {
                throw new ArgumentException($"Length cannot be more than {TEXT_LENGHT_MAX} symbols");
            }

            if (Context.Tags.FirstOrDefault(x => x.Text == newTagData.Text) != default)
            {
                throw new DuplicatedEntityException($"Tag \"{newTagData.Text}\" is already exist");
            }

            tag.Text = newTagData.Text;
            tag.Color = newTagData.Color;

            Context.SaveChanges();
        }

        public bool IsEntityExist(Guid id)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(id))
            {
                return false;
            }

            Tag tag = Context.Tags.FirstOrDefault(x => x.Id == id);

            return tag != default;
        }
    }
}
