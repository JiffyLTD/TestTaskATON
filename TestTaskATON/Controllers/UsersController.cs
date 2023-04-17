using Microsoft.AspNetCore.Mvc;
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
        public async Task<IResult> AddNewUser([FromQuery] UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adminResult = await _users.IsAdmin(model.User.Login, model.User.Password);

                if (adminResult.Status && adminResult.User != null)
                {
                    var creationResult = await _users.CreateUser(
                    model.Login, model.Password,
                    model.Name, model.Gender,
                    model.Birthday, model.Admin, model.User.Login);

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
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        [HttpGet]
        [Route("/users/getAllActive")]
        public async Task<IResult> GetActiveUsers([FromQuery] SignInViewModel model)
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
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя которого ищут</param>
        [HttpGet]
        [Route("/users/getByLogin")]
        public async Task<IResult> GetUserByLogin([FromQuery] SignInViewModel model, string userLogin)
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
                        isActive = false; // если RevokedOn и RevokedBy null, то пользователь активный
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
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        [HttpGet]
        [Route("/users/getByLoginAndPass")]
        public async Task<IResult> GetUserByLoginAndPassword([FromQuery] SignInViewModel model)
        {
            var userResult = await _users.GetUserByLoginAndPassword(model.Login, model.Password);

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
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="age">Возраст</param>
        [HttpGet]
        [Route("/users/elderThan")]
        public async Task<IResult> GetUsersOlderThan([FromQuery] SignInViewModel model, int age)
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
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя которого удаляют</param>
        [HttpDelete]
        [Route("/users/fullDelete")]
        public async Task<IResult> Delete([FromQuery] SignInViewModel model, string userLogin)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.DeleteUserByLogin(userLogin);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Удаление пользователя по логину мягкое
        /// </summary>
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя которого удаляют</param>
        [HttpDelete]
        [Route("/users/softDelete")]
        public async Task<IResult> SoftDelete([FromQuery] SignInViewModel model, string userLogin)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.StopUserActivity(userLogin, model.Login);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Восстановление пользователя
        /// </summary>
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя которого восстанавливают</param>
        [HttpPut]
        [Route("/users/restoreUser")]
        public async Task<IResult> RestoreUser([FromQuery] SignInViewModel model, string userLogin)
        {
            var adminResult = await _users.IsAdmin(model.Login, model.Password);

            if (adminResult.Status && adminResult.User != null)
            {
                var usersResult = await _users.RestoreUser(userLogin, model.Login);

                return Results.Ok(usersResult);
            }

            return Results.BadRequest(adminResult.Message);
        }

        /// <summary>
        /// Изменение имени, пола или даты рождения пользователя
        /// </summary>
        /// <param name="model">ViewModel для обновления данных пользователя</param>
        [HttpPut]
        [Route("/users/updateUser")]
        public async Task<IResult> UpdateUser([FromQuery] UpdateUserViewModel model)
        {
            var userResult = await _users.GetUserByLoginAndPassword(model.User.Login, model.User.Password);

            if (userResult.Status && userResult.User != null)
            {
                if (userResult.User.Admin)
                {
                    var usersResult = await _users.UpdateUser(model.Login, model.Name, model.Gender, model.Birthday, model.User.Login);

                    return Results.Ok(usersResult);
                }
                else
                {
                    if (model.Login == model.User.Login)
                    {
                        if (userResult.User.RevokedBy == null && userResult.User.RevokedOn == null)
                        {
                            var usersResult = await _users.UpdateUser(model.Login, model.Name, model.Gender, model.Birthday, model.User.Login);

                            return Results.Ok(usersResult);
                        }
                        else
                        {
                            return Results.Problem("Пользователь заблокирован");
                        }
                    }

                    return Results.Problem("Пользователь имеет право менять только свои данные");
                }
            }

            return Results.BadRequest(userResult.Message);
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя у которого меняют пароль</param>
        /// <param name="newPassword">Новый пароль пользователя</param>
        [HttpPut]
        [Route("/users/updateUserPass")]
        public async Task<IResult> UpdateUserPass([FromQuery] SignInViewModel model, string userLogin, string newPassword)
        {
            var userResult = await _users.GetUserByLoginAndPassword(model.Login, model.Password);

            if (userResult.Status && userResult.User != null)
            {
                if (userResult.User.Admin)
                {
                    var usersResult = await _users.UpdatePassword(userLogin, newPassword, model.Login);

                    return Results.Ok(usersResult);
                }
                else
                {
                    if (model.Login == userLogin)
                    {
                        if (userResult.User.RevokedBy == null && userResult.User.RevokedOn == null)
                        {
                            var usersResult = await _users.UpdatePassword(userLogin, newPassword, model.Login);

                            return Results.Ok(usersResult);
                        }
                        else
                        {
                            return Results.Problem("Пользователь заблокирован");
                        }
                    }

                    return Results.Problem("Пользователь имеет право менять только свои данные");
                }
            }

            return Results.BadRequest(userResult.Message);
        }

        /// <summary>
        /// Изменение пароля
        /// </summary>
        /// <param name="model">Логин и пароль выполняющего запрос</param>
        /// <param name="userLogin">Логин пользователя у которого меняют логин</param>
        /// <param name="newLogin">Новый логин пользователя</param>
        [HttpPut]
        [Route("/users/updateUserLogin")]
        public async Task<IResult> UpdateUserLogin([FromQuery] SignInViewModel model, string userLogin, string newLogin)
        {
            var userResult = await _users.GetUserByLoginAndPassword(model.Login, model.Password);

            if (userResult.Status && userResult.User != null)
            {
                if (userResult.User.Admin)
                {
                    var usersResult = await _users.UpdateLogin(userLogin, newLogin, model.Login);

                    return Results.Ok(usersResult);
                }
                else
                {
                    if (model.Login == userLogin)
                    {
                        if (userResult.User.RevokedBy == null && userResult.User.RevokedOn == null)
                        {
                            var usersResult = await _users.UpdateLogin(userLogin, newLogin, model.Login);

                            return Results.Ok(usersResult);
                        }
                        else
                        {
                            return Results.Problem("Пользователь заблокирован");
                        }
                    }

                    return Results.Problem("Пользователь имеет право менять только свои данные");
                }
            }

            return Results.BadRequest(userResult.Message);
        }
    }
}
