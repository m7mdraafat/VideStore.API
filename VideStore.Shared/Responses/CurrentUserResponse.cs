using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideStore.Shared.Responses
{
    public record CurrentUserResponse(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber
    );
}
