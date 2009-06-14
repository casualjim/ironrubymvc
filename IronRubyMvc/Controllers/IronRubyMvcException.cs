namespace System.Web.Mvc.IronRuby.Controllers
{
    public class IronRubyMvcException : Exception
    {
        private readonly string _stackTrace;

        public IronRubyMvcException(string message, string stackTrace) : base(message)
        {
            _stackTrace = stackTrace;
        }

        public IronRubyMvcException(string message, string stackTrace, Exception innerException) : base(message, innerException)
        {
            _stackTrace = stackTrace;
        }

        public override string StackTrace
        {
            get
            {
                return _stackTrace;
            }
        }
    }
}