﻿using System.Collections.Generic;
using System.Net.Http.Headers;

namespace PIMWebMVC.Models.Projects
{
    public class ProjectsPaginationResult
    {
        public ProjectsPaginationResult()
        {
            Projects = new List<ProjectForViewTableModel>();
        }
        public IList<ProjectForViewTableModel> Projects { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}