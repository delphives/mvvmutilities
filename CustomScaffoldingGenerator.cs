using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace BlazorGeekLandFacture.Helpers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    class CustomEntityTypeGenerator : CSharpEntityTypeGenerator
    {
        public CustomEntityTypeGenerator(IAnnotationCodeGenerator annotationCodeGenerator, ICSharpHelper helper) : base(annotationCodeGenerator, helper) { }

        public override string WriteCode(IEntityType type, string @namespace, bool useDataAnnotations)
        {
            //useDataAnnotations = true;
            /*Console.WriteLine($"-->{code}<--");
            var old = "public partial class " + type.Name;
            var updated = "[System.CodeDom.Compiler.GeneratedCode]\n" + old;
            return code.Replace(old, updated).Replace("namespace", "#nullable disable\n\nnamespace");*/

            //pour commenter toute la classe entity
            //var code = base.WriteCode(type, @namespace, useDataAnnotations);
            //return Transform(code);

            //pour ne rien faire
            return base.WriteCode(type, @namespace, useDataAnnotations);
        }

        //met tout ce qui se trouve dans la classe en commentaire
        private string CommentAll(string code)
        {
            string[] tmp = code.Split('{');
            string code2 = "";
            for (int i = 0; i < tmp.Length; i++)
            {
                if (i <= 1)
                {
                    code2 += tmp[i];
                }
                else if (i == 2)
                {
                    code2 += $"/*{tmp[i]}";
                }
                else
                {
                    code2 += tmp[i];
                }
                //on remet le signe de départ
                if (i != tmp.Length - 1)
                {
                    code2 += "{";
                }
            }
            tmp = code2.Split('}');
            code = "";
            for (int i = 0; i < tmp.Length; i++)
            {
                if (i == tmp.Length - 2)
                {
                    code += tmp[i] + "*/";
                }
                else
                {
                    code += tmp[i];
                }
                if (i != tmp.Length - 3)
                    code += "}";
            }
            return code;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    class CustomDbContextGenerator : CSharpDbContextGenerator
    {
        public CustomDbContextGenerator(
            IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator,
            IAnnotationCodeGenerator annotationCodeGenerator,
            ICSharpHelper cSharpHelper)
            : base(providerConfigurationCodeGenerator, annotationCodeGenerator, cSharpHelper)
        { }

        public override string WriteCode(IModel model, string contextName, string connectionString, string contextNamespace, string modelNamespace, bool useDataAnnotations, bool suppressConnectionStringWarning, bool suppressOnConfiguring)
        {
            // There is no good way to override the DbSet naming, as it uses 
            // an internal StringBuilder. This means we can't override 
            // AddDbSetProperties without re-implementing the entire class.
            // Therefore, we have to get the code and then do string manipulation 
            // to replace the DbSet property code

            //System.Console.WriteLine($"--> model :{model}");
            //System.Console.WriteLine($"--> contextName : {contextName}");
            //System.Console.WriteLine($"--> connectionString : {connectionString}");
            //System.Console.WriteLine($"--> contextNamespace : {contextNamespace}");
            //System.Console.WriteLine($"--> modelNamespace : {modelNamespace}");
            //System.Console.WriteLine($"--> useDataAnnotations : {useDataAnnotations}");
            //useDataAnnotations = true;
            //System.Console.WriteLine($"--> suppressConnectionStringWarning : {suppressConnectionStringWarning}");
            //System.Console.WriteLine($"--> suppressOnConfiguring : {suppressOnConfiguring}");
            var code = base.WriteCode(model, contextName, connectionString, contextNamespace, modelNamespace, useDataAnnotations, suppressConnectionStringWarning, suppressOnConfiguring);

            //foreach (var entityConfig in modelConfiguration.EntityConfigurations)
            //{
            //    var entityName = entityConfig.EntityType.Name;
            //    var setName = Inflector.Inflector.Pluralize(entityName) ?? entityName;

            //    code = code.Replace(
            //        $"DbSet<{entityName}> {entityName}",
            //        $"DbSet<{entityName}> {setName}");
            //}

            //System.Console.WriteLine($"-->{code}");

            //pour supprimer les 2 constructeurs dans la génération du DbContext
            code = CommentConstructors(code, contextName);
            code = AddDefaultConstructorWithJsonReadingAndConnectionString(code, "appsettings.json", "_connectionString");
            code = AddOnConfiguringWithLazyLoading(code, "_connectionString");

            return code;
        }

        /**
         * ajout de la méthode OnConfiguring avec le LazyLoading
         * et la connexion en param. la méthode est écrite après le constructeur
         */
        private string AddOnConfiguringWithLazyLoading(string code, string connectionStringName)
        {
            string[] list = code.Split('\n');
            List<string> listFinal = new List<string>();
            string lineToAdd = "";
            bool isDone = false;
            foreach (string line in list)
            {
                lineToAdd = "";
                //fin du constructeur
                if (line.Contains("}") && !isDone)
                {
                    lineToAdd = line + "\n\n";
                    lineToAdd += "protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)\n" +
                        "{\n" +
                        "optionsBuilder\n" +
                    ".UseLazyLoadingProxies()\n" +
                    $".UseSqlServer({connectionStringName});\n" +
                    "}";
                    isDone = true;
                }
                if (lineToAdd == "")
                {
                    lineToAdd = line;
                }

                listFinal.Add(lineToAdd);
            }
            return string.Join('\n', listFinal);
        }

        /**
         * Ajout d'un constructeur par défaut avec lecteur d'un fichier json (param1)
         * et lecture d'une valeur dans le fichier (param2) pour la connexion string
         */
        private string AddDefaultConstructorWithJsonReadingAndConnectionString(string code, string fileName, string connectionStringName)
        {
            string[] list = code.Split('\n');
            List<string> listFinal = new List<string>();
            string lineToAdd = "";
            int count = 0;
            foreach (string line in list)
            {
                lineToAdd = "";
                if (line.Contains("{"))
                {
                    count++;
                    if (count == 2)
                        //si on a trouvé pour la 2ème fois on continue car c'est la prochaine ligne qui nous intéresse
                        continue;
                }
                    
                if (count == 2)
                {
                    lineToAdd = "private readonly string _connectionString;\n\n";
                    lineToAdd += "public SqlServerContext()\n" +
                        "{\n" +
                        "var builder = new ConfigurationBuilder()\n" +
                            ".SetBasePath(Directory.GetCurrentDirectory())\n" +
                            $".AddJsonFile(\"{fileName}\", optional: false);\n" +
                    $"IConfiguration config = builder.Build();\n" +
                        $"_connectionString = config.GetSection(\"{connectionStringName}\").Value;\n" +
                    "}\n\n";
                }
                if (lineToAdd == "")
                {
                    lineToAdd = line;
                }

                listFinal.Add(lineToAdd);
            }
            return string.Join('\n', listFinal);
        }

        /**
         * commente les constructeurs du DbContext
         */
        private string CommentConstructors(string code, string contextName)
        {
            //on cherche les lignes contenant le constructeur et on les commente
            string[] list = code.Split('\n');
            List<string> listFinal = new List<string>();
            bool isFound = false;
            string lineToAdd = "";
            foreach (string line in list)
            {
                lineToAdd = "";
                //si première ligne on la commente
                if (line.Contains($"public {contextName}("))
                {
                    isFound = true;
                    lineToAdd = "//" + line;
                }
                //si dernière ligne on la commente
                if (isFound && line.Trim() == "}")
                {
                    isFound = false;
                    lineToAdd = "//" + line;
                }
                //si entre la première et la dernière on la commente
                if (isFound)
                {
                    lineToAdd = "//" + line;
                } else if (lineToAdd == "")
                { 
                    lineToAdd = line;
                }

                listFinal.Add(lineToAdd);
            }
            return string.Join('\n', listFinal);
        }
    }

    class MyDesignTimeServices : IDesignTimeServices
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICSharpEntityTypeGenerator, CustomEntityTypeGenerator>();
            serviceCollection.AddSingleton<ICSharpDbContextGenerator, CustomDbContextGenerator>();
        }
    }
}
