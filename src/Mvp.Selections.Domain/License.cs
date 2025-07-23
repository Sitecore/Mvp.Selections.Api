using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvp.Selections.Domain
{
    public class License(Guid id) : BaseEntity<Guid>(id)
    {
        public required string LicenseContent { get; set; }

        public DateTime ExpirationDate { get; set; }

        public Guid? AssignedUserId { get; set; }

        public required string FileName { get; set; }

        public User? AssignedUser { get; set; }
    }
}
