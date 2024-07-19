using KVA.Cinema.Exceptions;
using KVA.Cinema.Models;
using KVA.Cinema.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KVA.Cinema.Services
{
    public abstract class BaseService<TEntity, TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel>
        : IService<TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel>
        where TEntity : class, IEntity
        where TEntityCreateViewModel : class
        where TEntityDisplayViewModel : class
        where TEntityEditViewModel : class
    {
        protected CinemaContext Context { get; }

        public BaseService(CinemaContext context)
        {
            Context = context;
        }

        public virtual TEntityDisplayViewModel Read(Guid entityId)
        {
            if (entityId == default)
            {
                throw new ArgumentNullException(nameof(entityId), "No value");
            }

            TEntity entity = Context.Set<TEntity>().FirstOrDefault(x => x.Id == entityId);

            if (entity == default)
            {
                throw new EntityNotFoundException($"Entity with id \"{entityId}\" not found");
            }

            return MapToDisplayViewModel(entity);
        }

        public virtual IEnumerable<TEntityDisplayViewModel> ReadAll()
        {
            return Context.Set<TEntity>()
                .ToList()
                .Select(x => MapToDisplayViewModel(x))
                ;
        }

        public virtual void Create(TEntityCreateViewModel entityData)
        {
            if (entityData == default)
            {
                throw new ArgumentNullException(nameof(entityData), "No value");
            }

            ValidateInput(entityData);
            ValidateEntity(entityData);

            TEntity newEntity = MapToEntity(entityData);

            Context.Set<TEntity>().Add(newEntity);
            Context.SaveChanges();
        }

        public virtual void Update(Guid entityId, TEntityEditViewModel entityNewData)
        {
            if (entityId == default)
            {
                throw new ArgumentNullException(nameof(entityId), "No value");
            }

            if (entityNewData == default)
            {
                throw new ArgumentNullException(nameof(entityNewData), "No value");
            }

            ValidateInput(entityNewData);

            TEntity entity = Context.Set<TEntity>().FirstOrDefault(x => x.Id == entityId);

            if (entity == default)
            {
                throw new EntityNotFoundException($"Entity with id \"{entityId}\" not found");
            }

            ValidateEntity(entityNewData);
            UpdateFieldValues(entity, entityNewData);

            Context.SaveChanges();
        }

        public virtual void Delete(Guid entityId)
        {
            if (entityId == default)
            {
                throw new ArgumentNullException("Id has no value");
            }

            TEntity entity = Context.Set<TEntity>().FirstOrDefault(x => x.Id == entityId);

            if (entity == default)
            {
                throw new EntityNotFoundException($"Entity with id \"{entityId}\" not found");
            }

            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }

        protected abstract void ValidateInput(TEntityCreateViewModel entityData);

        protected abstract void ValidateInput(TEntityEditViewModel entityNewData);

        protected abstract void ValidateEntity(TEntityCreateViewModel entityData);

        protected abstract void ValidateEntity(TEntityEditViewModel entityNewData);

        protected abstract TEntity MapToEntity(TEntityCreateViewModel entityData);

        protected abstract TEntityDisplayViewModel MapToDisplayViewModel(TEntity entity);

        protected abstract void UpdateFieldValues(TEntity entity, TEntityEditViewModel entityNewData);
    }
}
