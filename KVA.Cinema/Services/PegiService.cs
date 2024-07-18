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
    public class PegiService : IService<PegiCreateViewModel, PegiDisplayViewModel, PegiEditViewModel>
    {
        private const int AGE_MIN = 0;

        private const int AGE_MAX = 99;

        private CinemaContext Context { get; }

        public PegiService(CinemaContext db)
        {
            Context = db;
        }

        public IEnumerable<PegiCreateViewModel> Read()
        {
            return Context.Pegis.Select(x => new PegiCreateViewModel()
            {
                Id = x.Id,
                Type = x.Type
            }).ToList();
        }

        public PegiDisplayViewModel Read(Guid pegiId)
        {
            var pegi = Context.Pegis.FirstOrDefault(x => x.Id == pegiId);

            if (pegi == default)
            {
                throw new EntityNotFoundException($"Pegi with id \"{pegiId}\" not found");
            }

            return MapToDisplayViewModel(pegi);
        }

        public IEnumerable<PegiDisplayViewModel> ReadAll()
        {
            return Context.Pegis.Select(x => new PegiDisplayViewModel()
            {
                Id = x.Id,
                Type = x.Type
            }).ToList();
        }

        public void Create(PegiCreateViewModel pegiData)
        {
            if (pegiData.Type is > AGE_MAX or < AGE_MIN)
            {
                throw new ArgumentException($"Value is not valid for age restriction");
            }

            if (Context.Pegis.FirstOrDefault(x => x.Type == pegiData.Type) != default)
            {
                throw new DuplicatedEntityException($"PEGI \"{pegiData.Type}+\" is already exist");
            }

            Pegi newPegi = new Pegi()
            {
                Id = Guid.NewGuid(),
                Type = pegiData.Type
            };

            Context.Pegis.Add(newPegi);
            Context.SaveChanges();
        }

        public void Delete(Guid pegiId)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(pegiId))
            {
                throw new ArgumentNullException("PEGI Id has no value");
            }

            Pegi pegi = Context.Pegis.FirstOrDefault(x => x.Id == pegiId);

            if (pegi == default)
            {
                throw new EntityNotFoundException($"PEGI with Id \"{pegiId}\" not found");
            }

            Context.Pegis.Remove(pegi);
            Context.SaveChanges();
        }

        public void Update(Guid pegiId, PegiEditViewModel pegiNewData)
        {
            if (CheckUtilities.ContainsNullOrEmptyValue(pegiId))
            {
                throw new ArgumentNullException("PEGI id has no value");
            }

            Pegi pegi = Context.Pegis.FirstOrDefault(x => x.Id == pegiId);

            if (pegi == default)
            {
                throw new EntityNotFoundException($"PEGI with id \"{pegiId}\" not found");
            }

            if (pegiNewData.Type is > AGE_MAX or < AGE_MIN)
            {
                throw new ArgumentException($"Value is not valid for age restriction");
            }

            if (Context.Pegis.FirstOrDefault(x => x.Type == pegiNewData.Type && x.Id != pegiNewData.Id) != default)
            {
                throw new DuplicatedEntityException($"PEGI \"{pegiNewData.Type}+\" is already exist");
            }

            pegi.Type = pegiNewData.Type;

            Context.SaveChanges();
        }

        private PegiDisplayViewModel MapToDisplayViewModel(Pegi pegi)
        {
            return new PegiDisplayViewModel()
            {
                Id = pegi.Id,
                Type = pegi.Type
            };
        }
    }
}
