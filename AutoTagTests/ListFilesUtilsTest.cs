using Microsoft.VisualStudio.TestTools.UnitTesting;
using AutoTag;
using System.Collections.Generic;
using System.Linq;

namespace AutoTagTests
{
    [TestClass]
    public class ListFilesUtilsTest
    {
        private string oldCurrentDirectory;

        [TestInitialize]
        public void InitializeTest()
        {
            oldCurrentDirectory = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory(@"../..");
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            System.IO.Directory.SetCurrentDirectory(oldCurrentDirectory);
        }

        [TestMethod]
        public void sutShouldReturnEmptyListWhenFolderInputIsEmpty()
        {
            string inputFolder = "rootTestFolder/emptyTestFolder";

            List<string> expectedOutput = new List<string>(); 

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnEmptyListWhenFolderInputIsNotEmptyAndEmptyExtensionListIsGiven()
        {
            string inputFolder = "rootTestFolder";
            List<string> inputExtensions = new List<string>();

            List<string> expectedOutput = new List<string>();

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder, inputExtensions)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnAllMusicFilesWhenNoExtensionIsGiven()
        {
            string inputFolder = "rootTestFolder/notEmptyTestFolder";

            List<string> expectedOutput = new List<string> {
                "rootTestFolder/notEmptyTestFolder/01 Music.flac",
                "rootTestFolder/notEmptyTestFolder/01 Music.mp3",
            };

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnOnlyMp3FilesWhenOnlyMp3ExtensionIsGiven()
        {
            string inputFolder = "rootTestFolder/notEmptyTestFolder";
            string inputExtension = ".mp3";

            List<string> expectedOutput = new List<string> {
                "rootTestFolder/notEmptyTestFolder/01 Music.mp3"
            };

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder, inputExtension)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnMusicFilesOfGivenExtensionsWhenExtensionsListIsGiven()
        {
            string inputFolder = "rootTestFolder";
            List<string> inputExtensions = new List<string> { ".mp3", ".flac" };

            List<string> expectedOutput = new List<string> {
                "rootTestFolder/notEmptyTestFolder/01 Music.flac",
                "rootTestFolder/notEmptyTestFolder/01 Music.mp3",
            };

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder, inputExtensions)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnAllRecurivesFilesFromGivenFolder()
        {
            string inputFolder = "rootTestFolder";

            List<string> expectedOutput = new List<string>
            {
                "rootTestFolder/01 Music.wav",
                "rootTestFolder/notEmptyTestFolder/01 Music.flac",
                "rootTestFolder/notEmptyTestFolder/01 Music.mp3",
                "rootTestFolder/notEmptyTestFolder/cover.jpg",
            };

            Assert.IsTrue(
                ListFilesUtils.ListFilesRecursivelyFromFolder(inputFolder)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldReturnAllRecursiveMusicFilesWhenGivenFolderContainsOtherFolders()
        {
            string inputFolder = "rootTestFolder";

            List<string> expectedOutput = new List<string> {
                "rootTestFolder/01 Music.wav",
                "rootTestFolder/notEmptyTestFolder/01 Music.flac",
                "rootTestFolder/notEmptyTestFolder/01 Music.mp3",
            };

            Assert.IsTrue(
                ListFilesUtils.ListMusicFilesFromFolder(inputFolder)
                              .SequenceEqual(expectedOutput)
            );
        }

        [TestMethod]
        public void sutShouldThrowExceptionWhenNotMusicFileExtensionIsGiven()
        {
            string inputFolder = "rootTestFolder";
            List<string> inputExtensions = new List<string> { ".mp3", ".jpg" };

            Assert.ThrowsException<NotMusicFileExtensionException>(
                () => ListFilesUtils.ListMusicFilesFromFolder(inputFolder, inputExtensions)
            );
        }
    }
}
