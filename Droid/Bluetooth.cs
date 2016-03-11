using System;
using System.Threading;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Views;
using System.IO;
using Java.Util;
using Android.Bluetooth;
using System.Threading.Tasks;
using BluetoothCP.Doid;

[assembly :Xamarin.Forms.Dependency (typeof(Bluetooth))]

namespace BluetoothCP.Doid {
	public class Bluetooth : IBluetooth{
		//UUID da placa Bluetooth
		private UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
		//MAC Address do dispositivo Bluetooth
		private String MEC_ADRESS = "";

		private BluetoothAdapter adaptadorBluetooth = null;
		private BluetoothSocket socketBluetooth = null;
		private BluetoothDevice device = null;

		//Streams de leitura I/O
		private Stream outStream = null;
		private Stream inStream = null;

		public Bluetooth(){
			
		}

		public void setMec(String Mac){
			this.MEC_ADRESS = Mac;
		}

		//Metodo para verificar o sensor Bluetooth
		private void CheckBluetoothDoAparelho() {
			//pega instancia do adaptador Bluetooth do aparelho
			adaptadorBluetooth = BluetoothAdapter.DefaultAdapter;

			//Está habilitado?
			if (!adaptadorBluetooth.Enable()) {
				throw new System.Exception ("Bluetooth Desativado");
			}
			//verifica s eo sensor Bluetooth é nulo
			if (adaptadorBluetooth == null) {
				throw new System.Exception ("Bluetooth No Existe o esta Ocupado");
			}

		}

		public void Conectar() {
			this.CheckBluetoothDoAparelho();
			while(!adaptadorBluetooth.IsEnabled){//Espera bluetooth ativar
				
			}
			if(this.MEC_ADRESS.Equals("")){
				throw new System.Exception("Informe o Mac da placa bluetooth a ser conectada.");
			}
			if(socketBluetooth !=null && socketBluetooth.IsConnected && device!=null){
				throw new System.Exception("Bluetooth já está pareado.");
			}
			try{
				device = adaptadorBluetooth.GetRemoteDevice(this.MEC_ADRESS);
			}catch(Exception ex){
				Console.WriteLine (ex.Message);
				throw new System.Exception("Endereço Bluetooth inválido");
			}

			//Parar o processo de pesquisa de dispositivos
			adaptadorBluetooth.CancelDiscovery();

			//Inicamos o socket de comunicação com o arduino
			//socketBluetooth = device.CreateRfcommSocketToServiceRecord(MY_UUID);
			socketBluetooth = device.CreateInsecureRfcommSocketToServiceRecord(MY_UUID);


			try{
				socketBluetooth.Connect();
			}catch(Exception ex){
				Console.WriteLine (ex.Message);
				throw new System.Exception("Conexão não pode ser estabelecida.");
			}
				
		}

		public void Desconectar() {
			if (socketBluetooth != null && socketBluetooth.IsConnected) {
				socketBluetooth.Close();
			} 
			//else {
			//	throw new System.Exception("Imposivel Desconectar.");
			//}
		}

		//Metodo de envio de dados pelo bluetooth
		public void write(String mensagem) {
			if(!socketBluetooth.IsConnected){
				throw new System.Exception("Bluetooth ainda não pareado.");
			}
			this.outStream = socketBluetooth.OutputStream; 


			Java.Lang.String msg = new Java.Lang.String(mensagem);

			//Converte em Bytes
			byte[] msgBuffer = msg.GetBytes();

			//envia o buffer
			outStream.Write(msgBuffer, 0, msgBuffer.Length);
		}


		public Stream getInputStream(){
			this.inStream = socketBluetooth.InputStream;
			return this.inStream;
		}

		public Boolean isConectado(){
			return socketBluetooth.IsConnected;
		}


	}

}

	