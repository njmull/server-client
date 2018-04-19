using SpreadsheetGUI;
using System;
using System.Windows.Forms;


namespace SS
{
    /// <summary>
    /// Keeps track of how many top-level forms are running
    /// </summary>
    class SpreadsheetApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetApplicationContext.
        /// </summary>
        public static SpreadsheetApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new SpreadsheetApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }


    static class Program
    {
        public static Form1 MainForm = new Form1();
        public static Form2 ConnectForm = new Form2();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            SpreadsheetApplicationContext appContext = SpreadsheetApplicationContext.getAppContext();
            if (args.Length == 0)
            {
                appContext.RunForm(ConnectForm);
                appContext.RunForm(MainForm);
                Application.Run(appContext);
            }
        }
    }

}