using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BT.Auctions.API.Models.Pagination
{
    public class PagingHeader
    {
        public PagingHeader(
            int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
        }

        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }

        public string ToJson() => JsonConvert.SerializeObject(this,
            new JsonSerializerSettings
            {
                ContractResolver = new
                    CamelCasePropertyNamesContractResolver()
            });
    }
}
