using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzekiDemo.DataObjects
{
    public class Settings
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CallerId { get; set; }
        public int ServiceDuration { get; set; }
        public bool? Restart { get; set; }
        public int? RegistrationStatus { get; set; }

        public Settings()
        {

        }

        public Settings(string domain, string username, string password, string callerid, int? registrationStatus)
        {
            this.Domain = domain;
            this.CallerId = callerid;
            this.Password = password;
            this.Username = username;
            this.RegistrationStatus = registrationStatus;
        }
    }
}
