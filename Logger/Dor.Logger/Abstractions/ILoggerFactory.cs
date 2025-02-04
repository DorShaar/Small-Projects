namespace Logger;

public interface ILoggerFactory
{
	Logger<T> CreateLogger<T>();
}