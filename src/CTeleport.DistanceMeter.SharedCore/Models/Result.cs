namespace CTeleport.DistanceMeter.SharedCore.Models
{
    public class Result<T>
    {
        public T Data { get; }
        public Error Error { get; }

        public Result(T data)
        {
            Data = data;
        }

        public Result(Error error)
        {
            Error = error;
        }

        public bool IsError => Error != null;
    }
}