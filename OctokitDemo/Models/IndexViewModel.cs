using System.Collections.Generic;
using Octokit;

namespace OctokitDemo.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<Repository> repositories)
        {
            Repositories = repositories;
        }

        public IEnumerable<Repository> Repositories { get; private set; }
    }
}
