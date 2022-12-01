using System;
using System.Collections.Generic;

namespace PerrijosGatijos.Models
{
	/// <summary>
	/// Generic model to host a list in a Data property
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ListRequest<T>
	{
		public List<T> Data { get; set; }
	}
}

