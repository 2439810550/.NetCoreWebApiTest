namespace day1.Interfaces
{
    public interface IDateTimeProvider
    {
        /// <summary>
        /// 获取当前的UTC时间。这个属性返回一个DateTime对象，表示当前的UTC时间。它通常用于需要获取当前时间的场景，例如记录日志、计算时间差等。
        /// </summary>
        DateTime UtcNow { get; }
    }
}
