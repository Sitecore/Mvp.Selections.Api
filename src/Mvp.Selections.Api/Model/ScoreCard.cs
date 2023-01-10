using System;

namespace Mvp.Selections.Api.Model
{
    public class ScoreCard
    {
        public Applicant Applicant { get; set; } = null!;

        public int Median { get; set; }

        public int Average { get; set; }

        public int Min { get; set; }

        public Guid MinReviewId { get; set; }

        public int Max { get; set; }

        public Guid MaxReviewId { get; set; }

        public int ReviewCount { get; set; }
    }
}
