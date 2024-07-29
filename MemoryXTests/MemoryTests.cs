
using Moq;
using Moq.Protected;
using System.Reflection;

namespace MemoryX.Tests
{
    [TestFixture]
    public class MemoryTests
    {
        private MemoryX memoryx;

        [SetUp]
        public void Setup()
        {
            memoryx = new MemoryX();
        }

        [Test]
        public void TestGetProcessHandleById_Mocked()
        {
            // Arrange
            var mockMemoryX = new Mock<MemoryX> { CallBase = true };
            var mockProcessHandle = new IntPtr(1234);
            var processId = 23456;

            // Use reflection to get the private OpenProcess method
            var openProcessMethod = typeof(MemoryX).GetMethod("OpenProcess", BindingFlags.NonPublic | BindingFlags.Static);

            // Set up the private method to return the mock handle
            mockMemoryX.Protected()
                       .Setup<IntPtr>("OpenProcess", (int)MemoryX.ProcessAccess.AllAccess, false, processId)
                       .Returns(mockProcessHandle);

            // Act
            var result = mockMemoryX.Object.GetProcessHandle(processId);

            // Assert
            Assert.That(result, Is.True);
            mockMemoryX.Protected().Verify("OpenProcess", Times.Once(), (int)MemoryX.ProcessAccess.AllAccess, false, processId);
        }
    }
}