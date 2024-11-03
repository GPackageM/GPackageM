using System.Runtime.InteropServices;
using System.Security.Principal;
using GPacket;
using GPacket.Enums;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Task = System.Threading.Tasks.Task;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (!IsAdministrator && IsUnix)
        {
            Console.WriteLine("This program must be run as an administrator.");
            return;
        }
        
        Command cmdType;
        PackageType pkgType;

        if (!File.Exists("config.yml"))
        {
            Console.WriteLine("Generating config.yml...");
            GenerateConfig();
        }
        
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: gpacket <command> <package-type> <package-name>");
            return;
        }

        switch (args[0].ToLower())
        {
            case "install":
                cmdType = Command.Install;
                break;
            case "upgrade":
                cmdType = Command.Upgrade;
                break;
            case "remove":
                cmdType = Command.Remove;
                break;
            default:
                Console.WriteLine("Invalid command. Supported commands: install, upgrade, remove"); 
                return;
        }
        
        switch (args[1].ToLower())
        {
            case "package":
                pkgType = PackageType.Pkg;
                break;
            case "pkg":
                pkgType = PackageType.Pkg;
                break;
            case "project":
                pkgType = PackageType.Project;
                break;
            case "proj":
                pkgType = PackageType.Project;
                break;
            default:
                Console.WriteLine("Invalid type. Supported types: package, project");
                return;
        }

        switch (cmdType)
        {
            case Command.Install:
                await PackageManager.InstallPackage(pkgType, args[2]);
                break;
            
            case Command.Upgrade:
                Console.WriteLine("Not implemented yet.");
                break;
            
            case Command.Remove:
                PackageManager.RemovePackage(pkgType, args[2]);
                break;
        }
    }

    private static void GenerateConfig()
    {
        var tempConfig = new Config();
        
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(tempConfig);
        
        File.WriteAllText("config.yml", yaml);
    }

    public static Config ReadConfig()
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
            .Build();
        
        var tempConfig = deserializer.Deserialize<Config>(File.ReadAllText("config.yml"));

        return tempConfig;
    }
    
    private static bool IsAdministrator =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator) :
            Mono.Unix.Native.Syscall.geteuid() == 0;

    private static bool IsUnix =>
        !RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
}