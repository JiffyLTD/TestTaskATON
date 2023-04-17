using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TestTaskATON.Data;
using TestTaskATON.Models;
using TestTaskATON.Models.ResponseModels;
using TestTaskATON.Repositories.Interfaces;

namespace TestTaskATON.Repositories
{
    public class UserRepository : IUsers
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Response> CreateUser(string login, string password, string name, int gender, DateTime? birthday, bool admin, string adminLogin)
        {
            try
            {
                var checkUser = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if (checkUser != null) { return new Response("Пользователь с данным логином уже зарегестрирован", false); }

                User user = new(login, password, name, gender, birthday, admin, adminLogin);

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                return new Response("Пользователь успешно добавлен", true, user);
            }
            catch (Exception ex) 
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> DeleteUserByLogin(string login)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if(user != null) 
                {
                    try
                    {
                        _db.Users.Remove(user);
                        await _db.SaveChangesAsync();

                        return new Response("Пользователь успешно удален", true);
                    }
                    catch (Exception ex) 
                    {
                        return new Response(ex.Message, false);
                    }
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex) 
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> GetActiveUsers()
        {
            try
            {
                var users = await _db.Users.Where(x => x.RevokedOn == null).ToListAsync();

                return new Response("Список активных пользователей", true, users);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> GetUserByLogin(string login)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);
                
                if(user != null)
                    return new Response("Пользователь успешно найден", true, user);
                else
                    return new Response($"Пользователь {login} не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> GetUserByLoginAndPassword(string login, string password)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

                if (user != null)
                    return new Response("Пользователь успешно найден", true, user);
                else
                    return new Response("Имя пользователя или пароль не верны", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> GetUsersOlderThan(int age)
        {
            try
            {
                var users = await _db.Users.Where(x => DateTime.Now.Year - x.Birthday.Value.Year > age).ToListAsync();

                if(users.Count > 0)
                    return new Response("Список пользователей старше " + age, true, users);
                else
                    return new Response("Не удалось найти пользователей старше " + age, false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> IsAdmin(string login, string password)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

                if (user != null)
                {
                    if (user.Admin)
                    {
                        return new Response("Пользователь администратор", true, user);
                    }

                    return new Response("Пользователь не администратор", false);
                }
                
                return new Response("Не удалось найти пользователя отправителя или неправильный логин/пароль", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> RestoreUser(string login, string adminLogin)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if (user != null)
                {
                    user.RevokedBy = null;
                    user.RevokedOn = null;
                    user.ModifiedBy = adminLogin;
                    user.ModifiedOn = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return new Response("Пользователь успешно восстановлен", true, user);
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> StopUserActivity(string login, string adminLogin)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if (user != null)
                {
                    user.RevokedBy = adminLogin;
                    user.RevokedOn = DateTime.Now;
                    user.ModifiedBy = adminLogin;
                    user.ModifiedOn = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return new Response("Пользователь успешно заблокирован", true, user);
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> UpdateLogin(string oldLogin, string newLogin, string adminLogin)
        {
            try
            {
                var checkUser = await _db.Users.FirstOrDefaultAsync(x => x.Login == newLogin);

                if (checkUser != null) { return new Response("Пользователь с данным логином уже зарегестрирован", false); }


                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == oldLogin);

                if (user != null)
                {
                    user.Login = newLogin;
                    user.ModifiedBy = adminLogin;
                    user.ModifiedOn = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return new Response("Логин пользователя успешно изменен", true, user);
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> UpdatePassword(string login, string password, string adminLogin)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if (user != null)
                {
                    user.Password = password;
                    user.ModifiedBy = adminLogin;
                    user.ModifiedOn = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return new Response("Пароль пользователя успешно изменен", true, user);
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }

        public async Task<Response> UpdateUser(string login, string name, int gender, DateTime? birthday, string adminLogin)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Login == login);

                if (user != null)
                {
                    user.Name = name;
                    user.Birthday = birthday;
                    user.Gender = gender;
                    user.ModifiedBy = adminLogin;
                    user.ModifiedOn = DateTime.Now;
                    await _db.SaveChangesAsync();

                    return new Response("Данные пользователя успешно изменены", true, user);
                }

                return new Response("Пользователь не найден", false);
            }
            catch (Exception ex)
            {
                return new Response(ex.Message, false);
            }
        }
    }
}
