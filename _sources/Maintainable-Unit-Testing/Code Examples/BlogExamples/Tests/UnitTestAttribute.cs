using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit2;

namespace BlogExamples.Tests
{
    /// <summary>
    /// This is the base test attribute, used in almost all the in-memory unit tests.
    /// Whenever a special configuration is required (example: for implementation tests)
    /// you can use the static CreateBasicFixture as a base.
    /// </summary>
    public class UnitTestAttribute : AutoDataAttribute
    {
        public UnitTestAttribute() : base(CreateBasicFixture)
        { /* Leave empty */}

        /// <summary>
        /// Creates the fixture for a basic in-memory unit test, to be re-used by other TestAttributes
        /// </summary>
        /// <returns>Configured fixture</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static IFixture CreateBasicFixture()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            return fixture;
        }
    }
}