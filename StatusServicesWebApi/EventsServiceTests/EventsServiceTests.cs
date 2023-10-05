using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StatusServicesApi;

namespace EventsServiceTests
{

    [TestFixture]
    public class EventsServiceTests
    {
        [Test]
        public void AddEvent_WhenValidEventDataProvided_ShouldAddEventToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

            using (var dbContext = new EventsDbContext(options))
            {
                var service = new EventsService(dbContext);
                var newEvent = new Event
                {
                    Service = "Service1",
                    Status = "Failure",
                    Message = "Error occurred",
                    Timestamp = new DateTime(2022, 1, 1)
                };

                // Act
                service.AddEvent(newEvent);
            }

            // Assert
            using (var dbContext = new EventsDbContext(options))
            {
                var eventsCount = dbContext.Events.Count();
                var addedEvent = dbContext.Events.FirstOrDefault();

                Assert.AreEqual(1, eventsCount);
                Assert.NotNull(addedEvent);
                Assert.AreEqual("Service1", addedEvent.Service);
                Assert.AreEqual("Failure", addedEvent.Status);
                Assert.AreEqual("Error occurred", addedEvent.Message);
                Assert.AreEqual(new DateTime(2022, 1, 1), addedEvent.Timestamp);
                Assert.IsFalse(addedEvent.IsArchived);
            }
        }
    }
}