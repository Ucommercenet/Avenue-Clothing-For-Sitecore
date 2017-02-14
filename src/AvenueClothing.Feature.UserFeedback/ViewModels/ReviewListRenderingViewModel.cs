using System;
using System.Collections.Generic;

namespace AvenueClothing.Feature.UserFeedback.ViewModels
{
    public class ReviewListRenderingViewModel
    {
        public List<Review> Reviews { get; set; }

        public ReviewListRenderingViewModel()
        {
            Reviews = new List<Review>();
        }

        public class Review
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public string Comments { get; set; }
            public int? Rating { get; set; }
            public DateTime CreatedOn { get; set; }
            public Guid ProductGuid { get; set; }
            public Guid CategoryGuid { get; set; }
        }
    }
}