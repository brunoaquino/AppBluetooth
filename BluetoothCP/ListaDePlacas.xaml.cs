using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BluetoothCP
{
	public partial class ListaDePlacas : ContentPage
	{
		public List<Conexao> Conexoes { get; set;}
		private IBluetooth bluetooth;

		public ListaDePlacas ()
		{
			Conexoes = new List<Conexao> ();

			BindingContext = this;

			InitializeComponent ();


			for(int i = 0; i<=15; i++){
				Conexao conexao = new Conexao ();

				conexao.Id = i;
				conexao.Mec = "98:D3:31:20:95:77";
				conexao.Placa = "NKT-125"+ i;

				Conexoes.Add (conexao);
			}

		}

		public void AcaoItem(Object o, ItemTappedEventArgs e) {
			Conexao conexaoClicada = e.Item as Conexao;
			try{
				bluetooth = DependencyService.Get<IBluetooth> ();
				bluetooth.setMec (conexaoClicada.Mec);
				bluetooth.Conectar();
			}catch(Exception ex){
				DisplayAlert ("Atenção", ex.Message, "OK");
				return;
			}
			ControleBluetooth telaControleBluetooth = new ControleBluetooth (conexaoClicada,bluetooth);
			Navigation.PushAsync (telaControleBluetooth);
		}
			
	}
}

