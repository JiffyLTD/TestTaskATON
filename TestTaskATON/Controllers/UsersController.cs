using Microsoft.AspNetCore.Mvc;
using TestTaskATON.Models.ResponseModels;
using TestTaskATON.Repositories.Interfaces;
using TestTaskATON.ViewModels;

namespace TestTaskATON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsers _users;

        public UsersController(IUsers users)
        {
            _users = users;
        }

        /// <summary>
        /// Создание пользователя по логину, паролю, имени, полу и дате рождения + указание будет ли пользователь админом
        /// </summary>
        /// <param name="model">ViewModel для регистрации</param>
        [HttpPost]
        [Route("/users/addNewUser")]
        public async Task<IResult> AddNewUser(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adminResult = await _users.IsAdmin(model.AdminModel.Login, model.AdminModel.Password);

                if (adminResult.Status && adminResult.User != null)
                {
                    var creationResult = await _users.CreateUser(
                    model.Login, model.Password,
                    model.Name, model.Gender,
                    model.Birthday, model.Admin, model.AdminModel.Password);

                    if (creationResult.Status)
                    {
                        return Results.Ok(creationResult);
                    }

                    return Results.BadRequest(creationResult.Message);
                }

                return Results.BadRequest(adminResult.Message);
            }

            return Results.BadRequest();
        }

        /// <summary>
        /// Запрос списка всех активных (отсутствует RevokedOn) пользователей
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        [HttpGet]
        [Route("/users/getAllActive")]
        public async Task<IResult> GetActiveUsers([FromQuery] AdminViewModel model)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var result = await _users.GetActiveUsers();

                if (result.Status)
                {
                    return Results.Ok(result);
                }

                return Results.BadRequest(result.Message);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Запрос пользователя по логину
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="userLogin">Логин пользователя которого ищут</param>
        [HttpGet]
        [Route("/users/getByLogin/{userLogin}")]
        public async Task<IResult> GetUserByLogin([FromQuery] AdminViewModel model, string userLogin)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var userResult = await _users.GetUserByLogin(userLogin);

                if (userResult.Status && userResult.User != null)
                {
                    bool isActive = true;

                    if (userResult.User.RevokedOn != null && userResult.User.RevokedBy != null)
                    {
                        isActive = false;
                    }

                    var userData = new
                    {
                        name = userResult.User.Name,
                        gender = userResult.User.Gender,
                        birthday = userResult.User.Birthday,
                        isActive
                    };

                    return Results.Ok(userData);
                }

                return Results.BadRequest(userResult.Message);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Запрос пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        [HttpGet]
        [Route("/users/getByLoginAndPass/{login}/{password}")]
        public async Task<IResult> GetUserByLoginAndPassword(string login, string password)
        {
            var userResult = await _users.GetUserByLoginAndPassword(login, password);

            if (userResult.Status && userResult.User != null)
            {
                if (userResult.User.RevokedBy == null && userResult.User.RevokedOn == null)
                {
                    return Results.Ok(userResult);
                }

                return Results.Problem("Пользователь заблокирован");
            }

            return Results.BadRequest(userResult.Message);
        }

        /// <summary>
        /// Запрос всех пользователей старше определённого возраста
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="age">Возраст</param>
        [HttpGet]
        [Route("/users/elderThan/{age}")]
        public async Task<IResult> GetUsersOlderThan([FromQuery] AdminViewModel model, int age)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.GetUsersOlderThan(age);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Удаление пользователя по логину полное
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="login">Логин пользователя которого удаляют</param>
        [HttpDelete]
        [Route("/users/fullDelete/{login}")]
        public async Task<IResult> Delete([FromQuery] AdminViewModel model, string login)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.DeleteUserByLogin(login);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Удаление пользователя по логину мягкое
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="login">Логин пользователя которого удаляют</param>
        [HttpDelete]
        [Route("/users/softDelete/{login}")]
        public async Task<IResult> SoftDelete([FromQuery] AdminViewModel model, string login)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.StopUserActivity(login, model.Login);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Восстановление пользователя
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="login">Логин пользователя которого восстанавливают</param>
        [HttpPut]
        [Route("/users/restoreUser/{login}")]
        public async Task<IResult> RestoreUser(AdminViewModel model, string login)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.RestoreUser(login, model.Login);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Изменение имени, пола или даты рождения пользователя
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        [HttpPut]
        [Route("/users/updateUser")]
        public async Task<IResult> UpdateUser(UpdateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adminResult = await _users.IsAdmin(model.Admin.Login, model.Admin.Password);
                var userResult = new Response("", false);

                if (model.Password != null)
                {
                    userResult = await _users.GetUserByLoginAndPassword(model.Login, model.Password);
                }

                if (adminResult.Status || userResult.Status)
                {
                    string whoUpdateUserLogin;

                    if (adminResult.Status)
                        whoUpdateUserLogin = model.Admin.Login;
                    else
                        whoUpdateUserLogin = model.Login;

                    var usersResult = await _users.UpdateUser(model.Login, model.Name, model.Gender, model.Birthday, whoUpdateUserLogin);

                    return Results.Ok(usersResult);
                }

                return Results.BadRequest(adminResult.Message);
            }

            return Results.BadRequest();
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="newPassword">Новый пароль пользователя</param>
        [HttpPut]
        [Route("/users/updateUserPass")]
        public async Task<IResult> UpdateUserPass(AdminViewModel model,string login, string? password, string newPassword)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);
            var userResult = new Response("", false);

            if (password != null)
            {
                userResult = await _users.GetUserByLoginAndPassword(login, password);
            }

            if (adminResult.Status || userResult.Status)
            {
                string whoUpdateUserLogin;

                if(adminResult.Status)
                    whoUpdateUserLogin = model.Login;
                else
                    whoUpdateUserLogin = login;

                var usersResult = await _users.UpdatePassword(login, newPassword, whoUpdateUserLogin);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model">ViewModel админа</param>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="newPassword">Новый пароль пользователя</param>
        [HttpPut]
        [Route("/users/updateUserLogin")]
        public async Task<IResult> UpdateUserLogin(AdminViewModel model, string login, string? password, string newLogin)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);
            var userResult = new Response("", false);

            if (password != null)
            {
                userResult = await _users.GetUserByLoginAndPassword(login, password);
            }

            if (adminResult.Status || userResult.Status)
            {
                string whoUpdateUserLogin;

                if (adminResult.Status)
                    whoUpdateUserLogin = model.Login;
                else
                    whoUpdateUserLogin = login;

                var usersResult = await _users.UpdateLogin(login, newLogin, whoUpdateUserLogin);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }
    }
}
