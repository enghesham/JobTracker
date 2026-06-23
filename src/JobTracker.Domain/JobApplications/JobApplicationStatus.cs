namespace JobTracker.Domain.JobApplications;

public enum JobApplicationStatus
{
    Draft = 0,
    Applied = 1,
    Screening = 2,
    Interview = 3,
    Offer = 4,
    Rejected = 5,
    Withdrawn = 6
}
