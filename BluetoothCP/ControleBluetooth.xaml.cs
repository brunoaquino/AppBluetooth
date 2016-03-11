using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.ExceptionServices;

using Xamarin.Forms;

namespace BluetoothCP
{
	public partial class ControleBluetooth : ContentPage
	{
		private IBluetooth Bluetooth{ get; set;}
		private Conexao Conexao{ get; set;}
		Dictionary<String, String> hashMapParametros;
		private String mensagemRecebida = "";

		public ControleBluetooth (Conexao conexao,IBluetooth bluetooth)
		{
			Bluetooth = bluetooth;
			Conexao = conexao;
			InitializeComponent ();

			lblStatus.Text = "Ativado";
			mecBluetooth.Text = conexao.Mec;
			beginListenForData();
			bluetooth.write("connect");
		}

		public void conectaBluetooth(Object sender, EventArgs e){
			try{
				lblStatus.Text = "Ativado";
				beginListenForData();
			}catch(Exception ex){
				DisplayAlert ("Atenção", ex.Message, "OK");
			}
		}

		protected override void OnDisappearing()
		{
			Bluetooth.Desconectar ();
		}


		public void acendeLuz(Object sender, EventArgs e){
			try{
				Bluetooth.write("liga");
			}catch(Exception ex){
				DisplayAlert ("Atenção", "Conecte o Bluetooth primeiro", "OK");
			}
		}

		public void apagaLuz(Object sender, EventArgs e){
			try{
				Bluetooth.write("desliga");
			}catch(Exception ex){
				DisplayAlert ("Atenção", "Conecte o Bluetooth primeiro", "OK");
			}
		}

		public void getParametrosDeEntrada(String msgEntrada){
			if (msgEntrada.Substring (0, 1).Equals (" ")) {
				return;
			}

			if (!msgEntrada.Contains(";")) {
				mensagemRecebida += msgEntrada;
				return;
			} else {
				mensagemRecebida += msgEntrada;
			}
			mensagemRecebida = mensagemRecebida.Replace (";", "");

			hashMapParametros = new Dictionary<String, String>();
			String parametro = "";

			char[] arrayParametros = mensagemRecebida.ToCharArray();
			for(int i = 0 ; i < arrayParametros.Length ; i++){
				char caracter = arrayParametros [i];
				if(caracter=='&'){
					parametro = "";
					continue;
				}
				if(caracter=='\0' || caracter==' '){
					break;
				}
				if(caracter=='='){
					hashMapParametros.Add(parametro, arrayParametros [++i].ToString());
				}else{
					parametro += caracter;
				}

			}
		}

		public void carregaRespostas(){
			foreach(var prmt in hashMapParametros){
				if(prmt.Key.Equals("statusLed")){
					if (prmt.Value.Equals ("0")) {
						lblRecebido.Text = "Desligado";
					} else {
						lblRecebido.Text = "ligado";
					}
				}

			}

		}

		//Evento para incializar a escuta do bluetooth
		public async void beginListenForData()   {
			//Cria thread para ficar monitorando chegada de dados
			Task.Factory.StartNew (() => {
				while (true) {
					try {
						Stream inStream = Bluetooth.getInputStream();
						byte[] buffer = new byte[1024];
						int bytes;
						inStream.Flush();
						bytes = inStream.Read(buffer, 0, buffer.Length);

						if(bytes>0){
							//Carrega na interface principal
							Device.BeginInvokeOnMainThread(()=>{
							//	string valor = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
								string valorRecebido = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length).Replace("\0", string.Empty); ;
								getParametrosDeEntrada(valorRecebido);
							});
						}
					} catch (Exception e) {
						Device.BeginInvokeOnMainThread(()=>{
							Navigation.RemovePage(this);
							lblStatus.Text = "Desativado";
							this.DisplayAlert ("Atenção", "Bluetooth desconectado.", "OK");
						});
						break;
					}
				}
			});
		}

	}
}

