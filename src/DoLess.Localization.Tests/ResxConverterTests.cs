using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoLess.Localization;
using NUnit.Framework;
using FluentAssertions;
using System.IO;
using System.Reflection;
using NSubstitute;

namespace DoLess.Localization.Tests
{
    [TestFixture]
    public class ResxConverterTests
    {
        [TestCase("resources.resx", null)]
        [TestCase("resources.fr.resx", "fr")]
        [TestCase("resources.fr-Fr.resx", "fr-Fr")]
        [TestCase("resources.tests.fr.resx", "fr")]
        public void GetLanguageOfResxTests(string resxFilePath, string expected)
        {
            var actual = ResxConverter.GetLanguageOfResx(resxFilePath);
            actual.Should().Be(expected);
        }

        [Test]
        public void GetResxFilesFromSample()
        {
            var relativeSampleProjectPath = "../../../DoLess.Localization.Sample.Strings";
            var fullSampleProjectPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relativeSampleProjectPath));

            var resxConverter = new ResxConverter(fullSampleProjectPath, GetLogger());
            var expected = new List<string>
            {
                "AppMessages.resx",
                "AppMessages.fr.resx",
                "AppResources.resx",
                "AppResources.en.resx",
                "AppResources.fr.resx"
            };
            var files = resxConverter.FindResxFiles();
            files.Select(x => Path.GetFileName(x)).Should().BeEquivalentTo(expected);
        }

        
        public void Test()
        {
            var relativeSampleProjectPath = "../../../DoLess.Localization.Sample.Strings";
            var fullSampleProjectPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), relativeSampleProjectPath));

            var resxConverter = new ResxConverter(fullSampleProjectPath, GetLogger());
            resxConverter.Execute();
        }

        private ILogger GetLogger()
        {
            return Substitute.For<ILogger>();
        }
    }
}
