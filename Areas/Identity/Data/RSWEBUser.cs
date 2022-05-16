using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RSWEB.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the RSWEBUser class
    public class RSWEBUser : IdentityUser
    {
        public System.Int64 LinkId { get; set; }
    }
}
