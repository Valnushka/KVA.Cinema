using System;
using System.Collections.Generic;

namespace KVA.Cinema.Services
{
    internal interface IService<TEntityCreateViewModel, TEntityDisplayViewModel, TEntityEditViewModel>
        where TEntityCreateViewModel : class
        where TEntityDisplayViewModel : class
        where TEntityEditViewModel : class
    {
        IEnumerable<TEntityCreateViewModel> Read();

        IEnumerable<TEntityDisplayViewModel> ReadAll();

        void CreateAsync(TEntityCreateViewModel entityData);

        void Update(Guid id, TEntityEditViewModel newEntityData);

        void Delete(Guid id);

        bool IsEntityExist(Guid id);
    }
}
