using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OzekiDemo.DataObjects
{
    public class Contact
    {

        public string FullName { get; set; }
        public string Address { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Company { get; set; }
        public string ContactGuid { get; set; }


        //public static List<Contact> ContactList()
        //{
        //    List<Contact> contactList = new List<Contact>(5);
        //    contactList.Add(new Contact() { FullName = "Bantale Jide", Company = "Soft Sparks", BusinessPhone = "814", MobilePhone = "814", Address = "Lagos" });
        //    contactList.Add(new Contact() { FullName = "Abisoye Solarin", Company = "Abisoye LTD", BusinessPhone = "809", MobilePhone = "809", Address = "Lagos" });
        //    contactList.Add(new Contact() { FullName = "Danjuma Audu", Company = "Danjay Industries", BusinessPhone = "822", MobilePhone = "822", Address = "Lagos" });
        //    contactList.Add(new Contact() { FullName = "Koya Temitope", Company = "Koya Enterprises", BusinessPhone = "827", MobilePhone = "827", Address = "Lagos" });
        //    contactList.Add(new Contact() { FullName = "Olamide Azeez", Company = "SoftTech", BusinessPhone = "819", MobilePhone = "819", Address = "Lagos" });
        //    return contactList;
        //}
    }

    public class DbContact : Contact
    {
        public string Url { get; set; }
        public string Phone { get; set; }
        public DbContact()
        {

        }

        public DbContact(string fullname, string phone, string company, string url) : base()
        {
            this.FullName = fullname;
            this.Phone = phone;
            this.Company = company;
            this.Url = url;
        }

    }
}
