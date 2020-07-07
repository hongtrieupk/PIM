namespace PIMWebMVC.Models.Common
{
    public class PaginationBarModel
    {
        public PaginationBarModel(int currentPage, int totalPage)
        {
            CurrentPage = currentPage;
            TotalPages = totalPage;
        }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}