using AutoMapper;
using Core.Application.Dtos.Email;
using Core.Application.Helpers;
using Core.Application.Interfaces.Repositories;
using Core.Application.Interfaces.Services;
using Core.Application.ViewModels.User;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Services
{
    public class UserService : GenericService<User,UserSaveViewModel,UserViewModel>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository,IMapper mapper,IEmailService emailService):base(userRepository,mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;

        } 
    public async Task<UserViewModel> Login(LoginViewModel loginViewModel)
        {
            User user = await _userRepository.Login(loginViewModel);
            if(user == null)
            {
                return null;
            }
            UserViewModel userView = _mapper.Map<UserViewModel>(user);

            return userView;
        }
  
        public override async Task<UserSaveViewModel> Add(UserSaveViewModel type)
        {
            var users = await _userRepository.GetAll();
            users = users.Where(user => user.UserName == type.UserName).ToList();
            if(users.Count == 0)
            {

                UserSaveViewModel userSaveViewModel = await base.Add(type);
                await _emailService.SendEmail(new EmailRequest
                {
                    To = userSaveViewModel.Mail,
                    Subject ="Confirmacion de Email",
                    Body = $"Haz click en el enlace para activar tu cuenta https://localhost:44352/User/ConfirmUser/{userSaveViewModel.Id}"
                });
                return userSaveViewModel;
             }
            return null;
        }
        public override Task Update(UserSaveViewModel type, int id)
        {
            
            return base.Update(type, id);
        }
        public async Task ConfirmMail(UserSaveViewModel userVm)
        {
            User user = _mapper.Map<User>(userVm);
            user.IsActiveUser = true;
            await _userRepository.Update(user,user.Id);
        }
        public async Task RestorePasswordMail(UserSaveViewModel userVm)
        {
            var userList = await _userRepository.GetAll();
            var userData = userList.FirstOrDefault(x => x.UserName == userVm.UserName);
            if (userData is not null)
            {
                var randomPassword = PasswordGenerator.RandomPassword();
                var newPassword = PasswordEncrytion.Encriptation(randomPassword);
                User user = _mapper.Map<User>(userData);
                user.Password = newPassword;
                await _userRepository.Update(user, user.Id);
                await _emailService.SendEmail(new EmailRequest
                {
                    To = user.Mail,
                    Subject = "Cambia de contraseña",
                    Body = $"Su nueva contraseña es: {randomPassword}"
                });
            }
        }
        public async Task<List<UserViewModel>> SearchFriend(SearchFriendViewModel searchViewModel)
        {
            var list = await _userRepository.GetAll();
            return list.Where(user => user.UserName.ToLower() == searchViewModel.userName.ToLower()).Select(user => new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                LastName =user.LastName,
                Phone = user.Phone,
                ProfilePicture = user.ProfilePicture,
                Mail = user.Mail,
                UserName = user.UserName,
                Password = user.Password

            }).ToList();
        }
    }
}
