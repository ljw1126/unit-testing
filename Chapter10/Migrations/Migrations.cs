using System;

// 엔티티 모델 - 디비 동기화 대한 설명에서 나온 코드 예제를 보여주는 듯 하다
namespace unit_testing.Chapter10.Migrations
{
    [Migration(1)]
    public class CreateUserTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users");
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }

    public class Delete
    {
        public static void Table(string users)
        {
            throw new NotImplementedException();
        }
    }

    public class Create
    {
        public static void Table(string users)
        {
            throw new NotImplementedException();
        }
    }

    public class Migration
    {
        public virtual void Up()
        {
            throw new NotImplementedException();
        }

        public virtual void Down()
        {
            throw new NotImplementedException();
        }
    }

    public class MigrationAttribute : Attribute
    {
        public MigrationAttribute(int i)
        {
            throw new NotImplementedException();
        }
    }
}