namespace Dignus.Commands.Internals
{
    internal static class CommandPathResolver
    {
        public static string Resolve(string currentPath, string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return Normalize(currentPath);
            }

            string normalizedInputPath = inputPath.Replace('\\', '/');
            bool isAbsolutePath = normalizedInputPath.StartsWith('/');
            List<string> resolvedPathSegments;
            if (isAbsolutePath)
            {
                resolvedPathSegments = [];
            }
            else
            {
                resolvedPathSegments = SplitPath(Normalize(currentPath));
            }

            string[] inputPathSegments = normalizedInputPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (string inputPathSegment in inputPathSegments)
            {
                if (inputPathSegment == ".")
                {
                    continue;
                }

                if (inputPathSegment == "..")
                {
                    if (resolvedPathSegments.Count > 0)
                    {
                        resolvedPathSegments.RemoveAt(resolvedPathSegments.Count - 1);
                    }
                    continue;
                }

                resolvedPathSegments.Add(inputPathSegment);
            }

            if (resolvedPathSegments.Count == 0)
            {
                return "/";
            }

            return "/" + string.Join("/", resolvedPathSegments);
        }
        private static string Normalize(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "/";
            }

            string normalizedPath = path.Replace('\\', '/');

            if (normalizedPath.StartsWith('/') == false)
            {
                normalizedPath = "/" + normalizedPath;
            }

            return normalizedPath.TrimEnd('/');
        }
        private static List<string> SplitPath(string path)
        {
            return [.. path.Split('/', StringSplitOptions.RemoveEmptyEntries)];
        }
    }
}
