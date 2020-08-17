using App.Utils;
using App.Entities;
//using EntityFramework.Extensions;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Linq;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>���ݿ⹹�������������Լ����</summary>
    public class ExtendedSqlGenerator : SqlServerMigrationSqlGenerator
    {
        protected override void Generate(DropForeignKeyOperation dropForeignKeyOperation)
        {
            return;
        }
        protected override void Generate(AddForeignKeyOperation addForeignKeyOperation)
        {
            return;
        }
    }


    /// <summary>
    /// Model�������Զ��������ݿ�
    /// �ο� http://www.tuicool.com/articles/EfIvey
    /// </summary>
    internal sealed class AppMigrationConfiguration : DbMigrationsConfiguration<AppContext>
    {
        public AppMigrationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "AppPlat";  // EF��������keyȥ����_MigrationHistory����ʵ��ģ�ͱ�������ݿ����
            SetSqlGenerator("System.Data.SqlClient", new ExtendedSqlGenerator());
        }

        // ÿ�����ݿ��������
        protected override void Seed(AppContext context)
        {
            //  This method will be called after migrating to the latest version.
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );

            // ����״̬������
            // ����ע��Ϣд�����ݿ�
            //ResetEnumData();
            //WriteComments();
        }


        //------------------------------------------------------
        // ��ö����Ϣд�����ݿ�
        //------------------------------------------------------
        /// <summary>���÷�ϵͳ��������</summary>
        public static void ResetEnumData()
        {
            ClearEnumData();
            AddEnum(typeof(FinanceType));
            AddEnum(typeof(ArticleType));
            AddEnum(typeof(ProductType));
            AddEnum(typeof(OrderStatus));
            AddEnum(typeof(OrderPayMode));
            AddEnum(typeof(InviteSource));
            AddEnum(typeof(InviteStatus));
            //AddEnum(typeof(Role));
            AddEnum(typeof(Powers));
            AddEnum(typeof(LogLevel));
        }

        //-------------------------------------------
        // ö����Ϣ������ Config ���У����¼��
        //-------------------------------------------
        /// <summary>���ö������</summary>
        public static void ClearEnumData()
        {
            XState.Set
                .Where(t => t.Category != "Site")
                .Where(t => t.Category != "AppBanner")
                .Delete();
        }


        /// <summary>��ö��ֵ���룬��Ϊ�����ֵ�</summary>
        public static void AddEnum(Type enumType)
        {
            var category = enumType.Name;
            foreach (object value in Enum.GetValues(enumType))
            {
                var id = (int)value;
                var name = value.GetTitle();
                var cfg = new XState() { Category = category, Key = value.ToString(), Value = id.ToString(), Title = name };
                cfg.Save(false);
            }
        }

        //------------------------------------------------------
        // ����ע��Ϣд�����ݿ�
        //------------------------------------------------------
        /// <summary>
        /// ����ע��Ϣд�����ݿ��
        /// ����AppContext�����е�DbSet���ԣ���ȡ����Ԫ����𣬸�����UIAttribute��ȡ��ע��Ϣ����д�����ݿ⡣
        /// ���ޣ�
        /// ��1���ֽ׶�ֻ֧��sqlserver��
        /// ��2���޷�����ʵ�������������ȡ��ȷ�����ݿ�������ֶ�������[Table][Column]�ȡ�
        /// ��3������Ҫ�о� DbModelBuilder �࣬��úÿ���EF��Դ�룬����������ôӳ��ġ�
        /// ��4���պ�����
        /// </summary>
        public void WriteComments()
        {
            foreach (var prop in this.GetType().GetProperties())
            {
                Type type = prop.PropertyType;
                if (type.IsGenericType && type.Name.Contains("DbSet"))
                {
                    string table = prop.Name;
                    Type itemType = type.GenericTypeArguments[0]; // ��ȡ���Ͳ�����
                    WriteComment(table, itemType);
                }
            }
        }

        /// <summary>
        /// ��ʵ�������Եı�עд�����ݿ⡣
        /// </summary>
        void WriteComment(string tableName, Type type)
        {
            //DbModelBuilder builder = new DbModelBuilder();
            foreach (var prop in type.GetProperties())
            {
                var desc = prop.GetTitle();
                if (prop.CanWrite && !desc.IsEmpty())
                {
                    var schema = "dbo";
                    var table = tableName;
                    var column = prop.Name;
                    var sql1 = string.Format(@"
                        EXEC sys.sp_dropextendedproperty 
                            @name='MS_Description', 
                            @level0type=N'SCHEMA',@level0name=N'{0}', 
                            @level1type=N'TABLE', @level1name=N'{1}', 
                            @level2type=N'COLUMN',@level2name=N'{2}'
                        ", schema, table, column
                        );
                    var sql2 = string.Format(@"
                        EXEC sys.sp_addextendedproperty 
                            @name='MS_Description', @value=N'{0}', 
                            @level0type=N'SCHEMA',@level0name=N'{1}', 
                            @level1type=N'TABLE', @level1name=N'{2}', 
                            @level2type=N'COLUMN',@level2name=N'{3}'
                        ", desc, schema, table, column
                        );
                    try { AppContext.Current.Database.ExecuteSqlCommand(sql1); } catch { }
                    try { AppContext.Current.Database.ExecuteSqlCommand(sql2); } catch { }
                }
            }
        }
    }
}
