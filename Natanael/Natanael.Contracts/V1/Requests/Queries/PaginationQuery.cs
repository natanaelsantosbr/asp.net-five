using System;
using System.Collections.Generic;
using System.Text;

namespace Natanael.Contracts.V1.Requests.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            this.PageNumber = 1;
            this.PageSize = 100;
        }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
