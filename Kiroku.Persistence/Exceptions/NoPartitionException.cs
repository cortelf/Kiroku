using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiroku.Persistence.Exceptions
{

	[Serializable]
	public class NoPartitionException : DatabaseException
	{
		public NoPartitionException() { }
		public NoPartitionException(string message) : base(message) { }
		public NoPartitionException(string message, Exception inner) : base(message, inner) { }
		protected NoPartitionException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
