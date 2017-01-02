using System;

namespace AvenueClothing.Project.UserFeedback.ViewModels
{
    public class ReviewFormRenderingViewModel
    {
        public string SubmitReviewUrl { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
    }
}