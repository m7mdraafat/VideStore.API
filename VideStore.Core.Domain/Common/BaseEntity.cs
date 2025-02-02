

namespace VideStore.Domain.Common
{
    public class BaseEntity
    {
        public string Id { get; set;  } = Guid.NewGuid().ToString();
    }
}
