using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Business.Exceptions
{

	[Serializable]
	public class BadLogRecordDateTimeException : Exception
	{
		public BadLogRecordDateTimeException() { }
		public BadLogRecordDateTimeException(string message) : base(message) { }
		public BadLogRecordDateTimeException(string message, Exception inner) : base(message, inner) { }
		protected BadLogRecordDateTimeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
