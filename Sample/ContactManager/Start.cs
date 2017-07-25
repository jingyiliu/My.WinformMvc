using System;
using System.Windows.Forms;
using ContactManager.Repository;
using ContactManager.Services;
using ContactManager.Views;
using My.IoC;
using My.WinformMvc;

namespace ContactManager
{
    static class Start
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                        
            try
            {
                var container = new ObjectContainer(false);
                container.Register(typeof (Database)).In(Lifetime.Container());
                container.Register(typeof(ILoginService), typeof(LoginService)).In(Lifetime.Container());
                container.Register(typeof(IContactService), typeof(ContactService)).In(Lifetime.Container());

                IIocWrapper iocWrapper = new IoCWrapper(container);
                IPairManager pairManager = new PairManager();
                pairManager.RegisterAssembly(typeof(LoginView).Assembly);
                pairManager.RegisterAssembly(typeof(Start).Assembly);

                ICoordinator coordinator = new Coordinator(pairManager, iocWrapper);
                coordinator.StartApplication("ListController");
            }
            catch (Exception e)
            {
                Console.WriteLine("...Error while initializing the application please contact thhe administrator...");
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
