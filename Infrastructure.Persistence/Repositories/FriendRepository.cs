using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Infrastructure.Persistence.Repositories
{
    class FriendRepository : IFriendRepository
    {
        private readonly SocialMediaContext _mediaContext;
        public FriendRepository(SocialMediaContext mediaContext)
        {
            _mediaContext = mediaContext;
        }
        public async Task<Friends> Add(Friends friends)
        {
            await _mediaContext.Set<Friends>().AddAsync(friends);
            await _mediaContext.SaveChangesAsync();
            return friends;
        }

        public async Task Delete(int id)
        {
            var cs = "Server=.;Database=SocialNetwork;Trusted_Connection=true;MultipleActiveResultSets=True";
            using var con = new SqlConnection(cs);
            await con.OpenAsync();
            var query = @"DELETE FROM Friends where IdFriend = @Id";
            await con.ExecuteAsync(query, new { Id = id });
        }

        public async Task<List<Friends>> GetAll()
        {
            return await _mediaContext.Set<Friends>().ToListAsync();
        }

 
    }
}
