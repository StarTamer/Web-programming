using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using httpserver.attributes;

namespace httpserver.controllers
{
    [HttpController("users")]
    public class Users
    {
        [HttpGET("list")]
        public List<User> getUsers()
        {
            List<User> users = new List<User>();
            users.Add(new User{ ID = 1, Name = "Igor"});
            return users;
        }

    }

    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}
