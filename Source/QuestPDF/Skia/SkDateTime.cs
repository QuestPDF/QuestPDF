namespace NativeSkia;

internal struct SkDateTime
{
    public short TimeZoneMinutes;
    public ushort Year;
    public byte Month;
    public byte DayOfWeek;
    public byte Day;
    public byte Hour;
    public byte Minute;
    public byte Second;
        
    public SkDateTime(System.DateTimeOffset dateTime)
    {
        TimeZoneMinutes = (short)(dateTime.Offset.TotalMinutes);
        Year = (ushort)dateTime.Year;
        Month = (byte)dateTime.Month;
        DayOfWeek = (byte)dateTime.DayOfWeek;
        Day = (byte)dateTime.Day;
        Hour = (byte)dateTime.Hour;
        Minute = (byte)dateTime.Minute;
        Second = (byte)dateTime.Second;
    }
}