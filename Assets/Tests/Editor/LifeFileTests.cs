using NUnit.Framework;

namespace Tests.Editor
{
    public sealed class LifeFileTests
    {
        [Test]
        public void PatternsEvolve()
            => Assert.DoesNotThrow(new LifeRegressionChecks().PatternsEvolve);
        
        [Test]
        public void LongLimitBlocksSurvive()
            => Assert.DoesNotThrow(new LifeRegressionChecks().LongLimitBlocksSurvive);
        
        [Test]
        public void EdgesDoNotWrap()
            => Assert.DoesNotThrow(new LifeRegressionChecks().EdgesDoNotWrap);
        
        [Test]
        public void LifeFilesParseAndSortCoordinates()
            => Assert.DoesNotThrow(new LifeRegressionChecks().FileParsingAndSorting);

        [Test]
        public void SampleFileRunsExactlyTenGenerations()
            => Assert.DoesNotThrow(new LifeRegressionChecks().SampleRunsTenGenerations);
    }
}
