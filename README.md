# TestTaskATON

Подключена БД MS SQL

Вся логика работы с бд была вынесена в репозиторий UserRepository

Валидация (отсутствует RevokedOn, Доступно Админам и тп.) была выполнена в контроллере UsersController

// моменты которые могут быть непонятны
User.Login и User.Password - означают данные отправляющего запрос (методы добавления пользователя и обновления данных пользователя)
в остальных случаях данные отправляющего запрос обозначаются как Login и Password

userLogin - во всех случаях обозначается как логин пользователя над которым производится операция

