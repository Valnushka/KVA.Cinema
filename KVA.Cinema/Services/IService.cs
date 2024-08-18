using System;
using System.Collections.Generic;

namespace KVA.Cinema.Services
{
    internal interface IService<TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel>
        where TEntityCreateViewModel : class
        where TEntityDisplayViewModel : class
        where TEntityEditViewModel : class
    {
        void Create(TEntityCreateViewModel entityData);

        TEntityDisplayViewModel Read(Guid entityId);

        IEnumerable<TEntityDisplayViewModel> ReadAll();

        void Update(Guid id, TEntityEditViewModel newEntityData);

        void Delete(Guid id);
    }
}
