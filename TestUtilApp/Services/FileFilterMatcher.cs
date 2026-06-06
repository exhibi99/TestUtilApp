using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestUtilApp.Models;

namespace TestUtilApp.Services
{
    public static class FileFilterMatcher
    {
        public static FileFilterProfile GetActiveProfile(FileFilterConfig fileFilter)
        {
            if (fileFilter?.ConditionProfiles == null || fileFilter.ConditionProfiles.Count == 0)
            {
                return null;
            }

            int profileIndex = fileFilter.ActiveProfileIndex;
            if (profileIndex < 0 || profileIndex >= fileFilter.ConditionProfiles.Count)
            {
                profileIndex = 0;
            }

            return fileFilter.ConditionProfiles[profileIndex];
        }

        public static List<string> GetMatchingFiles(
            string rootFolder,
            FileFilterConfig fileFilter,
            Func<string, bool> filePredicate)
        {
            if (string.IsNullOrWhiteSpace(rootFolder) || !Directory.Exists(rootFolder))
            {
                return new List<string>();
            }

            FileFilterProfile profile = GetActiveProfile(fileFilter);
            List<string> targetFolders = GetTargetFolders(rootFolder, profile);
            targetFolders = ApplyFileIncludeFolderFilter(targetFolders, profile);

            var matchingFiles = new List<string>();
            foreach (string folder in targetFolders)
            {
                foreach (string file in Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (filePredicate != null && !filePredicate(file))
                    {
                        continue;
                    }

                    if (!ShouldExcludeFile(file, profile))
                    {
                        matchingFiles.Add(file);
                    }
                }
            }

            return matchingFiles;
        }

        public static string GetProfileDisplayName(FileFilterProfile profile)
        {
            return string.IsNullOrWhiteSpace(profile?.Name) ? "File Filtering" : profile.Name;
        }

        private static List<string> GetTargetFolders(string rootFolder, FileFilterProfile profile)
        {
            var allFolders = new List<string> { rootFolder };
            allFolders.AddRange(Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories));

            List<string> includeKeywords = profile?.IncludeFolderKeywords ?? new List<string>();
            if (profile == null || !profile.UseFolderIncludeFilter || includeKeywords.Count == 0)
            {
                return allFolders;
            }

            return allFolders
                .Where(folder => includeKeywords.Any(keyword =>
                    Path.GetFileName(folder).IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                .ToList();
        }

        private static List<string> ApplyFileIncludeFolderFilter(List<string> targetFolders, FileFilterProfile profile)
        {
            List<string> includeFileKeywords = profile?.IncludeFileKeywords ?? new List<string>();
            if (profile == null || !profile.UseFileIncludeFilter || includeFileKeywords.Count == 0)
            {
                return targetFolders;
            }

            return targetFolders
                .Where(folder => Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly)
                    .Any(file => FileNameContainsKeyword(file, includeFileKeywords)))
                .ToList();
        }

        private static bool ShouldExcludeFile(string filePath, FileFilterProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            if (profile.UseExtensionExcludeFilter && profile.ExcludeExtensions != null && profile.ExcludeExtensions.Count > 0)
            {
                string fileExtension = Path.GetExtension(filePath).TrimStart('.');
                if (profile.ExcludeExtensions.Any(extension =>
                    string.Equals(extension.TrimStart('.'), fileExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }

            if (profile.UseFileExcludeFilter && profile.ExcludeFileKeywords != null && profile.ExcludeFileKeywords.Count > 0)
            {
                string fileName = Path.GetFileName(filePath);
                if (profile.ExcludeFileKeywords.Any(keyword =>
                    fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool FileNameContainsKeyword(string filePath, List<string> keywords)
        {
            string fileName = Path.GetFileName(filePath);
            return keywords.Any(keyword =>
                fileName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
