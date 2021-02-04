using Livet;
using System.Windows;

namespace FluGASv25
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            // Database Exist check
            var mes = string.Empty;
            Dao.CreateSQL.DbCreate.CheckDb(
                Dao.CreateSQL.DbCreate.GetDbTables(),
                ref mes);


            // TEST MODE.
            if (FluGASv25.Properties.Settings.Default.testmode)
            {
                System.Diagnostics.Debug.WriteLine("## test start.");


                Unit.UnitTest.DoTestAll();

                System.Diagnostics.Debug.WriteLine("## test end.");
                Application.Current.Shutdown();
            }

            DispatcherHelper.UIDispatcher = Dispatcher;
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

    }
}
