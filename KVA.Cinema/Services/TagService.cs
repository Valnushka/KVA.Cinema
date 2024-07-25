using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class TagService : BaseService<Tag, TagCreateViewModel, TagDisplayViewModel, TagEditViewModel>
    {
        /// <summary>
        /// Minimum length allowed for tag text
        /// </summary>
        private const int TEXT_LENGHT_MIN = 2;

        /// <summary>
        /// Maximum length allowed for tag text
        /// </summary>
        private const int TEXT_LENGHT_MAX = 20;

        public TagService(CinemaContext context) : base(context) { }

        protected override TagDisplayViewModel MapToDisplayViewModel(Tag tag)
        {
            return new TagDisplayViewModel()
            {
                Id = tag.Id,
                Text = tag.Text,
                Color = tag.Color
            };
        }

        protected override void ValidateInput(TagCreateViewModel tagData)
        {
            if (string.IsNullOrWhiteSpace(tagData.Text))
            {
                throw new ArgumentException("No value", nameof(tagData.Text));
            }

            if (string.IsNullOrWhiteSpace(tagData.Color))
            {
                throw new ArgumentException("No value", nameof(tagData.Color));
            }
        }

        protected override void ValidateInput(TagEditViewModel tagNewData)
        {
            if (string.IsNullOrWhiteSpace(tagNewData.Text))
            {
                throw new ArgumentException("No value", nameof(tagNewData.Text));
            }

            if (string.IsNullOrWhiteSpace(tagNewData.Color))
            {
                throw new ArgumentException("No value", nameof(tagNewData.Color));
            }
        }

        protected override void ValidateEntity(TagCreateViewModel tagData)
        {
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
        }

        protected override void ValidateEntity(TagEditViewModel newTagData)
        {
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
        }

        protected override Tag MapToEntity(TagCreateViewModel tagData)
        {
            return new Tag()
            {
                Id = Guid.NewGuid(),
                Text = tagData.Text,
                Color = tagData.Color
            };
        }

        protected override void UpdateFieldValues(Tag tag, TagEditViewModel newTagData)
        {
            tag.Text = newTagData.Text;
            tag.Color = newTagData.Color;
        }
    }
}
