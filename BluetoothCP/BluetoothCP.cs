using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;


using Xamarin.Forms;

namespace BluetoothCP
{
	public class App : Application
	{
		public App ()
		{

			MainPage = new NavigationPage(new ListaDePlacas());
		}


		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

