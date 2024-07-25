using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Entities;
using KVA.Cinema.ViewModels;
using System;
using System.Linq;

namespace KVA.Cinema.Services
{
    public class PegiService : BaseService<Pegi, PegiCreateViewModel, PegiDisplayViewModel, PegiEditViewModel>
    {
        private const int AGE_MIN = 0;

        private const int AGE_MAX = 99;

        public PegiService(CinemaContext context) : base(context) { }

        protected override PegiDisplayViewModel MapToDisplayViewModel(Pegi pegi)
        {
            return new PegiDisplayViewModel()
            {
                Id = pegi.Id,
                Type = pegi.Type
            };
        }

        protected override void ValidateInput(PegiCreateViewModel pegiData)
        {
            if (pegiData.Type == default)
            {
                throw new ArgumentException("No value", nameof(pegiData.Type));
            }
        }

        protected override void ValidateInput(PegiEditViewModel pegiNewData)
        {
            if (pegiNewData.Type == default)
            {
                throw new ArgumentException("No value", nameof(pegiNewData.Type));
            }
        }

        protected override void ValidateEntity(PegiCreateViewModel pegiData)
        {
            if (pegiData.Type is > AGE_MAX or < AGE_MIN)
            {
                throw new ArgumentException($"Value is not valid for age restriction");
            }

            if (Context.Pegis.FirstOrDefault(x => x.Type == pegiData.Type) != default)
            {
                throw new DuplicatedEntityException($"PEGI \"{pegiData.Type}+\" is already exist");
            }
        }

        protected override void ValidateEntity(PegiEditViewModel pegiNewData)
        {
            if (pegiNewData.Type is > AGE_MAX or < AGE_MIN)
            {
                throw new ArgumentException($"Value is not valid for age restriction");
            }

            if (Context.Pegis.FirstOrDefault(x => x.Type == pegiNewData.Type && x.Id != pegiNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"PEGI \"{pegiNewData.Type}+\" is already exist");
            }
        }

        protected override Pegi MapToEntity(PegiCreateViewModel pegiData)
        {
            return new Pegi()
            {
                Id = Guid.NewGuid(),
                Type = pegiData.Type
            };
        }

        protected override void UpdateFieldValues(Pegi pegi, PegiEditViewModel pegiNewData)
        {
            pegi.Type = pegiNewData.Type;
        }
    }
}
