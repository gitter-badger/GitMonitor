// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Options.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Export
{
    using CommandLine;

    public class Options
    {
        [Option('s', "service-endpoint", HelpText = "The service endpoint to execute rest calls against", Required = true)]
        public string ServiceEndPoint { get; set; }

        [Option('d', "days", HelpText = "The number of days to retrieve", Required = false, DefaultValue = 10)]
        public int Days { get; set; }

        [Option('r', "repositoryname", HelpText = "The name of the repository to retrieve", Required = false)]
        public string RepositoryName { get; set; }

        [Option('b', "branchname", HelpText = "The name of the branch to retrieve", Required = false, DefaultValue = "master")]
        public string BranchName { get; set; }

        [Option('x', "excel", HelpText = "Export to Excel format. Default is false and export is to csv", Required = false, DefaultValue = false)]
        public bool Excel { get; set; }
    }
}
