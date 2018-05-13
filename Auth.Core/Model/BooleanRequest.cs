using System.Runtime.Serialization;

namespace Auth.Core.Model
{
	[DataContract]
	public class BooleanRequest
	{
		/// <summary>
		/// Значение
		/// </summary>
		[DataMember(IsRequired = true)]
		public bool Value;
	}
}
