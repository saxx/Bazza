using System;

namespace Bazza.Models.Database
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}