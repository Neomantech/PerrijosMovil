using System;
namespace PerrijosGatijos.Models
{

	/// <summary>
	/// Hold an api operation result
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PerrijosGatijosOperation<T>
	{
        /// <summary>
        /// Gets or sets a value indicating whether this instance is sucessful.
        /// </summary>
        public bool IsSucessful { get; set; }

        /// <summary>
        /// Gets or sets the operation result message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the operation result.
        /// </summary>
        public T Result { get; set; }
    }
}

