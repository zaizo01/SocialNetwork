using Core.Application.ViewModels.Friends;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repositories
{
    public interface IFriendRepository
    {
        Task<Friends> Add(Friends type);
        Task Delete(int id);
        Task<List<Friends>> GetAll();
    }
}
