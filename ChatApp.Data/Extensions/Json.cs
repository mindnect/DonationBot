namespace Comm.Extensions
{
    //public class MyTypeResolver : JavaScriptTypeResolver
    //{
    //    public override Type ResolveType(string id)
    //    {
    //        return typeof(MyTypeResolver).Assembly.GetTypes().First(t => t.Name == id);
    //    }

    //    public override string ResolveTypeId(Type type)
    //    {
    //        return type.Name;
    //    }
    //}

    //public static class JSon
    //{
    //    public static JavaScriptSerializer Instance;

    //    static JSon()
    //    {
    //        Instance = new JavaScriptSerializer(new MyTypeResolver());
    //    }

    //    public static string Serialize(this object _this)
    //    {
    //        return Instance.Serialize(_this);
    //    }

    //    public static T Deserialize<T>(string s)
    //    {
    //        return Instance.Deserialize<T>(s);
    //    }
    //}
}