using DevBudy.CONTRACT.Repositories.RedisRepositories;
using DevBudy.DOMAIN.Entities.Concretes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBudy.INNERINFRASTRUCTURE.Redis
{
    public class UserQuotaInitializer
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMessageQuotaRepository _messageQuotaRepository;

        public UserQuotaInitializer(IMessageQuotaRepository messageQuotaRepository, UserManager<AppUser> userManager)
        {
            _messageQuotaRepository = messageQuotaRepository;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                await _messageQuotaRepository.InitializeUserMessageQuotaAsync(user.Id.ToString());
            }
        }
    }
}
