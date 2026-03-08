namespace Dignus.Commands.Internals
{
    internal static class CommandPathResolver
    {
        public static string[] Resolve(IReadOnlyList<string> currentPathSegments, string inputPath)
        {
            if (string.IsNullOrWhiteSpace(inputPath))
            {
                return [.. currentPathSegments];
            }

            string normalizedPath = inputPath.Replace('\\', '/');
            bool isAbsolutePath = normalizedPath.StartsWith('/');
            List<string> resolvedPathSegments;
            if (isAbsolutePath)
            {
                resolvedPathSegments = [];
            }
            else
            {
                resolvedPathSegments = [.. currentPathSegments];
            }
            string[] inputPathSegments = normalizedPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

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
            return [.. resolvedPathSegments];
        }
    }
}
