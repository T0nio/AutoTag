using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using AutoTag;

namespace AutoTagTests
{
    [TestClass]
    public class FileExtensionsUtilsTest
    {
        [TestMethod]
        public void sutShouldReturnTrueWhenOneMusicFileExtensionIsGiven()
        {
            string extension = ".flac";

            Assert.IsTrue(FileExtensionsUtils.IsMusicFile(extension));
        }

        [TestMethod]
        public void sutShouldReturnFalseWhenOneNotMusicFileExtensionIsGiven()
        {
            string extension = ".bmp";

            Assert.IsFalse(FileExtensionsUtils.IsMusicFile(extension));
        }

        [TestMethod]
        public void sutShouldReturnTrueWhenAListOfMusicFileExtensionsIsGiven()
        {
            List<string> extensions = new List<string> { ".flac", ".mp3" };

            Assert.IsTrue(FileExtensionsUtils.IsMusicFile(extensions));
        }

        [TestMethod]
        public void sutShouldReturnTrueWhenAListWithOneNotMusicFileExtensionIsGiven()
        {
            List<string> extensions = new List<string> { ".flac", ".jpg" };

            Assert.IsFalse(FileExtensionsUtils.IsMusicFile(extensions));
        }
    }
}
