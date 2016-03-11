using System;
using System.IO;

namespace BluetoothCP
{
	public interface IBluetooth
	{
		void setMec(String mec);
		void Conectar();
		void Desconectar();
		void write(String mensagem);
		Stream getInputStream();
		Boolean isConectado();
	}
}

