using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verbum_service_domain.Models;

namespace verbum_service_domain.DTO.Response
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string RoleCode { get; set; }
        public ICollection<Revelancy> Revelancies { get; set; }
    }
}
