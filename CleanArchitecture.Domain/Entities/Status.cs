using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Entities
{
    public class Status : BaseEntity
    {
        public StatusEnum StatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
