using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key]
        // (Group)Name will act as a primary key. [Key] will index it and easier to find in EF.
        public string Name { get; set; }

        // The reason for initializing a new List here, when we want a new Group, we automatically want a new List to add a Connection
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}