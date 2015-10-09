using System;

namespace Q2.Oao.Library.Common
{
	/// <summary>
	/// Represents a sequential globally unique identifier (GUID).
	/// </summary>
	public static class SequentialGuid
	{
		private static readonly long _baseTicks = new DateTime(1900, 1, 1).Ticks;

		/// <summary>
		/// Generate a new <see cref="Guid"/> using the comb algorithm.
		/// </summary>
		public static Guid NewGuid()
		{
			DateTime now = DateTime.Now;

			// Get the days and milliseconds which will be used to build the byte string 
			int daysSince1900 = new TimeSpan(now.Ticks - _baseTicks).Days;
			long msecs = (long) (now.TimeOfDay.TotalMilliseconds/3.333333);

			// Convert to a byte array 
			// Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
			byte[] daysArray = BitConverter.GetBytes(daysSince1900);
			byte[] msecsArray = BitConverter.GetBytes(msecs);

			// Reverse the bytes to match SQL Servers ordering 
			Array.Reverse(daysArray);
			Array.Reverse(msecsArray);

			byte[] guidArray = Guid.NewGuid().ToByteArray();
			// Copy the bytes into the guid 
			Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
			Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

			return new Guid(guidArray);
		}
	}
}