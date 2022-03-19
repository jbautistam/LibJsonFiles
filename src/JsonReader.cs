using System;
using System.Collections.Generic;
using System.Data;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bau.Libraries.LibJsonFiles
{
	/// <summary>
	///		Implementación de <see cref="IDataReader"/> para archivos Json
	/// </summary>
	public class JsonReader : IDataReader
	{
		// Eventos públicos
		public event EventHandler<EventArguments.ProgressEventArgs> Progress;
		// Variables privadas
		private bool _streamOpenedFromReader;
		private System.IO.StreamReader _fileReader;
		private List<JObject> _jsonValues;
		private List<object> _recordValues;
		private List<string> _headers = new List<string>();
		private int _row;

		public JsonReader(int notifyAfter = 10_000)
		{
			NotifyAfter = notifyAfter;
		}

		/// <summary>
		///		Abre el archivo
		/// </summary>
		public void Open(string fileName)
		{
			// Indica que el stream se ha abierto en la librería
			_streamOpenedFromReader = true;
			// Abre el archivo sobre el stream
			Open(new System.IO.StreamReader(fileName, true));
		}

		/// <summary>
		///		Abre el datareader sobre el stream
		/// </summary>
		public void Open(System.IO.StreamReader stream)
		{
			string json = stream.ReadToEnd();

				// Convierte la cadena Json
				if (!string.IsNullOrWhiteSpace(json))
					_jsonValues = JsonConvert.DeserializeObject<List<JObject>>(json);
				else
					_jsonValues = new List<JObject>();
				// Lee las cabeceras
				_headers = GetHeaders(_jsonValues);
				// e indica que aún no se ha leido ninguna línea
				_row = 0;
		}

		/// <summary>
		///		Obtiene las cabeceras
		/// </summary>
		private List<string> GetHeaders(List<JObject> jsonValues)
		{
			List<string> headers = new List<string>();

				// Obtiene las cabeceras con los datos del primer objeto
				if (jsonValues.Count > 0)
					foreach (KeyValuePair<string, JToken> keyToken in jsonValues[0])
						headers.Add(keyToken.Key);
				// Devuelve las cabeceras
				return headers;
		}

		/// <summary>
		///		Lee un registro
		/// </summary>
		public bool Read()
		{
			bool readed = false;

				// Lee la fila
				if (_row < _jsonValues.Count)
				{
					// Convierte la fila
					_recordValues = ConvertFields(_jsonValues[_row]);
					// Incrementa el número de líne y lanza el evento
					_row++;
					RaiseEventReadBlock(_row);
					// Indica que se han leido datos
					readed = true;
				}
				// Devuelve el valor que indica si se han leído datos
				return readed;
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
		///		Convierte el objeto leido
		/// </summary>
		private List<object> ConvertFields(JObject jsonObject)
		{
			List<object> values = new List<object>();

				// Inicializa la lista de objetos (tienen que coincidir con el número de cabeceras)
				for (int index = 0; index < _headers.Count; index++)
					values.Add(null);
				// Convierte cada uno de los valores
				foreach (KeyValuePair<string, JToken> keyToken in jsonObject)
				{
					int index = GetOrdinal(keyToken.Key);

						if (index >= 0)
							values[index] = ConvertJsonObject(keyToken.Value.Type, keyToken.Value);	
				}
				// Devuelve la lista de valores
				return values;
		}

		/// <summary>
		///		Convierte un valor de Json en un valor .Net
		/// </summary>
		private object ConvertJsonObject(JTokenType? type, JToken value)
		{
			switch (type)
			{
				case JTokenType.Integer:
					return (int?) value;
				case JTokenType.Float:
					return (float?) value;
				case JTokenType.String:
					return (string) value;
				case JTokenType.Boolean:
					return (bool) value;
				case JTokenType.Date:
					return (DateTime?) value;
				case JTokenType.Bytes:
					return (byte[]) value;
				case JTokenType.Guid:
					return (Guid?) value;
				case JTokenType.Uri:
					return (Uri) value;
				case JTokenType.TimeSpan:
					return (TimeSpan?) value;
				case JTokenType.Null:
					return DBNull.Value;
				default:
					return null;
			}
		}

		/// <summary>
		///		Cierra el archivo
		/// </summary>
		public void Close()
		{
			if (_streamOpenedFromReader && _fileReader != null)
			{
				// Cierra el archivo
				_fileReader.Close();
				// y libera los datos
				_fileReader = null;
			}
		}

		/// <summary>
		///		Obtiene el nombre del campo
		/// </summary>
		public string GetName(int i)
		{
			return _headers[i];
		}

		/// <summary>
		///		Obtiene el nombre del tipo de datos
		/// </summary>
		public string GetDataTypeName(int i)
		{
			return _recordValues[i].GetType().Name;
		}

		/// <summary>
		///		Obtiene el tipo de un campo
		/// </summary>
		public Type GetFieldType(int i)
		{
			return _recordValues[i].GetType();
		}

		/// <summary>
		///		Obtiene el valor de un campo
		/// </summary>
		public object GetValue(int i)
		{
			return _recordValues[i];
		}

		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		public int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public bool GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		public byte GetByte(int i)
		{
			throw new NotImplementedException();
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public char GetChar(int i)
		{
			throw new NotImplementedException();
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			throw new NotImplementedException();
		}

		public Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		public short GetInt16(int i)
		{
			throw new NotImplementedException();
		}

		public int GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		public long GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		public double GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		public string GetString(int i)
		{
			throw new NotImplementedException();
		}

		public decimal GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		public DateTime GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		public IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Obtiene el índice de un campo a partir de su nombre
		/// </summary>
		public int GetOrdinal(string name)
		{
			// Obtiene el índice del registro
			if (!string.IsNullOrWhiteSpace(name))
				for (int index = 0; index < _headers.Count; index++)
					if (_headers[index].Equals(name, StringComparison.CurrentCultureIgnoreCase))
						return index;
			// Si ha llegado hasta aquí es porque no ha encontrado el campo
			return -1;
		}

		/// <summary>
		///		Indica si el campo es un DbNull
		/// </summary>
		public bool IsDBNull(int index)
		{
			return index >= _recordValues.Count || _recordValues[index] == null || _recordValues[index] is DBNull;
		}

		/// <summary>
		///		Los CSV sólo devuelven un Resultset, de todas formas, DbDataAdapter espera este valor
		/// </summary>
		public bool NextResult()
		{
			return false;
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Libera los datos
				if (disposing)
					Close();
				// Indica que se ha liberado
				IsDisposed = true;
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
		///		Profundidad del recordset
		/// </summary>
		public int Depth 
		{ 
			get { return 0; }
		}

		/// <summary>
		///		Indica si está cerrado
		/// </summary>
		public bool IsClosed 
		{ 
			get { return _fileReader == null; }
		}

		/// <summary>
		///		Registros afectados
		/// </summary>
		public int RecordsAffected 
		{ 
			get { return -1; }
		}

		/// <summary>
		///		Bloque de filas para las que se lanza el evento de progreso
		/// </summary>
		public int NotifyAfter { get; }

		/// <summary>
		///		Indica si se ha liberado el recurso
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		///		Número de campos a partir de las columnas
		/// </summary>
		public int FieldCount 
		{ 
			get 
			{ 
				return _headers.Count; 
			}
		}

		/// <summary>
		///		Indizador por número de campo
		/// </summary>
		public object this[int i] 
		{ 
			get { return _recordValues[i]; }
		}

		/// <summary>
		///		Indizador por nombre de campo
		/// </summary>
		public object this[string name]
		{ 
			get 
			{ 
				int index = GetOrdinal(name);

					if (index >= _recordValues.Count)
						return null;
					else
						return _recordValues[GetOrdinal(name)]; 
			}
		}
	}
}
