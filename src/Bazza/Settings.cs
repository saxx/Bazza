using System;

namespace Bazza;

public static class Settings
{
    public static DateTime StartDate = new(2022, 09, 09);
    public static DateTime EndDate = new(2022, 09, 10);
    public static DateTime OnlineStartDate = new(2022, 08, 23);
    public static DateTime OnlineEndDate = new(2022, 09, 05);
    public static int MaxNumberOfArticles = 6000;
    public static DateTime BringArticlesDate = new DateTime(2022, 09, 08).Date;
}