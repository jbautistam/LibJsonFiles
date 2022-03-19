using System;

namespace Bau.Libraries.LibJsonFiles.EventArguments
{
	/// <summary>
	///		Argumento del evento de lectura / escritura sobre un archivo Json
	/// </summary>
	public class ProgressEventArgs : EventArgs
	{
		public ProgressEventArgs(long records)
		{
			Records = records;
		}

		/// <summary>
		///		Número de registros
		/// </summary>
		public long Records { get; }
	}
}
