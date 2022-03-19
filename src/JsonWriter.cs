using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJsonFiles
{
	/// <summary>
	///		Clase para escritura sobre un archivo Json
	/// </summary>
	public class JsonWriter : IDisposable
	{
		// Eventos públicos
		public event EventHandler<EventArguments.ProgressEventArgs> Progress;
		// Variables privadas
		System.Text.StringBuilder _builder = new System.Text.StringBuilder();

		public JsonWriter(int notifyAfter = 10_000)
		{
			NotifyAfter = notifyAfter;
		}

		/// <summary>
		///		Abre el archivo
		/// </summary>
		public void Open(string fileName, System.Text.Encoding encoding = null)
		{
			FileWriter = System.IO.File.CreateText(fileName);
		}

		/// <summary>
		///		Abre el archivo
		/// </summary>
		public void Open(System.IO.StreamWriter stream)
		{
			FileWriter = stream;
		}

		/// <summary>
		///		Manda los datos restantes al stream
		/// </summary>
		public void Flush()
		{
			if (FileWriter != null)
				FileWriter.Flush();
		}

		/// <summary>
		///		Cierra el stream
		/// </summary>
		public void Close()
		{
			if (FileWriter != null)
			{
				// Escribe la cadena JSON
				if (Rows > 1)
					FileWriter.Write("[" + _builder.ToString() + "]");
				else
					FileWriter.Write(_builder);
				// Envía los datos restantes al archivo
				Flush();
				// Cierra el stream
				FileWriter.Close();
				FileWriter = null;
			}
		}

		/// <summary>
		///		Escribe una línea
		/// </summary>
		public void WriteRow(Dictionary<string, object> values)
		{
			// Añade el separador de registros
			if (_builder.Length > 0)
				_builder.AppendLine(", ");
			// Campos
			_builder.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(values));
			// Incrementa el número de filas escritas y lanza el evento de progreso
			Rows++;
			RaiseEventReadBlock(Rows);
		}

		/// <summary>
		///		Lanza el evento de lectura de un bloque
		/// </summary>
		private void RaiseEventReadBlock(long row)
		{
			if (NotifyAfter > 0 && row % NotifyAfter == 0)
				Progress?.Invoke(this, new EventArguments.ProgressEventArgs(row));
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				// Cierra el archivo
				if (disposing)
					Close();
				// Indica que se ha liberado la memoria
				Disposed = true;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		///		Handle de archivo
		/// </summary>
		private System.IO.StreamWriter FileWriter { get; set; }

		/// <summary>
		///		Bloque de filas para las que se lanza el evento de progreso
		/// </summary>
		public int NotifyAfter { get; }

		/// <summary>
		///		Filas escritas
		/// </summary>
		public long Rows { get; private set; }

		/// <summary>
		///		Indica si se ha liberado el archivo
		/// </summary>
		public bool Disposed { get; private set; }
	}
}
