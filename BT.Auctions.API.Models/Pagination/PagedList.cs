using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BT.Auctions.API.Models.Pagination
{
    /// <summary>
    /// A generic class that 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        public PagedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="totalItems">The total items.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        public PagedList(IEnumerable<T> source, int totalItems, int pageNumber, int pageSize)
        {
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
            List = source;
        }

        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public IEnumerable<T> List { get; }
        public int TotalPages => TotalItems != 0 && PageSize != 0 ? (int)Math.Ceiling(TotalItems / (double)PageSize) : 1;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;
        public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;

        [NotMapped]
        [DefaultValue(false)]
        public bool IsCancelled { get; set; }
        [NotMapped]
        public string CancellationReason { get; set; }

        public PagingHeader GetHeader()
        {
            return new PagingHeader(
                TotalItems, PageNumber,
                PageSize, TotalPages);
        }
    }
}
