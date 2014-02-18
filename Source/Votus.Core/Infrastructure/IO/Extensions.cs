using System.IO;
using System.Linq;

namespace Votus.Core.Infrastructure.IO
{
    public static class Extensions
    {
        public 
        static
        FileInfo
        FindFileInParentTree(
            this 
            DirectoryInfo   directory,
            string          fileName)
        {
            var currentPath = directory;

            while (currentPath != null)
            {
                // Only supporting one settings file right now...
                var settingsFile = currentPath
                    .EnumerateFiles(fileName)
                    .SingleOrDefault();

                // Return it if found...
                if (settingsFile != null)
                    return settingsFile;

                // Otherwise, go up a directory and look again ...
                currentPath = currentPath.Parent;
            }

            throw new FileNotFoundException(
                string.Format("Could not find any files matching '{0}' starting from '{1}'.", fileName, directory)
            );
        }
    }
}
