namespace UsersApp;
using System.Collections.Generic;

public interface IUserRepository
{
    List<User> GetAllUsers();
    void InsertUser(User user);
}