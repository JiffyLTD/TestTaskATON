using TestTaskATON.Models.ResponseModels;

namespace TestTaskATON.Repositories.Interfaces
{
    public interface IUsers
    {
        /// <summary>
        /// Метод создания пользователя
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="name">Имя пользователя</param>
        /// <param name="gender">Пол пользователя</param>
        /// <param name="birthday">Дата рождения пользователя</param>
        /// <param name="admin">Администратор ли пользователь</param>
        /// <param name="adminLogin">Логин пользователя от имени которого этот пользователь создан</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> CreateUser(string login, string password, string name, int gender, DateTime? birthday, bool admin, string adminLogin);

        /// <summary>
        /// Метод изменения имени, даты рождения и пола пользователя
        /// </summary>
        /// <param name="login">Логин пользователя у которого будут менять данные</param>
        /// <param name="name">Новое имя пользователя</param>
        /// <param name="birthday">Новая дата рождения пользователя</param>
        /// <param name="gender">Новый пол пользователя</param>
        /// <param name="adminLogin">Логин пользователя от имени которого этот пользователь создан</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> UpdateUser(string login, string name, int gender, DateTime? birthday, string adminLogin);

        /// <summary>
        /// Метод изменение логина пользователя
        /// </summary>
        /// <param name="oldLogin">Логин пользователя у которого будут менять данные</param>
        /// <param name="newLogin">Новый логин пользователя</param>
        /// <param name="adminLogin">Логин пользователя от имени которого этот пользователь создан</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> UpdateLogin(string oldLogin, string newLogin, string adminLogin);

        /// <summary>
        /// Метод изменения пароля пользователя
        /// </summary>
        /// <param name="login">Логин пользователя у которого будут менять данные</param>
        /// <param name="password">Новый пароль пользователя</param>
        /// <param name="adminLogin">Логин пользователя от имени которого этот пользователь создан</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> UpdatePassword(string login, string password, string adminLogin);

        /// <summary>
        /// Метод восстановления пользователя(Очистка полей RevokedOn, RevokedBy)
        /// </summary>
        /// <param name="login">Логин пользователя у которого будут менять данные</param>
        /// <param name="adminLogin">Логин пользователя который изменяет данные</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> RestoreUser(string login, string adminLogin);

        /// <summary>
        /// Метод запроса списка всех активных пользователей
        /// </summary>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> GetActiveUsers();

        /// <summary>
        /// Метод запроса пользователя по его логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> GetUserByLogin(string login);

        /// <summary>
        /// Метод запроса пользователя по логину и паролю
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> GetUserByLoginAndPassword(string login, string password);

        /// <summary>
        /// Метод запроса всех пользователей старше определённого возраста
        /// </summary>
        /// <param name="age">Возраст, старше которого хотелось бы найти всех пользователей</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> GetUsersOlderThan(int age);

        /// <summary>
        /// Метод полного удаления пользователя по его логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции</returns>
        public Task<Response> DeleteUserByLogin(string login);

        /// <summary>
        /// Метод приостановки активности пользователя(мягкое удаление)
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="adminLogin">Логин пользователя который изменяет данные</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> StopUserActivity(string login, string adminLogin);

        /// <summary>
        /// Метод который проверяет администратор ли пользователь
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Возвращает ответ об операции в виде сообщения об успешности операции и/или самого объекта операции</returns>
        public Task<Response> IsAdmin(string login, string password);
    }
}
