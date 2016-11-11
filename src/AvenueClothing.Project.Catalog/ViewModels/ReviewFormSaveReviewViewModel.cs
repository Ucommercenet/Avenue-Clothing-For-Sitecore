using System;

namespace AvenueClothing.Project.Catalog.ViewModels
{
    public class ReviewFormSaveReviewViewModel
    {
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
    }
}