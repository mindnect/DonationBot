using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ChatApp.Incident;
using RimWorld;
using Verse;

namespace ChatApp
{
    [StaticConstructorOnStartup]
    public class AcDonationBot
    {
        public static string path = @"c:\\DonationBot\\Donation.txt";
        
        // 모드 클래스 생성자는 정적으로
        static AcDonationBot()
        {
            Log.Message(AssemblyName + "초기화");

            // Delete the file if it exists.
            if (File.Exists(AcDonationBot.path))
                File.Delete(AcDonationBot.path);
            File.Create(AcDonationBot.path).Close();


        }

        private static Assembly Assembly => Assembly.GetAssembly(typeof(AcDonationBot));

        public static string AssemblyName => Assembly.FullName.Split(',').First();

    
    }
}