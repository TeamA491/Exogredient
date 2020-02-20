using System;
using TeamA.Exogredient.AppConstants;

namespace TeamA.Exogredient.Services
{
    public static class TimeUtilityService
    {
        /// <summary>
        /// Returns the current time in Unix/Epoch.
        /// </summary>
        /// <returns>The unix time value (long)</returns>
        public static long CurrentUnixTime()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Gets the new UTC epoch time for which the token would expire.
        /// </summary>
        /// <param name="min">How many minutes from now, to get a UTC time.</param>
        /// <returns>Epoch time representing `x` minutes from now.</returns>
        public static long GetEpochFromNow(int min = Constants.TOKEN_EXPIRATION_MIN)
        {
            DateTime curTime = DateTime.UtcNow;
            return ((DateTimeOffset)curTime.AddMinutes(min)).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Turns a timspan object into a number of seconds.
        /// </summary>
        /// <param name="span">The time span (TimeSpan)</param>
        /// <returns>The number of seconds (long)</returns>
        public static long TimespanToSeconds(TimeSpan span)
        {
            long result = 0;

            // Get the hours, minutes, and seconds of the time span.
            int inputHours = span.Hours;
            int inputMinutes = span.Minutes;
            int inputSeconds = span.Seconds;

            // Sum all seconds.
            for (int i = 0; i < inputHours; i++)
            {
                result += Constants.SecondsInAnHour;
            }

            for (int i = 0; i < inputMinutes; i++)
            {
                result += Constants.SecondsInAMinute;
            }

            result += inputSeconds;

            return result;
        }


        /// <summary>
        /// Evaluates whether the current time is passed the time represented by the <paramref name="hour"/>,
        /// <paramref name="minute"/>, <paramref name="second"/>, <paramref name="month"/>, <paramref name="day"/>,
        /// and <paramref name="year"/> summed with the time span.
        /// </summary>
        /// <param name="span">The time span part of the equation (TimSpan)</param>
        /// <param name="hour">The hour part of the equation (int)</param>
        /// <param name="minute">The minute part of the equation (int)</param>
        /// <param name="second">The second part of the equation (int)</param>
        /// <param name="month">The month part of the equation (int)</param>
        /// <param name="day">The day part of the equation (int)</param>
        /// <param name="year">The year part of the equation (int)</param>
        /// <returns>bool whether or not the current time is pas the arguments summed together</returns>
        public static bool CurrentTimePastDatePlusTimespan(TimeSpan span, int hour, int minute,
                                                           int second, int month, int day, int year)
        {
            // The hours, minutes, and seconds contained within the time span.
            int inputHours = span.Hours;
            int inputMinutes = span.Minutes;
            int inputSeconds = span.Seconds;

            // Calculate the result (start at the input and add the time span)
            int resultHour = hour;
            int resultMinute = minute;
            int resultSecond = second;
            int resultMonth = month;
            int resultDay = day;
            int resultYear = year;


            // Sum the input hours to the result.
            for (int i = 0; i < inputHours; i++)
            {
                resultHour++;

                // If the hours went over the hours in a day, increment the day and reset the hour value.
                if (resultHour >= Constants.HoursInADay)
                {
                    resultHour = Constants.HourStartValue;

                    resultDay++;
                }

                // If the day went over the days in the current month, increment the month and reset the day value.
                if (resultDay > Constants.MonthDays[resultMonth])
                {
                    // If the month is february, the year is divisible by the leap year occurrence (4), and the day is the leap day value (29),
                    // the month may not be incremented (i.e it is a leap year and the day is valid for february).
                    if (resultMonth == Constants.FebruaryMonthValue &&
                        resultYear % Constants.LeapYearOccurrenceYears == 0 &&
                        resultDay == Constants.LeapDayValue)
                    {
                        // If the month is divisible by the unoccurence amount of years (100) and not the reoccurence amount of years (400),
                        // it is not a leap year so increment the month.
                        if (resultYear % Constants.LeapYearUnoccurenceYears == 0 &&
                            resultYear % Constants.LeapYearReoccurenceYears != 0)
                        {
                            resultDay = Constants.DayStartValue;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = Constants.DayStartValue;

                        resultMonth++;
                    }
                }

                // If the month went over the amount of months in a year, increment the year and reset the month value.
                if (resultMonth > Constants.MonthsInAYear)
                {
                    resultMonth = Constants.MonthStartValue;

                    resultYear++;
                }
            }

            // Sum the input minutes to the result.
            for (int i = 0; i < inputMinutes; i++)
            {
                resultMinute++;

                // If the minute went over the minutes in an hour, increment the hour and reset the minute value.
                if (resultMinute >= Constants.MinutesInAnHour)
                {
                    resultMinute = Constants.MinuteStartValue;

                    resultHour++;
                }

                // If the hours went over the hours in a day, increment the day and reset the hour value.
                if (resultHour >= Constants.HoursInADay)
                {
                    resultHour = Constants.HourStartValue;

                    resultDay++;
                }

                // If the day went over the days in the current month, increment the month and reset the day value.
                if (resultDay > Constants.MonthDays[resultMonth])
                {
                    // If the month is february, the year is divisible by the leap year occurrence (4), and the day is the leap day value (29),
                    // the month may not be incremented (i.e it is a leap year and the day is valid for february).
                    if (resultMonth == Constants.FebruaryMonthValue &&
                        resultYear % Constants.LeapYearOccurrenceYears == 0 &&
                        resultDay == Constants.LeapDayValue)
                    {
                        // If the month is divisible by the unoccurence amount of years (100) and not the reoccurence amount of years (400),
                        // it is not a leap year so increment the month.
                        if (resultYear % Constants.LeapYearUnoccurenceYears == 0 &&
                            resultYear % Constants.LeapYearReoccurenceYears != 0)
                        {
                            resultDay = Constants.DayStartValue;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = Constants.DayStartValue;

                        resultMonth++;
                    }
                }

                // If the month went over the amount of months in a year, increment the year and reset the month value.
                if (resultMonth > Constants.MonthsInAYear)
                {
                    resultMonth = Constants.MonthStartValue;

                    resultYear++;
                }
            }

            // Sum the input seconds to the result.
            for (int i = 0; i < inputSeconds; i++)
            {
                resultSecond++;

                // If the second went over the seconds in a minute, increment the minute and reset the second value.
                if (resultSecond >= Constants.SecondsInAMinute)
                {
                    resultSecond = Constants.SecondsStartValue;

                    resultMinute++;
                }

                // If the minute went over the minutes in an hour, increment the hour and reset the minute value.
                if (resultMinute >= Constants.MinutesInAnHour)
                {
                    resultMinute = Constants.MinuteStartValue;

                    resultHour++;
                }

                // If the hours went over the hours in a day, increment the day and reset the hour value.
                if (resultHour >= Constants.HoursInADay)
                {
                    resultHour = Constants.HourStartValue;

                    resultDay++;
                }

                // If the day went over the days in the current month, increment the month and reset the day value.
                if (resultDay > Constants.MonthDays[resultMonth])
                {
                    // If the month is february, the year is divisible by the leap year occurrence (4), and the day is the leap day value (29),
                    // the month may not be incremented (i.e it is a leap year and the day is valid for february).
                    if (resultMonth == Constants.FebruaryMonthValue &&
                        resultYear % Constants.LeapYearOccurrenceYears == 0 &&
                        resultDay == Constants.LeapDayValue)
                    {
                        // If the month is divisible by the unoccurence amount of years (100) and not the reoccurence amount of years (400),
                        // it is not a leap year so increment the month.
                        if (resultYear % Constants.LeapYearUnoccurenceYears == 0 &&
                            resultYear % Constants.LeapYearReoccurenceYears != 0)
                        {
                            resultDay = Constants.DayStartValue;

                            resultMonth++;
                        }
                        else
                        {
                            // Leap year.
                        }
                    }
                    else
                    {
                        resultDay = Constants.DayStartValue;

                        resultMonth++;
                    }
                }

                // If the month went over the amount of months in a year, increment the year and reset the month value.
                if (resultMonth > Constants.MonthsInAYear)
                {
                    resultMonth = Constants.MonthStartValue;

                    resultYear++;
                }
            }

            // Compare the result datetime to the current datetime
            DateTime resultTime = new DateTime(resultYear, resultMonth, resultDay, resultHour, resultMinute, resultSecond, DateTimeKind.Utc);

            int compareResult = DateTime.Compare(resultTime, DateTime.UtcNow);

            if (compareResult < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
