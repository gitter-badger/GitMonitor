// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitoredPath.cs" company="FreeToDev">Mike Fourie</copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace GitMonitor.Models
{
    using System;
    using System.Collections.Generic;

    public class MonitoredPath
    {
        public MonitoredPath()
        {
            this.Commits = new List<GitCommit>();
            this.Repositories = new List<GitRepository>();
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public bool AllFolders { get; set; }

        public bool AllowFetch { get; set; }

        public bool IncludeMergeCommits { get; set; }

        public int Days { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public int CommitCount { get; set; }

        public List<GitRepository> Repositories { get; set; }

        public List<GitCommit> Commits { get; set; }
    }
}
