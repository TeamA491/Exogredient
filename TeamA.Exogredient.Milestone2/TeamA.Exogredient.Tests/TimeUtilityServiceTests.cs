using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeamA.Exogredient.Services;

namespace TeamA.Exogredient.Tests
{
    [TestClass]
    public class TimeUtilityServiceTests
    {
        [TestMethod]
        public void TimeUtilityService_CurrentUnixTime_CurrentTimeMatchSuccess()
        {
            // Arrange:
            long expectedTime = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

            // Act: 
            long actualTime = TimeUtilityService.CurrentUnixTime();

            // Assert:
            Assert.AreEqual(expectedTime, actualTime);
        }



        [TestMethod]
        [DataRow(1, 3, 5)]
        public void TimeUtilityService_TimespanToSeconds_ConvertToSecondsSuccessfull(int hours, int minutes, int seconds)
        {
            // Arrange: Get the expected seconds for the timespan object
            long expectedSeconds = (long)(new TimeSpan(hours, minutes, seconds)).TotalSeconds;

            TimeSpan ts = new TimeSpan(hours, minutes, seconds);
            // Act: Get the seconds for timespan object using the function.
            long actualSeconds = TimeUtilityService.TimespanToSeconds(ts);

            // Assert:
            Assert.AreEqual(expectedSeconds, actualSeconds);
        }
    }
}
