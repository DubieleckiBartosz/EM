using System.Collections.Generic;

namespace EventManagement.Application.Wrappers
{
    public class ResponseList<T>
    {
        public int Count { get; private set; }
        public List<T> Data { get; set; }

        public ResponseList()
        {
        }

        private ResponseList(int count, List<T> data)
        {
            this.Count = count;
            this.Data = data;
        }

        public static ResponseList<T> Ok(int count, List<T> data)
        {
            return new ResponseList<T>(count, data);
        }
    }
}