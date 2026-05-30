namespace zuli_Business.Utils
{
    public static class DateTimeExtensions
    {
        public static int ToDayOfWeekMask(this DateTime date)
        {
            int dayOfWeekNum = date.DayOfWeek == DayOfWeek.Sunday ? 6 : (int)date.DayOfWeek - 1;
            return 1 << dayOfWeekNum;
        }
    }
}