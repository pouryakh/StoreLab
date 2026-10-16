using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StoreDto
{
    public class UpdateStoreRequest
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
    }
}
